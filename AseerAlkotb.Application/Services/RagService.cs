using System.Text.RegularExpressions;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Rag.Requests;
using AseerAlkotb.Application.Features.Rag.Responses;
using AseerAlkotb.Application.Features.Rag.Models;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Application.Utils;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        private readonly ISessionMemoryService _sessionMemory;
        private readonly IConfiguration _cfg;
        private readonly ILogger<RagService> _logger;

        public RagService(
            IUnitOfWork uow,
            IEmbeddingService emb,
            IAnswerSynthesisService synth,
            IQuestionRouterService router,
            ITranslationService translator,
            ISessionMemoryService sessionMemory,
            IServiceProvider sp,
            Microsoft.Extensions.Hosting.IHostEnvironment env
        ) : base(sp, env)
        {
            _uow = uow; _emb = emb; _synth = synth; _router = router; _translator = translator;
            _sessionMemory = sessionMemory;
            _cfg = sp.GetRequiredService<IConfiguration>();
            _logger = sp.GetRequiredService<ILogger<RagService>>();
        }

        public async Task<ApiResponse<RagAskResponse>> AskAsync(RagAskRequest request)
        {
            return await AskWithSessionAsync(request, sessionId: null);
        }

        public async Task<ApiResponse<RagAskResponse>> AskWithSessionAsync(RagAskRequest request, string? sessionId)
        {
            await DoValidationAsync<Application.Features.Rag.Validators.RagAskRequestValidator, RagAskRequest>(request);
            
            var originalQuestion = request.Question;
            var isEnglishQuery = await _translator.IsEnglishTextAsync(originalQuestion);
            
            // Generate session ID if not provided
            sessionId ??= Guid.NewGuid().ToString();
            
            // Check for previous similar questions in session
            var similarQuestions = await _sessionMemory.FindSimilarQuestionsAsync(sessionId, originalQuestion);
            var conversationContext = await _sessionMemory.GetConversationContextAsync(sessionId, originalQuestion);
            
            // Translate English questions to Arabic for database search
            var processedQuestion = isEnglishQuery 
                ? await _translator.TranslateToArabicAsync(originalQuestion)
                : originalQuestion;
                
            var lang = LangUtils.Detect(originalQuestion); // Keep original language for response
            
            if (IsGreeting(originalQuestion))
            {
                var greetingResponse = new RagAskResponse
                {
                    Answer = Intro(lang)
                };
                
                // Save greeting to session
                await SaveMessageToSessionAsync(sessionId, originalQuestion, greetingResponse.Answer, "greeting", null, null, null, null, isEnglishQuery);
                
                return Success(greetingResponse);
            }

            // Handle repeated questions
            if (similarQuestions.Any())
            {
                var mostSimilar = similarQuestions.First();
                var timeDiff = DateTime.UtcNow - mostSimilar.Timestamp;
                
                // If asked same question within last 10 minutes, provide context-aware response
                if (timeDiff.TotalMinutes < 10)
                {
                    var contextualAnswer = lang == Lang.English
                        ? $"I remember you asked about this recently. {mostSimilar.Answer}"
                        : $"أتذكر إنك سألت عن هذا مؤخراً. {mostSimilar.Answer}";
                    
                    var contextualResponse = new RagAskResponse
                    {
                        Answer = contextualAnswer
                    };
                    
                    // Don't save repeated question, just update last accessed
                    return Success(await TranslateResponseIfNeededAsync(contextualResponse, isEnglishQuery));
                }
            }

            // 1) Use Gemini for intent detection and entity extraction on processed question
            var route = await _router.RouteAsync(processedQuestion);
            
            // 2) Get intent first
            string intent = NormalizeRouterIntent(route.intent) ?? "general_recs";
            
            // 2.1) Special case: If no specific intent but we have cached author and question about books
            if (intent == "general_recs" && !string.IsNullOrWhiteSpace(await _sessionMemory.GetCachedAuthorAsync(sessionId)))
            {
                var lowerQuestion = processedQuestion.ToLower();
                if (lowerQuestion.Contains("كتب") || lowerQuestion.Contains("مؤلفات") || 
                    lowerQuestion.Contains("أعمال") || lowerQuestion.Contains("روايات") ||
                    lowerQuestion.Contains("أشهر") || lowerQuestion.Contains("books") ||
                    lowerQuestion.Contains("works") || lowerQuestion.Contains("novels"))
                {
                    intent = "more_by_author";
                    _logger.LogInformation("Detected 'more_by_author' intent from cached context for session {SessionId}", sessionId);
                }
            }
            
            // 3) Trust Gemini completely - no local fallbacks or guessing
            string? title = route.entities.title;
            string? author = route.entities.author;
            string? category = SanitizeCategory(request.Category) ?? route.entities.category;
            string? publisher = route.entities.publisher;
            
            // 4) If entities are missing, try to get them from session cache
            if (string.IsNullOrWhiteSpace(title))
                title = await _sessionMemory.GetCachedTitleAsync(sessionId);
            
            if (string.IsNullOrWhiteSpace(author))
                author = await _sessionMemory.GetCachedAuthorAsync(sessionId);
                
            if (string.IsNullOrWhiteSpace(category))
                category = await _sessionMemory.GetCachedCategoryAsync(sessionId);
                
            if (string.IsNullOrWhiteSpace(publisher))
                publisher = await _sessionMemory.GetCachedPublisherAsync(sessionId);
            
            // Log for debugging
            _logger.LogInformation("Session {SessionId}: Intent={Intent}, Author={Author}, Title={Title}, CachedAuthor={CachedAuthor}", 
                sessionId, intent, route.entities.author, route.entities.title, author);

            switch (intent)
            {
                case "summary":
                    {
                        if (string.IsNullOrWhiteSpace(title))
                        {
                            var response = new RagAskResponse
                            {
                                Answer = "برجاء تحديد اسم الكتاب للحصول على ملخص."
                            };
                            return await RespondAndRememberAsync(sessionId, originalQuestion, response,
                                intent, title, author, category, publisher, isEnglishQuery);
                        }

                        // جرّب نجيب الكتاب من العنوان
                        var book = await FindBookByTitleAsync(title);
                        if (book == null)
                        {
                            var response = new RagAskResponse
                            {
                                Answer = $"لم أجد ملخصًا للكتاب «{title}»."
                            };
                            return await RespondAndRememberAsync(sessionId, originalQuestion, response,
                                intent, title, author, category, publisher, isEnglishQuery);
                        }

                        // لو مفعّل إننا نستخدم الوصف كما هو (من الإعدادات)
                        bool preferDesc = string.Equals(_cfg["Rag:SummarizeFromDescription"], "true", StringComparison.OrdinalIgnoreCase);
                        if (preferDesc && !string.IsNullOrWhiteSpace(book.Description) && book.Description!.Length >= 160)
                        {
                            var fromDesc = new RagAskResponse
                            {
                                Answer = book.Description!,
                                Sources = ToSources(new List<BookBriefDto> {
                new BookBriefDto(
                    book.Id,
                    book.Title,
                    book.Author?.Name,
                    book.Price,
                    book.DiscountedPrice,
                    book.CoverImageUrl,
                    book.Description)
            }, originalQuestion),
                                PrimaryBookId = book.Id,
                                PrimaryBookTitle = book.Title
                            };

                            return await RespondAndRememberAsync(sessionId, originalQuestion, fromDesc,
                                intent, title, author, category, publisher, isEnglishQuery);
                        }

                        // تجهيز مصدر وسياق للمُلخّص المُولّد (بدون AuthorName)
                        var src = new ChatSource
                        {
                            BookId = book.Id,
                            Title = book.Title,
                            CoverImageUrl = book.CoverImageUrl,
                            Snippet = book.Description
                        };

                        string prompt = !string.IsNullOrWhiteSpace(book.Description) && book.Description!.Trim().Length >= 120
                            ? $"لخّص كتاب \"{book.Title}\" للمؤلف {book.Author?.Name} في 3–5 نقاط بالاعتماد على الوصف أدناه."
                            : $"أعطني ملخصًا موجزًا وواضحًا لكتاب \"{book.Title}\" للمؤلف {book.Author?.Name}.";

                        if (!string.IsNullOrEmpty(conversationContext))
                        {
                            prompt += $"\n\n{conversationContext}";
                        }

                        var summary = await _synth.SynthesizeAsync(prompt, new List<ChatSource> { src });
                        var answer = string.IsNullOrWhiteSpace(summary)
                            ? (book.Description?.Trim().Length >= 120
                                ? book.Description!
                                : "لا تتوفر لدينا نبذة كافية لهذا الكتاب حاليًا.")
                            : summary;

                        var resp = new RagAskResponse
                        {
                            Answer = answer,
                            Sources = new List<ChatSource> { src },
                            PrimaryBookId = book.Id,
                            PrimaryBookTitle = book.Title
                        };

                        return await RespondAndRememberAsync(sessionId, originalQuestion, resp,
                            intent, title, author, category, publisher, isEnglishQuery);
                    }


                case "availability":
                    {
                        if (string.IsNullOrWhiteSpace(title))
                        {
                            var response = new RagAskResponse
                            {
                                Answer = "برجاء تحديد اسم الكتاب للاستعلام عن توافره."
                            };
                            return await RespondAndRememberAsync(sessionId, originalQuestion, response, intent, title, author, category, publisher, isEnglishQuery);
                        }

                        var r = await GetAvailabilityAnswerAsync(title, includePrice: false);
                        return await RespondAndRememberAsync(sessionId, originalQuestion, r, intent, title, author, category, publisher, isEnglishQuery);
                    }

                // --- price ---
                case "price":
                    {
                        if (string.IsNullOrWhiteSpace(title))
                        {
                            var response = new RagAskResponse
                            {
                                Answer = "برجاء تحديد اسم الكتاب للاستعلام عن سعره."
                            };
                            return await RespondAndRememberAsync(sessionId, originalQuestion, response, intent, title, author, category, publisher, isEnglishQuery);
                        }

                        var r = await GetAvailabilityAnswerAsync(title, includePrice: true);
                        return await RespondAndRememberAsync(sessionId, originalQuestion, r, intent, title, author, category, publisher, isEnglishQuery);
                    }

                // --- author_bio ---
                // (also answers “مين اللي كاتب …؟ / who wrote … ?” if a title is present)
                case "author_bio":
                    {
                        // Try to resolve author if only a title is known
                        string? authorName = author ?? (title != null ? await ResolveAuthorByTitleAsync(title) : null);

                        if (string.IsNullOrWhiteSpace(authorName))
                        {
                            var response = new RagAskResponse
                            {
                                Answer = "برجاء تحديد اسم المؤلف أو اسم كتاب له للحصول على نبذة عنه."
                            };
                            return await RespondAndRememberAsync(sessionId, originalQuestion, response, intent, title, author, category, publisher, isEnglishQuery);
                        }

                        var authorRow = await _uow.Authors
                            .GetQueryable(x => ((x.Name ?? string.Empty).ToLower()).Contains(authorName.ToLower()))
                            .Include(x => x.Books)
                            .FirstOrDefaultAsync();

                        if (authorRow == null)
                        {
                            var response = new RagAskResponse { Answer = $"لم أجد مؤلف بالاسم: {authorName}" };
                            // Save with the resolved authorName so it’s cached
                            return await RespondAndRememberAsync(sessionId, originalQuestion, response, intent, title, authorName, category, publisher, isEnglishQuery);
                        }

                        // Detect “who wrote / مين اللي كاتب / من مؤلف”
                        var isWhoWrote = Regex.IsMatch(processedQuestion ?? string.Empty,
                            pattern: "(مين(\\s)?(ال)?لي\\s*كاتب|من\\s*مؤلف|who\\s*wrote)",
                            options: RegexOptions.IgnoreCase);

                        string? bio = authorRow.Bio;
                        if (string.IsNullOrWhiteSpace(bio) || bio.Trim().Length < 80)
                        {
                            bio = await _synth.SynthesizeAsync(
                                      $"اكتب نبذة قصيرة وواضحة عن المؤلف {authorRow.Name}.",
                                      new List<ChatSource>())
                                  ?? $"لا تتوفر لدينا نبذة كافية عن {authorRow.Name} حاليًا.";
                        }

                        var answer = (isWhoWrote && !string.IsNullOrWhiteSpace(title))
                            ? $"مؤلف «{title}» هو: {authorRow.Name}.\n{bio}"
                            : bio;

                        var finalResponse = new RagAskResponse { Answer = answer };
                        return await RespondAndRememberAsync(sessionId, originalQuestion, finalResponse, intent, title, authorRow.Name, category, publisher, isEnglishQuery);
                    }

                // --- category_recs ---
                case "category_recs":
                    {
                        if (string.IsNullOrWhiteSpace(category))
                        {
                            var response = new RagAskResponse
                            {
                                Answer = "برجاء تحديد التصنيف المطلوب (مثال: روايات، تطوير ذات، تاريخ)."
                            };
                            return await RespondAndRememberAsync(sessionId, originalQuestion, response, intent, title, author, category, publisher, isEnglishQuery);
                        }

                        var take = request.Limit > 0 ? request.Limit : 8;
                        var list = await GetCategoryBooksAsync(category, take);
                        var dedup = list.Data!
                            .GroupBy(b => (b.Title ?? string.Empty).Trim())
                            .Select(g => g.First())
                            .Take(take)
                            .ToList();

                        var ans = dedup.Any()
                            ? $"ترشيحات ضمن «{category}»: {string.Join("، ", dedup.Select(b => b.Title))}."
                            : $"لا توجد نتائج ضمن التصنيف «{category}».";
                        var resp = new RagAskResponse { Answer = ans, Sources = ToSources(dedup, originalQuestion) };

                        return await RespondAndRememberAsync(sessionId, originalQuestion, resp, intent, title, author, category, publisher, isEnglishQuery);
                    }

                // --- similar_to_title ---
                case "similar_to_title":
                    {
                        if (string.IsNullOrWhiteSpace(title))
                        {
                            var response = new RagAskResponse
                            {
                                Answer = "برجاء تحديد اسم الكتاب للبحث عن كتب مشابهة له."
                            };
                            return await RespondAndRememberAsync(sessionId, originalQuestion, response, intent, title, author, category, publisher, isEnglishQuery);
                        }

                        var take = request.Limit > 0 ? request.Limit : 8;
                        var recs = await SimilarByTitleAsync(title, take);

                        var ans = recs.Any()
                            ? $"كتب تشبه «{title}»: {string.Join("، ", recs.Select(b => b.Title))}."
                            : $"لم أجد ترشيحات قريبة من «{title}».";
                        var resp = new RagAskResponse { Answer = ans, Sources = ToSources(recs, originalQuestion) };

                        return await RespondAndRememberAsync(sessionId, originalQuestion, resp, intent, title, author, category, publisher, isEnglishQuery);
                    }

                // --- publisher_info ---
                case "publisher_info":
                    {
                        var r = await HandlePublisherInfoAsync(title, publisher, request.Question);
                        // r هو RagAskResponse بالفعل
                        await SaveMessageToSessionAsync(sessionId, originalQuestion, r.Answer, intent, title, author, category, publisher, isEnglishQuery);
                        return Success(r);
                    }

                // --- publisher_books ---
                case "publisher_books":
                    {
                        var r = await HandlePublisherBooksAsync(publisher, request.Limit, request.Question);
                        await SaveMessageToSessionAsync(sessionId, originalQuestion, r.Answer, intent, title, author, category, publisher, isEnglishQuery);
                        return Success(r);
                    }

                default:
                    {
                        var top = await GetRecommendationsAsync(processedQuestion);
                        var reply = top.Data!.Any()
                            ? $"لم أجد نتائج مطابقة تمامًا لسؤالك، لكن بناءً على ما بحثت عنه أرشح لك: {string.Join("، ", top.Data!.Select(x => x.Title))}."
                            : "لم أجد كتباً متعلقة مباشرة بسؤالك. جرّب كلمات مفتاحية مختلفة أو تصنيفاً آخر.";
                        
                        var defaultResponse = new RagAskResponse { Answer = reply, Sources = ToSources(top.Data ?? new List<BookBriefDto>(), originalQuestion) };
                        await SaveMessageToSessionAsync(sessionId, originalQuestion, defaultResponse.Answer, "general_recs", title, author, category, publisher, isEnglishQuery);
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
        
        // ====== Session Memory Helper ======
        private async Task SaveMessageToSessionAsync(
            string sessionId, 
            string question, 
            string answer, 
            string? intent, 
            string? title, 
            string? author, 
            string? category, 
            string? publisher, 
            bool isEnglishQuery)
        {
            var sessionMessage = new SessionMessage
            {
                Question = question,
                Answer = answer,
                Intent = intent,
                ExtractedTitle = title,
                ExtractedAuthor = author,
                ExtractedCategory = category,
                ExtractedPublisher = publisher,
                IsEnglishQuery = isEnglishQuery,
                Timestamp = DateTime.UtcNow
            };
            
            // Try to resolve entity IDs for caching
            int? resolvedBookId = null;
            int? resolvedAuthorId = null;
            int? resolvedPublisherId = null;
            int? resolvedCategoryId = null;
            
            try
            {
                // Resolve book ID if title is available
                if (!string.IsNullOrWhiteSpace(title))
                {
                    var book = await FindBookByTitleAsync(title);
                    if (book != null)
                    {
                        resolvedBookId = book.Id;
                        // Also capture author and publisher from the book
                        if (book.Author != null && string.IsNullOrWhiteSpace(author))
                        {
                            author = book.Author.Name;
                            resolvedAuthorId = book.Author.Id;
                        }
                        if (book.Publisher != null && string.IsNullOrWhiteSpace(publisher))
                        {
                            publisher = book.Publisher.Name;
                            resolvedPublisherId = book.Publisher.Id;
                        }
                    }
                }
                
                // Resolve author ID if author name is available
                if (!string.IsNullOrWhiteSpace(author) && !resolvedAuthorId.HasValue)
                {
                    var authorEntity = await _uow.Authors.GetQueryable(a => 
                        EF.Functions.Like(a.Name.ToLower(), $"%{author.ToLower()}%"))
                        .FirstOrDefaultAsync();
                    if (authorEntity != null)
                        resolvedAuthorId = authorEntity.Id;
                }
                
                // Resolve publisher ID if publisher name is available
                if (!string.IsNullOrWhiteSpace(publisher) && !resolvedPublisherId.HasValue)
                {
                    var publisherEntity = await FindPublisherByNameAsync(publisher);
                    if (publisherEntity != null)
                        resolvedPublisherId = publisherEntity.Id;
                }
                
                // Resolve category ID if category name is available
                if (!string.IsNullOrWhiteSpace(category))
                {
                    var categoryEntity = await _uow.Categories.GetQueryable(c => 
                        EF.Functions.Like(c.Name.ToLower(), $"%{category.ToLower()}%"))
                        .FirstOrDefaultAsync();
                    if (categoryEntity != null)
                        resolvedCategoryId = categoryEntity.Id;
                }
            }
            catch (Exception ex)
            {
                // Don't fail the main operation if entity resolution fails
                _logger.LogWarning(ex, "Failed to resolve entity IDs for session {SessionId}", sessionId);
            }
            
            await _sessionMemory.AddMessageWithEntitiesAsync(
                sessionId, 
                sessionMessage,
                resolvedBookId, 
                resolvedAuthorId, 
                resolvedPublisherId, 
                resolvedCategoryId,
                NormalizeEntityName(title),
                NormalizeEntityName(author),
                NormalizeEntityName(publisher),
                NormalizeEntityName(category)
            );
        }
        
        private static string? NormalizeEntityName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            
            return name.Trim().ToLowerInvariant()
                .Replace("ـ", "")
                .Replace('أ', 'ا').Replace('إ', 'ا').Replace('آ', 'ا')
                .Replace('ى', 'ي').Replace('ؤ', 'و').Replace('ئ', 'ي');
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

        private async Task<ApiResponse<RagAskResponse>> RespondAndRememberAsync(
    string sessionId,
    string originalQuestion,
    RagAskResponse response,
    string intent,
    string? title,
    string? author,
    string? category,
    string? publisher,
    bool isEnglishQuery)
        {
            await SaveMessageToSessionAsync(
                sessionId,
                originalQuestion,   // question
                response.Answer,    // answer
                intent,
                title,
                author,
                category,
                publisher,
                isEnglishQuery
            );

            var translated = await TranslateResponseIfNeededAsync(response, isEnglishQuery);
            return Success(translated);
        }




    }
}
