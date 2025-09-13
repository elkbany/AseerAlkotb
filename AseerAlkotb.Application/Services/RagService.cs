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
        private readonly IConfiguration _cfg;

        public RagService(
            IUnitOfWork uow,
            IEmbeddingService emb,
            IAnswerSynthesisService synth,
            IQuestionRouterService router,
            IServiceProvider sp,
            Microsoft.Extensions.Hosting.IHostEnvironment env
        ) : base(sp, env)
        {
            _uow = uow; _emb = emb; _synth = synth; _router = router;
            _cfg = sp.GetRequiredService<IConfiguration>();
        }

        public async Task<ApiResponse<RagAskResponse>> AskAsync(RagAskRequest request)
        {
            await DoValidationAsync<Application.Features.Rag.Validators.RagAskRequestValidator, RagAskRequest>(request);
            var lang = LangUtils.Detect(request.Question);
            if (IsGreeting(request.Question))
            {
                return Success(new RagAskResponse
                {
                    Answer = Intro(lang)
                });
            }
            // 1) استخرج Hints محليًا (عنوان/مؤلف)
            var (titleHint, authorHint) = QueryExtractor.Extract(request.Question);

            // 2) ابنِ نسخة موجّهة للـ Router فيها الـ Hints بشكل صريح
            var routerInput = BuildRouterInput(request.Question, titleHint, authorHint);

            // 3) اسأل Gemini دائمًا أولًا عن النية والكيانات
            var route = await _router.RouteAsync(routerInput);
            var _ = route.confidence < 0.55; // للعلم فقط

            // 4) دمج الكيانات: Gemini أولاً ثم fallback محلي للـ title فقط
            string? title = route.entities.title ?? titleHint ?? ExtractTitleLoose(request.Question);
            string? author = route.entities.author ?? authorHint;
            string? category = SanitizeCategory(request.Category) ?? route.entities.category;

            // 5) النية: اعتماد كامل على Gemini (بدون أي تخمين محلي)
            static string? NormalizeRouterIntent(string? x)
            {
                if (string.IsNullOrWhiteSpace(x)) return null;
                var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "summary","availability","price","author_bio",
                    "more_by_author","category_recs","similar_to_title","general_recs"
                };
                return allowed.Contains(x) ? x : null;
            }
            string intent = NormalizeRouterIntent(route.intent) ?? "general_recs";

            switch (intent)
            {
                case "summary":
                    {
                        var t = title ?? request.Question;
                        var book = await FindBookByTitleAsync(t);
                        if (book != null)
                        {
                            bool preferDesc = string.Equals(_cfg["Rag:SummarizeFromDescription"], "true", StringComparison.OrdinalIgnoreCase);
                            if (preferDesc && !string.IsNullOrWhiteSpace(book.Description) && book.Description!.Length >= 160)
                            {
                                return Success(new RagAskResponse
                                {
                                    Answer = book.Description!,
                                    Sources = ToSources(new List<BookBriefDto> {
                                        new BookBriefDto(book.Id, book.Title, book.Author?.Name, book.Price, book.DiscountedPrice, book.CoverImageUrl, book.Description)
                                    }, request.Question),
                                    PrimaryBookId = book.Id,
                                    PrimaryBookTitle = book.Title
                                });
                            }

                            var src = new ChatSource { BookId = book.Id, Title = book.Title, CoverImageUrl = book.CoverImageUrl, Snippet = book.Description };
                            string prompt = !string.IsNullOrWhiteSpace(book.Description) && book.Description!.Trim().Length >= 120
                                ? $"لخّص كتاب \"{book.Title}\" للمؤلف {book.Author?.Name} في 3–5 نقاط بالاعتماد على الوصف أدناه."
                                : $"أعطني ملخصًا موجزًا وواضحًا لكتاب \"{book.Title}\" للمؤلف {book.Author?.Name}.";

                            var summary = await _synth.SynthesizeAsync(prompt, new List<ChatSource> { src });
                            var answer = string.IsNullOrWhiteSpace(summary) ? (book.Description ?? "لا تتوفر لدينا نبذة كافية لهذا الكتاب حاليًا.") : summary;

                            return Success(new RagAskResponse
                            {
                                Answer = answer,
                                Sources = new List<ChatSource> { src },
                                PrimaryBookId = book.Id,
                                PrimaryBookTitle = book.Title
                            });
                        }

                        var gen = await _synth.SynthesizeAsync(request.Question, new List<ChatSource>());
                        return Success(new RagAskResponse { Answer = string.IsNullOrWhiteSpace(gen) ? $"ملقتش كتاب بعنوان «{t}»." : gen });
                    }

                case "availability":
                    {
                        var t = title ?? request.Question;
                        var r = await GetAvailabilityAnswerAsync(t, includePrice: false);
                        return Success(r);
                    }

                case "price":
                    {
                        var t = title ?? request.Question;
                        var r = await GetAvailabilityAnswerAsync(t, includePrice: true);
                        return Success(r);
                    }

                case "author_bio":
                    {
                        var a = author ?? (title != null ? await ResolveAuthorByTitleAsync(title) : null);
                        if (string.IsNullOrWhiteSpace(a))
                            return Success(new RagAskResponse { Answer = "اكتب اسم المؤلف أو اسم كتاب له علشان أجيب لك نبذة عنه." });

                        var authorRow = await _uow.Authors.GetQueryable(x => ((x.Name ?? "").ToLower()).Contains(a.ToLower()))
                                                          .Include(x => x.Books)
                                                          .FirstOrDefaultAsync();
                        if (authorRow == null)
                            return Success(new RagAskResponse { Answer = $"مش لاقي مؤلف بالاسم: {a}" });

                        string? bio = authorRow.Bio;
                        if (string.IsNullOrWhiteSpace(bio) || bio.Trim().Length < 80)
                        {
                            bio = await _synth.SynthesizeAsync($"اكتب نبذة قصيرة وواضحة عن المؤلف {authorRow.Name}.", new List<ChatSource>())
                                  ?? $"لا تتوفر لدينا نبذة كافية عن {authorRow.Name} حاليًا.";
                        }

                        return Success(new RagAskResponse { Answer = bio });
                    }

                case "more_by_author":
                    {
                        var a = author ?? (title != null ? await ResolveAuthorByTitleAsync(title) : null);
                        if (string.IsNullOrWhiteSpace(a))
                            return Success(new RagAskResponse { Answer = "محتاج اسم المؤلف أو اسم كتاب له علشان أرشّح مؤلفات أخرى." });

                        var list = await GetAuthorBooksAsync(a);
                        var names = list.Data!.Select(b => b.Title).ToList();
                        var ans = names.Any()
                            ? $"كتب أخرى لنفس المؤلف: {string.Join("، ", names)}."
                            : "لم أجد كتبًا لنفس المؤلف.";
                        return Success(new RagAskResponse { Answer = ans, Sources = ToSources(list.Data!) });
                    }

                case "category_recs":
                    {
                        var cat = category ?? ExtractCategory(request.Question);
                        if (string.IsNullOrWhiteSpace(cat))
                            return Success(new RagAskResponse { Answer = "اذكر اسم التصنيف (مثال: روايات/تطوير ذات)." });

                        var take = request.Limit > 0 ? request.Limit : 8;
                        var list = await GetCategoryBooksAsync(cat, take);
                        var dedup = list.Data!.GroupBy(b => b.Title.Trim()).Select(g => g.First()).Take(take).ToList();

                        var ans = dedup.Any()
                            ? $"ترشيحات ضمن «{cat}»: {string.Join("، ", dedup.Select(b => b.Title))}."
                            : $"لا توجد نتائج ضمن التصنيف «{cat}».";
                        return Success(new RagAskResponse { Answer = ans, Sources = ToSources(dedup, request.Question) });
                    }

                case "similar_to_title":
                    {
                        if (string.IsNullOrWhiteSpace(title))
                            return Success(new RagAskResponse { Answer = "اذكر اسم الكتاب اللي عايز ترشيحات شبهه." });

                        var take = request.Limit > 0 ? request.Limit : 8;
                        var recs = await SimilarByTitleAsync(title, take);
                        var ans = recs.Any()
                            ? $"كتب تشبه «{title}»: {string.Join("، ", recs.Select(b => b.Title))}."
                            : $"ملقتش ترشيحات قريبة من «{title}».";
                        return Success(new RagAskResponse { Answer = ans, Sources = ToSources(recs, request.Question) });
                    }

                default:
                    {
                        var top = await GetRecommendationsAsync(request.Question);
                        var reply = top.Data!.Any()
                            ? $"بناءً على سؤالك، أنصحك بالاطلاع على: {string.Join("، ", top.Data!.Select(x => x.Title))}."
                            : "لم أجد كتباً متعلقة مباشرة بسؤالك. جرّب كلمات مفتاحية مختلفة أو تصنيفاً آخر.";
                        return Success(new RagAskResponse { Answer = reply, Sources = ToSources(top.Data ?? new List<BookBriefDto>(), request.Question) });
                    }
            }
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
        private static string BuildRouterInput(string question, string? titleHint, string? authorHint)
        {
            // بنضيف HINTS واضحة للسؤال علشان Gemini يلقط العنوان/المؤلف بسهولة
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(question?.Trim() ?? string.Empty);
            if (!string.IsNullOrWhiteSpace(titleHint)) sb.AppendLine($"[TITLE_HINT]: {titleHint}");
            if (!string.IsNullOrWhiteSpace(authorHint)) sb.AppendLine($"[AUTHOR_HINT]: {authorHint}");
            return sb.ToString();
        }

        // لم نعد نستخدم أي Fallback محلي للنية — Gemini هو المصدر الوحيد
        private static string DetectIntentFallback(string q) => "general_recs";

        private static string? SanitizeCategory(string? cat)
        {
            if (string.IsNullOrWhiteSpace(cat)) return null;
            var v = cat.Trim().Trim('"', '“', '”');
            var bad = new[] { "string", "undefined", "null", "-" };
            return bad.Contains(v, StringComparer.OrdinalIgnoreCase) ? null : v;
        }

        private static string Normalize(string? x)
        {
            if (string.IsNullOrWhiteSpace(x)) return string.Empty;
            var s = x.Trim().ToLowerInvariant();
            s = s.Replace("ـ", "")
                 .Replace('أ', 'ا').Replace('إ', 'ا').Replace('آ', 'ا')
                 .Replace('ى', 'ي').Replace('ؤ', 'و').Replace('ئ', 'ي');
            return s;
        }

        private static string? ExtractTitleLoose(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var s = text.Trim();

            var q = Regex.Match(s, "[\"«»“”'`]{1}(?<t>[^\"«»“”'`]{2,})[\"«»“”'`]{1}");
            if (q.Success) return q.Groups["t"].Value.Trim();

            var m = Regex.Match(s, @"(?:نبذة|نبذه|ملخص)\s+عن\s+(?<t>[^\.،:;!\?؟]{2,})", RegexOptions.IgnoreCase);
            if (m.Success) return m.Groups["t"].Value.Trim();

            m = Regex.Match(s, @"(?:ال)?(?:كتاب|كتب|رواية|الرواية)\s+(?<t>[^\.،:;!\?؟]{2,})", RegexOptions.IgnoreCase);
            if (m.Success) return m.Groups["t"].Value.Trim();

            return null;
        }

        private async Task<Domain.Entites.Models.Book?> FindBookByTitleAsync(string rawTitle)
        {
            if (string.IsNullOrWhiteSpace(rawTitle)) return null;

            var coreOriginal = rawTitle.Trim();
            var lower = coreOriginal.ToLower();
            var lowerAlt = lower.StartsWith("ال") ? lower[2..] : lower;

            IQueryable<Domain.Entites.Models.Book> baseQuery =
                _uow.Books.GetQueryable(b => b.IsActive && b.Title != null, q => q.Include(b => b.Author));

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
            var price = (match.DiscountedPrice > 0m && match.DiscountedPrice < match.Price) ? match.DiscountedPrice : match.Price;

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
        private static string? ExtractCategory(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var q = text.Trim();
            var qLower = q.ToLowerInvariant();
            var markers = new[] { "نوع", "تصنيف", "genre", "category" };

            foreach (var m in markers)
            {
                var idx = qLower.IndexOf(m);
                if (idx >= 0)
                {
                    var start = idx + m.Length;
                    var tail = q.Substring(start).Trim(' ', ':', '：', '-', '—', '،', '"', '«', '»', '“', '”', '\'', '`');
                    var stops = new[] { "،", ",", ";", "؛", ".", ":", "—", "-", "!", "؟", "?" };
                    foreach (var s in stops)
                    {
                        var cut = tail.IndexOf(s, StringComparison.Ordinal);
                        if (cut >= 0) { tail = tail[..cut]; break; }
                    }
                    tail = tail.Trim();
                    if (tail.StartsWith("ال")) tail = tail[2..];
                    return string.IsNullOrWhiteSpace(tail) ? null : tail;
                }
            }

            var likePatterns = new[]
            {
                @"بحب\s+ال?(?:نوع|تصنيف)\s+(?<cat>[^\.,;:!\?؟،\-]{2,})",
                @"i\s+like\s+the\s+(?:genre|category)\s+(?<cat>[^\.,;:!\?،\-]{2,})"
            };
            foreach (var pat in likePatterns)
            {
                var m = System.Text.RegularExpressions.Regex.Match(q, pat, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (m.Success) return m.Groups["cat"].Value.Trim();
            }

            return null;
        }

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
            ? "Hi! I’m Aseer Alkotb assistant 🤖. I can help you with book summaries, availability, prices, author bios, and recommendations."
            : "أهلاً! أنا مساعد عصير الكتب 🤖. أقدر أساعدك في ملخصات الكتب، التوافر، الأسعار، نبذات المؤلفين، والترشيحات.";
    }
}
