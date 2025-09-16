using System.Text.RegularExpressions;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Rag.Requests;
using AseerAlkotb.Application.Features.Rag.Responses;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Application.Utils;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class RagService : AppService, IRagService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmbeddingService _emb;
        private readonly IAnswerSynthesisService _synth;
        private readonly IQuestionRouterService _router;
        private readonly ITranslationService _translator;
        private readonly IConfiguration _cfg;

        public RagService(
            IUnitOfWork uow,
            IEmbeddingService emb,
            IAnswerSynthesisService synth,
            IQuestionRouterService router,
            ITranslationService translator,
            IServiceProvider sp,
            Microsoft.Extensions.Hosting.IHostEnvironment env
        ) : base(sp, env)
        {
            _uow = uow; _emb = emb; _synth = synth; _router = router; _translator = translator;
            _cfg = sp.GetRequiredService<IConfiguration>();
        }

        public async Task<ApiResponse<RagAskResponse>> AskAsync(RagAskRequest request)
        {
            await DoValidationAsync<Application.Features.Rag.Validators.RagAskRequestValidator, RagAskRequest>(request);
            
            var originalQuestion = request.Question;
            var isEnglishQuery = await _translator.IsEnglishTextAsync(originalQuestion);
            
            // Translate English questions to Arabic for database search
            var processedQuestion = isEnglishQuery 
                ? await _translator.TranslateToArabicAsync(originalQuestion)
                : originalQuestion;
                
            var lang = LangUtils.Detect(originalQuestion); // Keep original language for response
            
            if (IsGreeting(originalQuestion))
            {
                return Success(new RagAskResponse
                {
                    Answer = Intro(lang)
                });
            }

            // 1) Use Gemini for intent detection and entity extraction on processed question
            var route = await _router.RouteAsync(processedQuestion);
            
            // 2) Trust Gemini completely - no local fallbacks or guessing
            string? title = route.entities.title;
            string? author = route.entities.author;
            string? category = SanitizeCategory(request.Category) ?? route.entities.category;
            string? publisher = route.entities.publisher;
            
            // 3) Use only the intent from Gemini - no local intent detection
            string intent = NormalizeRouterIntent(route.intent) ?? "general_recs";

            switch (intent)
            {
                case "summary":
                    {
                        if (string.IsNullOrWhiteSpace(title))
                        {
                            var response = new RagAskResponse { Answer = "برجاء تحديد اسم الكتاب المطلوب تلخيصه." };
                            return Success(await TranslateResponseIfNeededAsync(response, isEnglishQuery));
                        }
                        
                        var book = await FindBookByTitleAsync(title);
                        if (book != null)
                        {
                            bool preferDesc = string.Equals(_cfg["Rag:SummarizeFromDescription"], "true", StringComparison.OrdinalIgnoreCase);
                            if (preferDesc && !string.IsNullOrWhiteSpace(book.Description) && book.Description!.Length >= 160)
                            {
                                var response = new RagAskResponse
                                {
                                    Answer = book.Description!,
                                    Sources = ToSources(new List<BookBriefDto> {
                                        new BookBriefDto(book.Id, book.Title, book.Author?.Name, book.Price, book.DiscountedPrice, book.CoverImageUrl, book.Description)
                                    }, originalQuestion),
                                    PrimaryBookId = book.Id,
                                    PrimaryBookTitle = book.Title
                                };
                                return Success(await TranslateResponseIfNeededAsync(response, isEnglishQuery));
                            }

                            var src = new ChatSource { BookId = book.Id, Title = book.Title, CoverImageUrl = book.CoverImageUrl, Snippet = book.Description };
                            string prompt = !string.IsNullOrWhiteSpace(book.Description) && book.Description!.Trim().Length >= 120
                                ? $"لخّص كتاب \"{book.Title}\" للمؤلف {book.Author?.Name} في 3–5 نقاط بالاعتماد على الوصف أدناه."
                                : $"أعطني ملخصًا موجزًا وواضحًا لكتاب \"{book.Title}\" للمؤلف {book.Author?.Name}.";

                            var summary = await _synth.SynthesizeAsync(prompt, new List<ChatSource> { src });
                            var answer = string.IsNullOrWhiteSpace(summary) ? (book.Description ?? "لا تتوفر لدينا نبذة كافية لهذا الكتاب حاليًا.") : summary;

                            var finalResponse = new RagAskResponse
                            {
                                Answer = answer,
                                Sources = new List<ChatSource> { src },
                                PrimaryBookId = book.Id,
                                PrimaryBookTitle = book.Title
                            };
                            return Success(await TranslateResponseIfNeededAsync(finalResponse, isEnglishQuery));
                        }

                        var notFoundResponse = new RagAskResponse { Answer = $"لم أجد كتاب بعنوان «{title}». تأكد من كتابة الاسم بشكل صحيح." };
                        return Success(await TranslateResponseIfNeededAsync(notFoundResponse, isEnglishQuery));
                    }

                case "availability":
                    {
                        if (string.IsNullOrWhiteSpace(title))
                        {
                            var response = new RagAskResponse { Answer = "برجاء تحديد اسم الكتاب للاستعلام عن توافره." };
                            return Success(await TranslateResponseIfNeededAsync(response, isEnglishQuery));
                        }
                        
                        var r = await GetAvailabilityAnswerAsync(title, includePrice: false);
                        return Success(await TranslateResponseIfNeededAsync(r, isEnglishQuery));
                    }

                case "price":
                    {
                        if (string.IsNullOrWhiteSpace(title))
                        {
                            var response = new RagAskResponse { Answer = "برجاء تحديد اسم الكتاب للاستعلام عن سعره." };
                            return Success(await TranslateResponseIfNeededAsync(response, isEnglishQuery));
                        }
                        
                        var r = await GetAvailabilityAnswerAsync(title, includePrice: true);
                        return Success(await TranslateResponseIfNeededAsync(r, isEnglishQuery));
                    }

                case "author_bio":
                    {
                        string? authorName = author ?? (title != null ? await ResolveAuthorByTitleAsync(title) : null);
                        if (string.IsNullOrWhiteSpace(authorName))
                        {
                            var response = new RagAskResponse { Answer = "برجاء تحديد اسم المؤلف أو اسم كتاب له للحصول على نبذة عنه." };
                            return Success(await TranslateResponseIfNeededAsync(response, isEnglishQuery));
                        }

                        var authorRow = await _uow.Authors.GetQueryable(x => ((x.Name ?? "").ToLower()).Contains(authorName.ToLower()))
                                                          .Include(x => x.Books)
                                                          .FirstOrDefaultAsync();
                        if (authorRow == null)
                        {
                            var response = new RagAskResponse { Answer = $"لم أجد مؤلف بالاسم: {authorName}" };
                            return Success(await TranslateResponseIfNeededAsync(response, isEnglishQuery));
                        }

                        string? bio = authorRow.Bio;
                        if (string.IsNullOrWhiteSpace(bio) || bio.Trim().Length < 80)
                        {
                            bio = await _synth.SynthesizeAsync($"اكتب نبذة قصيرة وواضحة عن المؤلف {authorRow.Name}.", new List<ChatSource>())
                                  ?? $"لا تتوفر لدينا نبذة كافية عن {authorRow.Name} حاليًا.";
                        }

                        var finalResponse = new RagAskResponse { Answer = bio };
                        return Success(await TranslateResponseIfNeededAsync(finalResponse, isEnglishQuery));
                    }

                case "more_by_author":
                    {
                        string? authorName = author ?? (title != null ? await ResolveAuthorByTitleAsync(title) : null);
                        if (string.IsNullOrWhiteSpace(authorName))
                        {
                            var authorResponse = new RagAskResponse { Answer = "برجاء تحديد اسم المؤلف أو اسم كتاب له للعثور على مؤلفات أخرى." };
                            return Success(await TranslateResponseIfNeededAsync(authorResponse, isEnglishQuery));
                        }

                        var list = await GetAuthorBooksAsync(authorName);
                        var names = list.Data!.Select(b => b.Title).ToList();
                        var ans = names.Any()
                            ? $"كتب أخرى لنفس المؤلف: {string.Join("، ", names)}."
                            : "لم أجد كتبًا لنفس المؤلف.";
                        var authorBooksResponse = new RagAskResponse { Answer = ans, Sources = ToSources(list.Data!) };
                        return Success(await TranslateResponseIfNeededAsync(authorBooksResponse, isEnglishQuery));
                    }

                case "category_recs":
                    {
                        if (string.IsNullOrWhiteSpace(category))
                        {
                            var categoryResponse = new RagAskResponse { Answer = "برجاء تحديد التصنيف المطلوب (مثال: روايات، تطوير ذات، تاريخ)." };
                            return Success(await TranslateResponseIfNeededAsync(categoryResponse, isEnglishQuery));
                        }

                        var take = request.Limit > 0 ? request.Limit : 8;
                        var list = await GetCategoryBooksAsync(category, take);
                        var dedup = list.Data!.GroupBy(b => b.Title.Trim()).Select(g => g.First()).Take(take).ToList();

                        var ans = dedup.Any()
                            ? $"ترشيحات ضمن «{category}»: {string.Join("، ", dedup.Select(b => b.Title))}."
                            : $"لا توجد نتائج ضمن التصنيف «{category}».";
                        var categoryRecsResponse = new RagAskResponse { Answer = ans, Sources = ToSources(dedup, originalQuestion) };
                        return Success(await TranslateResponseIfNeededAsync(categoryRecsResponse, isEnglishQuery));
                    }

                case "similar_to_title":
                    {
                        if (string.IsNullOrWhiteSpace(title))
                        {
                            var similarResponse = new RagAskResponse { Answer = "برجاء تحديد اسم الكتاب للبحث عن كتب مشابهة له." };
                            return Success(await TranslateResponseIfNeededAsync(similarResponse, isEnglishQuery));
                        }

                        var take = request.Limit > 0 ? request.Limit : 8;
                        var recs = await SimilarByTitleAsync(title, take);
                        var ans = recs.Any()
                            ? $"كتب تشبه «{title}»: {string.Join("، ", recs.Select(b => b.Title))}."
                            : $"لم أجد ترشيحات قريبة من «{title}».";
                        var similarTitleResponse = new RagAskResponse { Answer = ans, Sources = ToSources(recs, originalQuestion) };
                        return Success(await TranslateResponseIfNeededAsync(similarTitleResponse, isEnglishQuery));
                    }

                case "publisher_info":
                    {
                        return Success(await HandlePublisherInfoAsync(title, publisher, request.Question));
                    }

                case "publisher_books":
                    {
                        return Success(await HandlePublisherBooksAsync(publisher, request.Limit, request.Question));
                    }

                default:
                    {
                        var top = await GetRecommendationsAsync(processedQuestion);
                        var reply = top.Data!.Any()
                            ? $"لم أجد نتائج مطابقة تمامًا لسؤالك، لكن بناءً على ما بحثت عنه أرشح لك: {string.Join("، ", top.Data!.Select(x => x.Title))}."
                            : "لم أجد كتباً متعلقة مباشرة بسؤالك. جرّب كلمات مفتاحية مختلفة أو تصنيفاً آخر.";
                        
                        var defaultResponse = new RagAskResponse { Answer = reply, Sources = ToSources(top.Data ?? new List<BookBriefDto>(), originalQuestion) };
                        return Success(await TranslateResponseIfNeededAsync(defaultResponse, isEnglishQuery));
                    }
            }
        }

        private async Task<RagAskResponse> TranslateResponseIfNeededAsync(RagAskResponse response, bool isEnglishQuery)
        {
            if (!isEnglishQuery) return response; // Keep Arabic response for Arabic queries
            
            // Translate Arabic response to English
            var translatedAnswer = await _translator.TranslateToEnglishAsync(response.Answer);
            
            return new RagAskResponse
            {
                Answer = translatedAnswer,
                Sources = response.Sources,
                IsAvailable = response.IsAvailable,
                PrimaryBookId = response.PrimaryBookId,
                PrimaryBookTitle = response.PrimaryBookTitle,
                CanAddToCart = response.CanAddToCart
            };
        }

        // ====== Public Queries ======
        public async Task<ApiResponse<string>> GetBookAvailabilityAsync(string bookTitle)
        {
            var lower = (bookTitle ?? "").ToLower();
            var b = await _uow.Books.GetQueryable(
                        x => x.IsActive && ((x.Title ?? "").ToLower().Contains(lower)),
                        q => q.Include(x => x.Author))
                    .OrderByDescending(x => x.SalesCount)
                    .FirstOrDefaultAsync();

            if (b == null)
                return Success<string>($"لم يتم العثور على كتاب بعنوان: {bookTitle}");

            var available = b.StockQuantity > 0;
            var ans = available ? $"\"{b.Title}\" متاح للشراء الآن." : $"\"{b.Title}\" غير متاح حاليًا.";
            return Success(ans);
        }

        public async Task<ApiResponse<List<BookBriefDto>>> GetAuthorBooksAsync(string authorName)
        {
            var items = await _uow.Books.GetQueryable(
                    x => x.IsActive && x.Author != null && ((x.Author.Name ?? "").ToLower()).Contains(authorName.ToLower()),
                    q => q.Include(b => b.Author))
                .OrderByDescending(b => b.SalesCount).ThenByDescending(b => b.ViewCount)
                .Take(20)
                .ToListAsync();

            return Success(items.Adapt<List<BookBriefDto>>());
        }

        public async Task<ApiResponse<List<BookBriefDto>>> GetCategoryBooksAsync(string categoryName)
            => await GetCategoryBooksAsync(categoryName, 20);

        public async Task<ApiResponse<List<BookBriefDto>>> GetCategoryBooksAsync(string categoryName, int take)
        {
            categoryName = categoryName?.Trim() ?? "";
            var q1 = categoryName;
            var q2 = categoryName.StartsWith("ال") ? categoryName[2..] : categoryName;

            var items = await _uow.Books.GetQueryable(
                x => x.IsActive && x.Categories.Any(c =>
                    EF.Functions.Like(c.Name, $"%{q1}%") ||
                    EF.Functions.Like(c.Name, $"%{q2}%") ||
                    EF.Functions.Like("ال" + c.Name, $"%{q1}%")),
                q => q.Include(b => b.Author).Include(b => b.Categories))
            .OrderByDescending(b => b.SalesCount)
            .ThenByDescending(b => b.ViewCount)
            .Take(take)
            .ToListAsync();

            return Success(items.Adapt<List<BookBriefDto>>());
        }

        public async Task<ApiResponse<List<BookBriefDto>>> GetRecommendationsAsync(string query)
        {
            var vecHits = await _emb.SearchSimilarBooksAsync(query, 10);
            if (vecHits?.Any() == true)
            {
                var books = vecHits.Where(e => e.Book != null)
                                   .Select(e => e.Book!)
                                   .GroupBy(b => b.Id).Select(g => g.First())
                                   .Take(10).ToList();
                return Success(books.Adapt<List<BookBriefDto>>());
            }

            var items = await _uow.Books.GetQueryable(
                    x => x.IsActive && (((x.Title ?? "")).Contains(query) || ((x.Description ?? "")).Contains(query)),
                    q => q.Include(b => b.Author))
                .OrderByDescending(b => b.SalesCount).ThenByDescending(b => b.ViewCount)
                .Take(10)
                .ToListAsync();

            return Success(items.Adapt<List<BookBriefDto>>());
        }

        // ====== Helpers ======
        private static string? NormalizeRouterIntent(string? x)
        {
            if (string.IsNullOrWhiteSpace(x)) return null;
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "summary","availability","price","author_bio",
                "more_by_author","category_recs","similar_to_title",
                "publisher_info","publisher_books","general_recs"
            };
            return allowed.Contains(x) ? x : null;
        }

        private static string? SanitizeCategory(string? cat)
        {
            if (string.IsNullOrWhiteSpace(cat)) return null;
            var v = cat.Trim().Trim('"', '“', '”');
            var bad = new[] { "string", "undefined", "null", "-" };
            return bad.Contains(v, StringComparer.OrdinalIgnoreCase) ? null : v;
        }
        private async Task<Domain.Entites.Models.Book?> FindBookByTitleAsync(string rawTitle)
        {
            if (string.IsNullOrWhiteSpace(rawTitle)) return null;

            var coreOriginal = rawTitle.Trim();
            var lower = coreOriginal.ToLower();
            var lowerAlt = lower.StartsWith("ال") ? lower[2..] : lower;

            IQueryable<Domain.Entites.Models.Book> baseQuery =
                _uow.Books.GetQueryable(b => b.IsActive && b.Title != null, q => q.Include(b => b.Author).Include(b => b.Publisher));

            // تطابق دقيق بعد ToLower
            var exactDb = await baseQuery
                .Where(b => ((b.Title ?? "").ToLower()) == lower || ((b.Title ?? "").ToLower()) == lowerAlt)
                .OrderByDescending(b => b.SalesCount).ThenByDescending(b => b.ViewCount)
                .FirstOrDefaultAsync();
            if (exactDb != null) return exactDb;

            // مرشّحين بـ LIKE
            var candidates = await baseQuery
                .Where(b => EF.Functions.Like(b.Title!, $"%{coreOriginal}%") ||
                            EF.Functions.Like(b.Title!, $"%{lowerAlt}%"))
                .OrderByDescending(b => b.SalesCount).ThenByDescending(b => b.ViewCount)
                .Take(80)
                .AsNoTracking()
                .ToListAsync();

            // تطبيع ومطابقة دقيقة بعد الجلب
            static string Norm(string s)
                => s.Trim().ToLowerInvariant()
                    .Replace("ـ", "")
                    .Replace('أ', 'ا').Replace('إ', 'ا').Replace('آ', 'ا')
                    .Replace('ى', 'ي').Replace('ؤ', 'و').Replace('ئ', 'ي');

            var coreNorm = Norm(coreOriginal);
            var altNorm = coreNorm.StartsWith("ال") ? coreNorm[2..] : coreNorm;

            var exactish = candidates.FirstOrDefault(b =>
            {
                var tn = Norm(b.Title ?? "");
                return tn == coreNorm || tn == altNorm;
            });
            if (exactish != null) return exactish;

            // اختياري: embeddings بشرط تطابق الاسم بعد التطبيع
            var hit = (await _emb.SearchSimilarBooksAsync(coreOriginal, 1)).FirstOrDefault();
            if (hit?.Book != null)
            {
                var tn = Norm(hit.Book.Title ?? "");
                if (tn == coreNorm || tn == altNorm) return hit.Book;
            }

            return null;
        }

        private async Task<string?> ResolveAuthorByTitleAsync(string title)
        {
            var b = await FindBookByTitleAsync(title);
            return b?.Author?.Name;
        }

        private async Task<RagAskResponse> GetAvailabilityAnswerAsync(string rawTitle, bool includePrice)
        {
            var match = await FindBookByTitleAsync(rawTitle);
            if (match == null)
            {
                var msgAr = $"ملقتش كتاب بعنوان: «{rawTitle}». تأكّد من كتابة الاسم صح أو جرّب صيغة تانية (مثال: من غير «ال» في البداية).";
                var msgEn = $"I couldn’t find a book titled “{rawTitle}”. Please double-check the spelling or try a slightly different title.";
                var lang = LangUtils.Detect(rawTitle);
                return new RagAskResponse { Answer = lang == Lang.English ? msgEn : msgAr, IsAvailable = false };
            }

            bool available = match.StockQuantity > 0;
            
            var price = (match.Price - (match.Price * match.DiscountPercentage / 100m));

            var ansAr = available
                ? $"\"{match.Title}\" متاح للشراء الآن{(includePrice ? $" — السعر: {price:F2}" : "")}."
                : $"\"{match.Title}\" غير متاح حاليًا{(includePrice ? $" — آخر سعر معروف: {price:F2}" : "")}.";

            var ansEn = available
                ? $"“{match.Title}” is available now{(includePrice ? $" — Price: {price:F2}" : "")}."
                : $"“{match.Title}” is currently unavailable{(includePrice ? $" — Last known price: {price:F2}" : "")}.";

            var lang2 = LangUtils.Detect(rawTitle);
            return new RagAskResponse
            {
                Answer = lang2 == Lang.English ? ansEn : ansAr,
                IsAvailable = available,
                PrimaryBookId = match.Id,
                PrimaryBookTitle = match.Title,
                CanAddToCart = available,
                Sources = ToSources(new List<BookBriefDto> {
                    new(match.Id, match.Title, match.Author?.Name, match.Price, match.DiscountedPrice, match.CoverImageUrl, match.Description)
                }, rawTitle)
            };
        }

        private async Task<List<BookBriefDto>> SimilarByTitleAsync(string title, int take = 8)
        {
            var hits = await _emb.SearchSimilarBooksAsync($"العنوان: {title}", take);
            var books = hits.Where(h => h.Book != null).Select(h => h.Book!)
                            .GroupBy(b => b.Id).Select(g => g.First())
                            .Take(take).ToList();
            return books.Adapt<List<BookBriefDto>>();
        }

        private static List<ChatSource> ToSources(List<BookBriefDto> books, string? question = null) =>
            books.Select(b => new ChatSource
            {
                BookId = b.Id,
                Title = b.Title,
                CoverImageUrl = b.CoverImageUrl,
                Snippet = BuildSnippet(b.Description, question ?? "")
            }).ToList();

        private static string? BuildSnippet(string? description, string question)
        {
            if (string.IsNullOrWhiteSpace(description)) return null;
            return description.Length > 240 ? description[..240] + "..." : description;
        }

        // ====== Helpers: ExtractCategory ======
        private static bool IsGreeting(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return false;
            var n = q.Trim().ToLowerInvariant();
            return new[]
            {
                "hi","hello","hey","good morning","good evening","salam","salām",
                "اهلا","أهلًا","مرحبا","مرحباً","السلام عليكم","ازيك","ازاى","هاي","هلا"
            }.Any(w => n.Contains(w));
        }

        private static string Intro(Lang lang) => lang == Lang.English
            ? "Hi! I'm Aseer Alkotb assistant 🤖. I can help you with book summaries, availability, prices, author bios, publisher info, and recommendations."
            : "أهلاً! أنا مساعد عصير الكتب 🤖. أقدر أساعدك في ملخصات الكتب، التوافر، الأسعار، نبذات المؤلفين، معلومات دور النشر، والترشيحات.";

        // ====== Publisher Methods ======
        private async Task<RagAskResponse> HandlePublisherInfoAsync(string? title, string? publisher, string question)
        {
            var isEnglishQuery = await _translator.IsEnglishTextAsync(question);
            
            Publisher? publisherEntity = null;
            
            // If we have a title, find its publisher
            if (!string.IsNullOrWhiteSpace(title))
            {
                var book = await FindBookByTitleAsync(title);
                if (book?.Publisher != null)
                {
                    var answer = $"\"{book.Title}\" منشور من دار {book.Publisher.Name}.";
                    if (!string.IsNullOrWhiteSpace(book.Publisher.Description))
                        answer += $" نبذة عن دار النشر: {book.Publisher.Description}";
                    
                    var response = new RagAskResponse 
                    { 
                        Answer = answer,
                        Sources = ToSources(new List<BookBriefDto> {
                            new(book.Id, book.Title, book.Author?.Name, book.Price, book.DiscountedPrice, book.CoverImageUrl, book.Description)
                        }, question)
                    };
                    return await TranslateResponseIfNeededAsync(response, isEnglishQuery);
                }
                var notFoundResponse = new RagAskResponse { Answer = $"لم أجد معلومات عن ناشر كتاب \"{title}\"." };
                return await TranslateResponseIfNeededAsync(notFoundResponse, isEnglishQuery);
            }
            
            // Direct publisher query
            if (!string.IsNullOrWhiteSpace(publisher))
            {
                publisherEntity = await FindPublisherByNameAsync(publisher);
                if (publisherEntity != null)
                {
                    var answer = $"معلومات عن {publisherEntity.Name}:";
                    if (!string.IsNullOrWhiteSpace(publisherEntity.Description))
                        answer += $" {publisherEntity.Description}";
                    else
                        answer += " لا تتوفر نبذة مفصلة حالياً.";
                    
                    var response = new RagAskResponse { Answer = answer };
                    return await TranslateResponseIfNeededAsync(response, isEnglishQuery);
                }
                var notFoundResponse = new RagAskResponse { Answer = $"لم أجد دار نشر بالاسم \"{publisher}\"." };
                return await TranslateResponseIfNeededAsync(notFoundResponse, isEnglishQuery);
            }
            
            var defaultResponse = new RagAskResponse { Answer = "برجاء تحديد اسم دار النشر أو اسم كتاب للاستعلام عن ناشره." };
            return await TranslateResponseIfNeededAsync(defaultResponse, isEnglishQuery);
        }
        
        private async Task<RagAskResponse> HandlePublisherBooksAsync(string? publisher, int limit, string question)
        {
            var isEnglishQuery = await _translator.IsEnglishTextAsync(question);
            
            if (string.IsNullOrWhiteSpace(publisher))
            {
                var response = new RagAskResponse { Answer = "برجاء تحديد اسم دار النشر للعثور على كتبها." };
                return await TranslateResponseIfNeededAsync(response, isEnglishQuery);
            }
                
            var take = limit > 0 ? limit : 10;
            var books = await GetPublisherBooksAsync(publisher, take);
            
            if (!books.Data!.Any())
            {
                var response = new RagAskResponse { Answer = $"لم أجد كتباً لدار النشر \"{publisher}\"." };
                return await TranslateResponseIfNeededAsync(response, isEnglishQuery);
            }
                
            var titles = books.Data!.Select(b => b.Title).ToList();
            var answer = $"كتب من {publisher}: {string.Join("، ", titles)}.";
            
            var finalResponse = new RagAskResponse 
            {
                Answer = answer,
                Sources = ToSources(books.Data!, question)
            };
            return await TranslateResponseIfNeededAsync(finalResponse, isEnglishQuery);
        }
        
        private async Task<Publisher?> FindPublisherByNameAsync(string publisherName)
        {
            if (string.IsNullOrWhiteSpace(publisherName)) return null;
            
            var lower = publisherName.Trim().ToLower();
            var lowerAlt = lower.StartsWith("ال") ? lower[2..] : lower;
            
            return await _uow.Publishers.GetQueryable(
                p => EF.Functions.Like(p.Name.ToLower(), $"%{lower}%") ||
                     EF.Functions.Like(p.Name.ToLower(), $"%{lowerAlt}%"))
                .FirstOrDefaultAsync();
        }
        
        private async Task<ApiResponse<List<BookBriefDto>>> GetPublisherBooksAsync(string publisherName, int take = 10)
        {
            var publisher = await FindPublisherByNameAsync(publisherName);
            if (publisher == null)
                return Success(new List<BookBriefDto>());
                
            var books = await _uow.Books.GetQueryable(
                    b => b.IsActive && b.PublisherId == publisher.Id,
                    q => q.Include(b => b.Author).Include(b => b.Publisher))
                .OrderByDescending(b => b.SalesCount)
                .ThenByDescending(b => b.ViewCount)
                .Take(take)
                .ToListAsync();
                
            return Success(books.Adapt<List<BookBriefDto>>());
        }
    }
}
