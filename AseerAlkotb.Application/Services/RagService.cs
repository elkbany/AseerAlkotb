using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Books.Requests;
using AseerAlkotb.Application.Features.Authors.Requests;
using AseerAlkotb.Application.Features.Categories.Requests;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Application.Features.Rag.Requests;
using AseerAlkotb.Application.Features.Rag.Responses;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Interfaces.Base;
using Mapster;
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;
using Microsoft.EntityFrameworkCore;
using AseerAlkotb.Application.Utils;

namespace AseerAlkotb.Application.Services
{
    public class RagService : AppService, IRagService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmbeddingService _emb;
        private readonly IAnswerSynthesisService _synth;

        public RagService(IUnitOfWork uow, IEmbeddingService emb,
                  IAnswerSynthesisService synth,
                  IServiceProvider sp, Microsoft.Extensions.Hosting.IHostEnvironment env)
    : base(sp, env)
        {
            _uow = uow; _emb = emb; _synth = synth;
        }
        public async Task<ApiResponse<RagAskResponse>> AskAsync(RagAskRequest request)
        {
            await DoValidationAsync<Application.Features.Rag.Validators.RagAskRequestValidator, RagAskRequest>(request);
            var sanitizedCategory = SanitizeCategory(request.Category);

            var q = request.Question.Trim().ToLower();
            var (titleGuess, authorGuess) = QueryExtractor.Extract(request.Question);
            var qNorm = Normalize(request.Question);


            // نوايا بسيطة
            bool asksSummary = ContainsAny(qNorm, SummaryKeys);
            bool asksAvailability = ContainsAny(qNorm, AvailabilityKeys);
            bool asksCategory = ContainsAny(qNorm, CategoryKeys) || !string.IsNullOrWhiteSpace(SanitizeCategory(request.Category));
            bool asksAuthor = !asksSummary && (
                ContainsAny(qNorm, AuthorMoreKeysLoose) || HasByAuthorPattern(qNorm) || HasArabicAuthorL(qNorm)
            );

            // ===== 1) Summary FIRST (أولوية قصوى) =====
            if (asksSummary)
            {
                var enrichedQuestion = !string.IsNullOrWhiteSpace(titleGuess)
                                        ? $"أريد تلخيص كتاب \"{titleGuess}\". {request.Question}"
                                        : request.Question;

                var answer = await _synth.SynthesizeAsync(enrichedQuestion, new List<ChatSource>());
                return Success(new RagAskResponse
                {
                    Answer = string.IsNullOrWhiteSpace(answer) ? "لم أستطع توليد ملخص حالياً." : answer,
                    Sources = new List<ChatSource>()
                });
            }

            // ===== 2) Availability =====
            if (asksAvailability)
            {
                var titleToken = !string.IsNullOrWhiteSpace(titleGuess) ? titleGuess! : request.Question;
                var availability = await GetBookAvailabilityAsync(titleToken);
                return Success(new RagAskResponse { Answer = availability.Data! });
            }

            // ===== 3) Author suggestions (بعد ما استبعدنا التلخيص) =====
            if (asksAuthor || !string.IsNullOrWhiteSpace(authorGuess))
            {
                var authorName = !string.IsNullOrWhiteSpace(authorGuess) ? authorGuess! : request.Question;
                var list = await GetAuthorBooksAsync(authorName);
                var names = list.Data!.Select(b => b.Title).ToList();
                var ans = names.Any()
                    ? $"كتب أخرى لنفس المؤلف المقترحة: {string.Join("، ", names)}."
                    : "لم أجد كتبًا مناسبة لنفس المؤلف.";
                return Success(new RagAskResponse { Answer = ans, Sources = ToSources(list.Data!) });
            }

            // ===== 4) Category recs =====
            if (asksCategory)
            {
                var cat = sanitizedCategory ?? ExtractCategory(request.Question);
                if (!string.IsNullOrWhiteSpace(cat))
                {
                    var take = (request.Limit > 0 ? request.Limit : 8);
                    var list = await GetCategoryBooksAsync(cat, take);
                    var dedup = list.Data!
                        .GroupBy(b => b.Title.Trim())
                        .Select(g => g.First())
                        .Take(take)
                        .ToList();

                    var ans = dedup.Any()
                        ? $"ترشيحات لكتب ضمن التصنيف \"{cat}\": {string.Join("، ", dedup.Select(b => b.Title))}."
                        : $"لا توجد نتائج ضمن التصنيف \"{cat}\" حالياً.";

                    return Success(new RagAskResponse
                    {
                        Answer = ans,
                        Sources = ToSources(dedup, request.Question)
                    });
                }
            }




            // ===== 5) Fallback: semantic recs =====
            var top = await GetRecommendationsAsync(request.Question);
            var reply = top.Data!.Any()
                ? $"بناءً على سؤالك، أنصحك بالاطلاع على: {string.Join("، ", top.Data!.Select(x => x.Title))}."
                : "لم أجد كتباً متعلقة مباشرة بسؤالك. جرّب كلمات مفتاحية مختلفة أو تصنيفاً آخر.";

            return Success(new RagAskResponse { Answer = reply, Sources = ToSources(top.Data!) });
        }

        public async Task<ApiResponse<string>> GetBookAvailabilityAsync(string bookTitle)
        {
            var lower = bookTitle.ToLower();

            var b = await _uow.Books.GetQueryable(
                        x => x.IsActive && ((x.Title ?? "").ToLower().Contains(lower)),
                        q => q.Include(x => x.Author))
                    .OrderByDescending(x => x.SalesCount)
                    .FirstOrDefaultAsync();

            if (b == null)
                return Success<string>($"لم يتم العثور على كتاب بعنوان: {bookTitle}");

            var available = b.StockQuantity > 0;
            var ans = available
                ? $"\"{b.Title}\" متاح للشراء الآن."
                : $"\"{b.Title}\" غير متاح حاليًا.";

            return Success(ans);
        }

        public async Task<ApiResponse<List<BookBriefDto>>> GetAuthorBooksAsync(string authorName)
        {
            var items = await _uow.Books.GetQueryable(
                    x => x.IsActive && x.Author != null && x.Author.Name.ToLower().Contains(authorName.ToLower()),
                    q => q.Include(b => b.Author))
                .OrderByDescending(b => b.SalesCount).ThenByDescending(b => b.ViewCount)
                .Take(20)
                .ToListAsync();

            return Success(items.Adapt<List<BookBriefDto>>());
        }

        public async Task<ApiResponse<List<BookBriefDto>>> GetCategoryBooksAsync(string categoryName)
        {
            return await GetCategoryBooksAsync(categoryName, 20);
        }

        public async Task<ApiResponse<List<BookBriefDto>>> GetCategoryBooksAsync(string categoryName, int take)
        {
            categoryName = categoryName?.Trim() ?? "";
            var q1 = categoryName;                         
            var q2 = categoryName.StartsWith("ال") ? categoryName[2..] : categoryName; 

            var items = await _uow.Books.GetQueryable(
                x => x.IsActive && x.Categories.Any(c =>
                    EF.Functions.Like(c.Name, $"%{q1}%")    
                    || EF.Functions.Like(c.Name, $"%{q2}%") 
                    || EF.Functions.Like("ال" + c.Name, $"%{q1}%") 
                ),
                q => q.Include(b => b.Author).Include(b => b.Categories)
            )
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

            // fallback keyword
            var items = await _uow.Books.GetQueryable(
                    x => x.IsActive && ((x.Title ?? "").Contains(query) || (x.Description ?? "").Contains(query)),
                    q => q.Include(b => b.Author))
                .OrderByDescending(b => b.SalesCount).ThenByDescending(b => b.ViewCount)
                .Take(10)
                .ToListAsync();

            return Success(items.Adapt<List<BookBriefDto>>());
        }

        // Helpers

        // 1) قوائم موسّعة
        private static readonly string[] SummaryKeys = {
  "تلخيص","ملخص","لخص","خلاصة","شرح مختصر","نبذة","ملخص سريع",
  "summary","summarize","brief","overview","tl;dr"
};

        private static readonly string[] AvailabilityKeys = {
  "متاح","متوفر","موجود","أقدر أشتري","اقدر اشتري","اشتري","شراء",
  "in stock","available","buy","purchase","order",
  "غير متاح","غير متوفر","نفد","خلص","out of stock"
};

        private static readonly string[] CategoryKeys = {
  "نوع","تصنيف","فئة","قسم","مجال","موضوع",
  "genre","category","kind","type","topic","subject"
};

        private static readonly string[] AuthorMoreKeysLoose = {
  "كتب أخرى","له كتب","مؤلفات أخرى","نفس المؤلف","نفس الكاتب",
  "more books by","other books by","from the same author","works of"
};

        // 2) تطبيع + فحص Contains
        private static string Normalize(string? x)
        {
            if (string.IsNullOrWhiteSpace(x)) return string.Empty;
            var s = x.Trim().ToLowerInvariant();
            s = s.Replace("ـ", "");                    // كشيدة
            s = s.Replace('أ', 'ا').Replace('إ', 'ا').Replace('آ', 'ا');
            s = s.Replace('ى', 'ي').Replace('ؤ', 'و').Replace('ئ', 'ي');
            // حذف التشكيل
            var diacritics = new[] { '\u064B', '\u064C', '\u064D', '\u064E', '\u064F', '\u0650', '\u0651', '\u0652' };
            foreach (var d in diacritics) s = s.Replace(d.ToString(), "");
            return s;
        }
        private static bool ContainsAny(string haystack, string[] needles)
            => needles.Any(k => haystack.Contains(k, StringComparison.OrdinalIgnoreCase));

        // 3) فحوص regex أدق لبعض المفاتيح الحساسة
        private static bool HasByAuthorPattern(string qNorm)
        {
            // \bby\s+ <اسم>  (إنجليزي)
            return System.Text.RegularExpressions.Regex.IsMatch(qNorm, @"\bby\s+[a-z][a-z\.\-'\s]{1,60}\b",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
        private static bool HasArabicAuthorL(string qNorm)
        {
            // ل/لـ + اسم عربي (خفيف)
            return System.Text.RegularExpressions.Regex.IsMatch(qNorm, @"\bل\s*[اأإآء-ي][\p{L}\s\.\-']{1,60}\b",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }


        private static List<ChatSource> ToSources(List<BookBriefDto> books, string? question = null) =>
    books.Select(b => new ChatSource
    {
        BookId = b.Id,
        Title = b.Title,
        CoverImageUrl = b.CoverImageUrl,
        Snippet = BuildSnippet(b.Description, question ?? "")
    }).ToList();


        // Helper لو مش موجود عندك
        private static string? BuildSnippet(string? description, string question)
        {
            if (string.IsNullOrWhiteSpace(description)) return null;
            var desc = description.Length > 240 ? description[..240] + "..." : description;
            return desc;
        }

        private static string? ExtractCategory(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            // خد النص الأصلي (مش لازم lower) وبعدين اشتغل بنسخة lower للمطابقة
            var q = text.Trim();
            var qLower = q.ToLower();

            // نماذج ماركرز: نوع/تصنيف/genre/category
            var markers = new[] { "نوع", "تصنيف", "genre", "category" };

            foreach (var m in markers)
            {
                var idx = qLower.IndexOf(m);
                if (idx >= 0)
                {
                    var start = idx + m.Length;
                    var tail = q.Substring(start).Trim(' ', ':', '：', '-', '،');

                    var stops = new[] { "،", ",", ";", "؛", ".", ":", "—", "-", "!", "؟", "?" };
                    foreach (var s in stops)
                    {
                        var cut = tail.IndexOf(s, StringComparison.Ordinal);
                        if (cut >= 0) { tail = tail[..cut]; break; }
                    }

                    tail = tail.Trim().Trim('\"', '“', '”', '«', '»', '\'', '`');


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

        private static string? SanitizeCategory(string? cat)
        {
            if (string.IsNullOrWhiteSpace(cat)) return null;
            var v = cat.Trim().Trim('"', '“', '”');
            var bad = new[] { "string", "undefined", "null", "-" };
            return bad.Contains(v, StringComparer.OrdinalIgnoreCase) ? null : v;
        }

    }
}
