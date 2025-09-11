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
using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class RagService : AppService, IRagService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEmbeddingService _emb;
        private readonly IAnswerSynthesisService _synth;

        private const int MIN_DESC_CHARS = 160;

        public RagService(
            IUnitOfWork uow,
            IEmbeddingService emb,
            IAnswerSynthesisService synth,
            IServiceProvider sp,
            Microsoft.Extensions.Hosting.IHostEnvironment env) : base(sp, env)
        {
            _uow = uow;
            _emb = emb;
            _synth = synth;
        }

        #region Public API
        public async Task<ApiResponse<RagAskResponse>> AskAsync(RagAskRequest request)
        {
            await DoValidationAsync<Application.Features.Rag.Validators.RagAskRequestValidator, RagAskRequest>(request);

            // Language detection (Arabic/English) from the user's question
            var lang = DetectLanguage(request.Question);
            var L = new Localizer(lang);

            var sanitizedCategory = SanitizeCategory(request.Category);
            var (titleGuess, authorGuess) = QueryExtractor.Extract(request.Question);
            var qNorm = Normalize(request.Question);
            var showIntro = LooksLikeGreeting(qNorm);

            var asksAuthorBio =
                   ContainsAny(qNorm, AuthorBioKeys)
                || Regex.IsMatch(qNorm, @"\b(about\s+the\s+author|author\s+bio|who\s+is)\b", RegexOptions.IgnoreCase)
                || Regex.IsMatch(qNorm, @"\b(نبذة|سيرة)\s+عن\s+ال?(?:كاتب|مؤلف)\b");

            var asksSummary = !asksAuthorBio && (
                   ContainsAny(qNorm, SummaryKeys)
                || Regex.IsMatch(qNorm, @"\b(?:تلخيص|ملخص|summary|tl;dr)\b", RegexOptions.IgnoreCase)
                || Regex.IsMatch(qNorm, @"\bنبذة\s+عن\s+(?:كتاب|رواية)\b")
            );

            var asksAvailability = ContainsAny(qNorm, AvailabilityKeys);
            var asksCategory = ContainsAny(qNorm, CategoryKeys) || !string.IsNullOrWhiteSpace(sanitizedCategory);
            var asksAuthor = !asksSummary && (ContainsAny(qNorm, AuthorMoreKeysLoose) || HasByAuthorPattern(qNorm) || HasArabicAuthorL(qNorm));

            // Friendly short intro if it looks like a greeting with a very short message
            if (showIntro && qNorm.Replace(" ", "").Length <= 12)
            {
                var greet = L.T(
                    "👋 أهلاً بك! أنا مساعد عصير الكُتُب — أقدر أساعدك في الترشيحات، الملخصات، وتوفر الكتب.\n\nأمثلة:\n- ترشيحات في تصنيف «روايات»\n- ملخص كتاب «الخيميائي»\n- هل «العادات الذرية» متاح؟\n- نبذة عن نجيب محفوظ",
                    "Hi! I’m the Aseer Alkotb assistant — I can help with recommendations, summaries, and availability.\n\nExamples:\n- Recommendations in the 'Novels' genre\n- Summary of “The Alchemist”\n- Is “Atomic Habits” in stock?\n- Short bio of Naguib Mahfouz"
                );

                // لا نضيف Outro مع الترحيب القصير
                return Success(new RagAskResponse
                {
                    Answer = greet,
                    Sources = new List<ChatSource>()
                });
            }

            // SUMMARY
            // داخل AskAsync – فرع SUMMARY استبدل الكود بالكامل بهذا:
            if (asksSummary)
            {
                var titleToken = !string.IsNullOrWhiteSpace(titleGuess) ? titleGuess! : request.Question;

                var book = await _uow.Books.GetQueryable(
                                b => b.IsActive && ((b.Title ?? "").ToLower().Contains(titleToken.ToLower())),
                                q => q.Include(b => b.Author))
                            .OrderByDescending(b => b.SalesCount)
                            .FirstOrDefaultAsync();

                if (book is null)
                {
                    var msg = L.T(
                        "لم أجد كتابًا بعنوان واضح في كتالوجنا. جرّب كتابة الاسم بدقة أكبر (مثال: اسم الكتاب + اسم المؤلف).",
                        "I couldn't find a clear title in our catalog. Please try a more precise title (e.g., book title + author)."
                    );
                    return Success(new RagAskResponse { Answer = FriendlyWrap(msg, lang, false) });
                }

                var baseSource = new ChatSource
                {
                    BookId = book.Id,
                    Title = book.Title,
                    CoverImageUrl = book.CoverImageUrl,
                    Snippet = book.Description
                };

                string summaryText;

                if (ChunkFactory.HasRichDescription(book))
                {
                    var prompt = L.T(
                        $"لخّص كتاب \"{book.Title}\" للمؤلف {book.Author?.Name} في 3–5 نقاط بالاعتماد على الوصف المرفق فقط.",
                        $"Summarize the book \"{book.Title}\" by {book.Author?.Name} in 3–5 bullet points using ONLY the attached description."
                    );
                    summaryText = await _synth.SynthesizeAsync(prompt, new List<ChatSource> { baseSource }) ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(summaryText))
                        summaryText = book.Description!.Trim().Length > 0 ? book.Description! : L.T("لا تتوفر لدينا نبذة كافية لهذا الكتاب حاليًا.", "We don't have enough info for this book yet.");
                }
                else
                {
                    var prompt = L.T(
                        $"أعطني ملخصًا موجزًا وواضحًا لكتاب \"{book.Title}\" للمؤلف {book.Author?.Name}.",
                        $"Give me a brief and clear summary of \"{book.Title}\" by {book.Author?.Name}."
                    );
                    summaryText = await _synth.SynthesizeAsync(prompt, new List<ChatSource>()) ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(summaryText))
                        summaryText = L.T("لا تتوفر لدينا نبذة كافية لهذا الكتاب حاليًا.", "We don't have enough info for this book yet.");
                }

                return Success(new RagAskResponse
                {
                    Answer = FriendlyWrap(summaryText, lang, false),
                    Sources = new List<ChatSource> { baseSource },
                    PrimaryBookId = book.Id,
                    PrimaryBookTitle = book.Title
                });
            }


            // AVAILABILITY
            // داخل AskAsync – فرع AVAILABILITY بدّل الكود بالكامل بهذا:
            if (asksAvailability)
            {
                var titleTokenRaw = !string.IsNullOrWhiteSpace(titleGuess) ? titleGuess! : ExtractTitleForAvailability(request.Question);
                var tokenCandidate = !string.IsNullOrWhiteSpace(titleTokenRaw) ? titleTokenRaw : CleanForTitle(request.Question);

                if (string.IsNullOrWhiteSpace(tokenCandidate) || tokenCandidate.Length < 2)
                {
                    var msg = L.T(
                        "علشان أقدر أحدد التوفّر، اكتب اسم الكتاب (مثال: هل \"أولاد حارتنا\" متاح؟).",
                        "To check availability, please provide a book title (e.g., Is \"The Alchemist\" available?)."
                    );
                    return Success(new RagAskResponse { Answer = FriendlyWrap(msg, lang, false), Sources = new List<ChatSource>() });
                }

                var core = CleanForTitle(tokenCandidate);
                var alt = core.StartsWith("ال") ? core[2..] : core;

                var match = await _uow.Books.GetQueryable(
                                b => b.IsActive && (EF.Functions.Like(b.Title!, $"%{core}%") || EF.Functions.Like(b.Title!, $"%{alt}%")),
                                q => q.Include(b => b.Author))
                            .OrderByDescending(b => b.SalesCount)
                            .FirstOrDefaultAsync();

                // فَزّة Embedding لو مفيش نتيجة نصّية
                if (match is null)
                {
                    var hit = (await _emb.SearchSimilarBooksAsync(core, 1)).FirstOrDefault();
                    match = hit?.Book;
                }

                if (match is null)
                {
                    var notFound = L.T(
                        $"ملقتش عنوان مطابق لـ \"{titleTokenRaw}\". اكتب الاسم بدقة أكبر (مثال: الاسم + المؤلف).",
                        $"Couldn't find a title matching \"{titleTokenRaw}\". Try a more precise title (e.g., title + author)."
                    );
                    return Success(new RagAskResponse { Answer = FriendlyWrap(notFound, lang, false) });
                }

                var available = match.StockQuantity > 0;
                var ans = available
                    ? L.T($"\"{match.Title}\" متاح للشراء الآن.", $"\"{match.Title}\" is available to buy now.")
                    : L.T($"\"{match.Title}\" غير متاح حاليًا.", $"\"{match.Title}\" is currently unavailable.");

                var src = ToSources(new List<BookBriefDto> {
        new BookBriefDto(
            Id: match.Id,
            Title: match.Title,
            AuthorName: match.Author?.Name,
            Price: match.Price,
            DiscountedPrice: match.DiscountedPrice,
            CoverImageUrl: match.CoverImageUrl,
            Description: match.Description)
    }, request.Question);

                return Success(new RagAskResponse
                {
                    Answer = FriendlyWrap(ans, lang, true),
                    Sources = src,
                    IsAvailable = available,
                    PrimaryBookId = match.Id,
                    PrimaryBookTitle = match.Title,
                    CanAddToCart = available
                });
            }


            // MORE BY AUTHOR
            if (asksAuthor || !string.IsNullOrWhiteSpace(authorGuess))
            {
                var authorName = !string.IsNullOrWhiteSpace(authorGuess) ? authorGuess! : request.Question;
                var list = await GetAuthorBooksAsync(authorName);
                var names = list.Data!.Select(b => b.Title).ToList();

                var ans = names.Any()
                    ? L.T($"كتب أخرى لنفس المؤلف المقترحة: {string.Join("، ", names)}.",
                          $"Other suggested books by the same author: {string.Join(", ", names)}.")
                    : L.T("لم أجد كتبًا مناسبة لنفس المؤلف.", "I couldn't find suitable books for the same author.");

                return Success(new RagAskResponse { Answer = FriendlyWrap(ans, lang, true), Sources = ToSources(list.Data!) });
            }

            // BY CATEGORY
            if (asksCategory)
            {
                var cat = sanitizedCategory ?? ExtractCategory(request.Question);
                if (!string.IsNullOrWhiteSpace(cat))
                {
                    var take = (request.Limit > 0 ? request.Limit : 8);
                    var list = await GetCategoryBooksAsync(cat!, take);
                    var dedup = list.Data!.GroupBy(b => b.Title.Trim()).Select(g => g.First()).Take(take).ToList();

                    var ans = dedup.Any()
                        ? L.T($"ترشيحات لكتب ضمن التصنيف \"{cat}\": {string.Join("، ", dedup.Select(b => b.Title))}.",
                              $"Recommended books in the \"{cat}\" category: {string.Join(", ", dedup.Select(b => b.Title))}.")
                        : L.T($"لا توجد نتائج ضمن التصنيف \"{cat}\" حالياً.",
                              $"No current results in the \"{cat}\" category.");

                    return Success(new RagAskResponse { Answer = FriendlyWrap(ans, lang, false), Sources = ToSources(dedup, request.Question) });
                }
            }

            // AUTHOR BIO
            if (asksAuthorBio)
            {
                var authorName = !string.IsNullOrWhiteSpace(authorGuess)
                    ? authorGuess!
                    : (ExtractAuthorNameForBio(request.Question) ?? request.Question);

                var token = authorName.Trim();
                var token2 = token.StartsWith("ال") ? token[2..] : token;

                var author = await _uow.Authors.GetQueryable(a =>
                                    a.Name.ToLower().Contains(token.ToLower()) ||
                                    a.Name.ToLower().Contains(token2.ToLower()))
                                .Include(a => a.Books)
                                .FirstOrDefaultAsync();

                if (author is null)
                {
                    var msg = L.T(
                        "مش لاقي مؤلف بالاسم المطلوب في كتالوجنا. جرّب تكتب الاسم كامل أو كتاب مشهور له.",
                        "I couldn't find an author with that name in our catalog. Try the full name or a well-known book by them."
                    );
                    return Success(new RagAskResponse { Answer = FriendlyWrap(msg, lang, false) });
                }

                var bio = author.Bio;

                if (!ChunkFactory.HasGoodBio(bio))
                {
                    var prompt = L.T(
                        $"اكتب نبذة قصيرة وواضحة عن المؤلف {author.Name}.",
                        $"Write a short, clear bio for the author {author.Name}."
                    );
                    var synth = await _synth.SynthesizeAsync(prompt, new List<ChatSource>()) ?? string.Empty;

                    if (ChunkFactory.HasGoodBio(synth))
                    {
                        // حفظ الـ Bio مرة واحدة
                        author.Bio = synth.Trim();
                        await _uow.SaveAsync();
                        bio = author.Bio;
                    }
                    else
                    {
                        bio = L.T($"لا تتوفر لدينا نبذة كافية عن {author.Name} حاليًا.", $"We don't have a sufficient bio for {author.Name} yet.");
                    }
                }

                var src = new List<ChatSource> {
        new ChatSource {
            BookId = 0,
            Title = L.T($"نبذة عن {author.Name}", $"About {author.Name}"),
            Snippet = bio
        }
    };

                return Success(new RagAskResponse { Answer = FriendlyWrap(bio!, lang, false), Sources = src });
            }
            //if (asksAuthorBio)
            //{
            //    var authorName = !string.IsNullOrWhiteSpace(authorGuess) ? authorGuess! : (ExtractAuthorNameForBio(request.Question) ?? request.Question);

            //    var token = authorName.Trim();
            //    var token2 = token.StartsWith("ال") ? token[2..] : token;

            //    var author = await _uow.Authors.GetQueryable(a =>
            //                        a.Name.ToLower().Contains(token.ToLower()) ||
            //                        a.Name.ToLower().Contains(token2.ToLower()))
            //                    .Include(a => a.Books)
            //                    .FirstOrDefaultAsync();

            //    if (author is null)
            //    {
            //        var msg = L.T(
            //            "مش لاقي مؤلف بالاسم المطلوب في كتالوجنا. جرّب تكتب الاسم كامل أو كتاب مشهور له.",
            //            "I couldn't find an author with that name in our catalog. Try the full name or a well-known book by them."
            //        );
            //        return Success(new RagAskResponse { Answer = FriendlyWrap(msg, lang, false), Sources = new List<ChatSource>() });
            //    }

            //    var bio = author.GetType().GetProperty("Bio")?.GetValue(author) as string;

            //    var src = new List<ChatSource> {
            //        new ChatSource {
            //            BookId = 0,
            //            Title = L.T($"نبذة عن {author.Name}", $"About {author.Name}"),
            //            CoverImageUrl = null,
            //            Snippet = string.IsNullOrWhiteSpace(bio) ? L.T(
            //                $"لدى {author.Name} {author.Books?.Count ?? 0} كتابًا في كتالوجنا.",
            //                $"{author.Name} has {author.Books?.Count ?? 0} book(s) in our catalog."
            //            ) : bio
            //        }
            //    };

            //    if (!string.IsNullOrWhiteSpace(bio))
            //        return Success(new RagAskResponse { Answer = FriendlyWrap(bio!, lang, false), Sources = src });

            //    var enriched = L.T(
            //        $"اكتب نبذة قصيرة وواضحة عن المؤلف {author.Name}.",
            //        $"Write a short, clear bio for the author {author.Name}."
            //    );

            //    var synth = await _synth.SynthesizeAsync(enriched, new List<ChatSource>());
            //    var ans = string.IsNullOrWhiteSpace(synth)
            //        ? L.T($"لا تتوفر لدينا نبذة كافية عن {author.Name} حاليًا.", $"We don't have a sufficient bio for {author.Name} yet.")
            //        : synth!;

            //    return Success(new RagAskResponse { Answer = FriendlyWrap(ans, lang, false), Sources = src });
            //}

            // FALLBACK: Recommendations (vector → keyword), then website-style answer if needed
            var top = await GetRecommendationsAsync(request.Question);
            var reply = top.Data!.Any()
                ? L.T($"بناءً على سؤالك، أنصحك بالاطلاع على: {string.Join("، ", top.Data!.Select(x => x.Title))}.",
                      $"Based on your question, consider: {string.Join(", ", top.Data!.Select(x => x.Title))}.")
                : L.T("لم أجد كتباً متعلقة مباشرة بسؤالك. جرّب كلمات مفتاحية مختلفة أو تصنيفاً آخر.",
                      "I couldn't find books directly related to your question. Try different keywords or another category.");

            if (string.IsNullOrWhiteSpace(reply) || reply.Trim() == "لا أعرف")
            {
                var fallbackPrompt = lang == Language.English ? BuildWebsiteFallbackPromptEn(request.Question) : BuildWebsiteFallbackPrompt(request.Question);
                var fallbackAnswer = await _synth.SynthesizeAsync(fallbackPrompt, ToSources(top.Data ?? new List<BookBriefDto>(), request.Question));
                if (!string.IsNullOrWhiteSpace(fallbackAnswer))
                    return Success(new RagAskResponse
                    {
                        Answer = FriendlyWrap(fallbackAnswer!, lang, true),
                        Sources = ToSources(top.Data ?? new List<BookBriefDto>(), request.Question)
                    });
            }

            return Success(new RagAskResponse
            {
                Answer = FriendlyWrap(reply, lang, true),
                Sources = ToSources(top.Data ?? new List<BookBriefDto>(), request.Question)
            });
        }
        #endregion

        #region Queries
        private async Task<ApiResponse<string>> GetBookAvailabilityAsync(string bookTitle)
        {
            var lower = bookTitle.ToLower();

            var b = await _uow.Books.GetQueryable(
                        x => x.IsActive && ((x.Title ?? "").ToLower().Contains(lower)),
                        q => q.Include(x => x.Author))
                    .OrderByDescending(x => x.SalesCount)
                    .FirstOrDefaultAsync();

            if (b == null) return Success<string>($"لم يتم العثور على كتاب بعنوان: {bookTitle}");

            var available = b.StockQuantity > 0;
            var ans = available ? $"\"{b.Title}\" متاح للشراء الآن." : $"\"{b.Title}\" غير متاح حاليًا.";
            return Success(ans);
        }

        private async Task<ApiResponse<List<BookBriefDto>>> GetAuthorBooksAsync(string authorName)
        {
            var items = await _uow.Books.GetQueryable(
                    x => x.IsActive && x.Author != null && x.Author.Name.ToLower().Contains(authorName.ToLower()),
                    q => q.Include(b => b.Author))
                .OrderByDescending(b => b.SalesCount).ThenByDescending(b => b.ViewCount)
                .Take(20)
                .ToListAsync();

            return Success(items.Adapt<List<BookBriefDto>>());
        }

        private async Task<ApiResponse<List<BookBriefDto>>> GetCategoryBooksAsync(string categoryName, int take)
        {
            categoryName = categoryName?.Trim() ?? "";
            var q1 = categoryName;
            var q2 = categoryName.StartsWith("ال") ? categoryName[2..] : categoryName;

            var items = await _uow.Books.GetQueryable(
                x => x.IsActive && x.Categories.Any(c =>
                    EF.Functions.Like(c.Name, $"%{q1}%")
                    || EF.Functions.Like(c.Name, $"%{q2}%")
                    || EF.Functions.Like("ال" + c.Name, $"%{q1}%")),
                q => q.Include(b => b.Author).Include(b => b.Categories))
            .OrderByDescending(b => b.SalesCount)
            .ThenByDescending(b => b.ViewCount)
            .Take(take)
            .ToListAsync();

            return Success(items.Adapt<List<BookBriefDto>>());
        }

        private async Task<ApiResponse<List<BookBriefDto>>> GetRecommendationsAsync(string query)
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
        #endregion

        #region Helpers (Parsing / Text)
        private static string CleanForTitle(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var v = s.Trim();

            string[] stop = {
                "كتاب","الكتاب","كتب","رواية","الرواية","روايات",
                "ملخص","تلخيص","عن","حول",
                "عايز","عاوز","أريد","اريد","محتاج",
                "هل","موجود","متاح","متوفر",
                "available","in","stock","book","novel"
            };

            foreach (var w in stop)
                v = Regex.Replace(v, $@"\b{Regex.Escape(w)}\b", "", RegexOptions.IgnoreCase);

            v = v.Replace("“", "\"").Replace("”", "\"")
                 .Trim('\"', '«', '»', '\'', '`', ' ', ':', '-', '–', '—', '.', '،', '؛', '?', '؟', '!');
            v = Normalize(v);
            return v.Trim();
        }

        private static string? ExtractTitleForAvailability(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var s = text.Trim();

            var q = Regex.Match(s, "[\"«»“”'`]{1}(?<t>[^\"«»“”'`]{2,})[\"«»“”'`]{1}");
            if (q.Success) return q.Groups["t"].Value.Trim();

            var m = Regex.Match(
                s,
                @"(?:هل)?\s*(?:ال)?(?:كتاب|كتب|رواية|روايات)\s+(?<t>[^\.،:;!\?؟]{2,})\s+(?:موجود|متاح|متوفر)",
                RegexOptions.IgnoreCase);
            if (m.Success) return m.Groups["t"].Value.Trim();

            m = Regex.Match(s, @"(?<t>[^\.،:;!\?؟]{2,})\s+(?:موجود|متاح|متوفر)", RegexOptions.IgnoreCase);
            if (m.Success) return m.Groups["t"].Value.Trim();

            var e = Regex.Match(s, @"(?:is|book|novel)\s+(?<t>[^\.,:;!\?]{2,})\s+(?:available|in\s+stock)", RegexOptions.IgnoreCase);
            if (e.Success) return e.Groups["t"].Value.Trim();

            return null;
        }

        // Helpers (Parsing / Text) — add this method
        private static string? ExtractAuthorNameForBio(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var s = text.Trim()
                        .Replace("؟", "")
                        .Replace("?", "")
                        .Replace("“", "\"")
                        .Replace("”", "\"")
                        .Trim('"', '«', '»', '\'', '`');

            // Arabic & English patterns to catch "about the author …", "نبذة عن …", etc.
            var patterns = new[]
            {
        // Arabic
        @"نبذة\s+عن\s+(?<name>[\p{L}\s\.\-']{2,})",
        @"عن\s+ال?كاتب\s+(?<name>[\p{L}\s\.\-']{2,})",
        @"عن\s+ال?مؤلف\s+(?<name>[\p{L}\s\.\-']{2,})",
        @"من\s+هو\s+(?<name>[\p{L}\s\.\-']{2,})",

        // English
        @"about\s+the\s+author\s+(?<name>[A-Za-z\.\-'\s]{2,})",
        @"author\s+bio\s+of\s+(?<name>[A-Za-z\.\-'\s]{2,})",
        @"bio\s+of\s+(?<name>[A-Za-z\.\-'\s]{2,})"
    };

            foreach (var pat in patterns)
            {
                var m = Regex.Match(s, pat, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    var name = m.Groups["name"].Value.Trim();

                    // tidy up trailing punctuation
                    name = Regex.Replace(name, @"[\.,;:!\?؟،\-]+$", "");

                    // remove leading “الكاتب/المؤلف …” if captured as part of the name
                    name = Regex.Replace(name, @"^(?:ال)?(?:كاتب|مؤلف)\s+", "", RegexOptions.IgnoreCase);

                    return string.IsNullOrWhiteSpace(name) ? null : name;
                }
            }

            return null;
        }

        private static string Normalize(string? x)
        {
            if (string.IsNullOrWhiteSpace(x)) return string.Empty;
            var s = x.Trim().ToLowerInvariant();
            s = s.Replace("ـ", "").Replace('أ', 'ا').Replace('إ', 'ا').Replace('آ', 'ا').Replace('ى', 'ي').Replace('ؤ', 'و').Replace('ئ', 'ي');
            var diacritics = new[] { '\u064B', '\u064C', '\u064D', '\u064E', '\u064F', '\u0650', '\u0651', '\u0652' };
            foreach (var d in diacritics) s = s.Replace(d.ToString(), "");
            return s;
        }

        private static bool ContainsAny(string haystack, string[] needles)
            => needles.Any(k => haystack.Contains(k, StringComparison.OrdinalIgnoreCase));

        private static bool HasByAuthorPattern(string qNorm)
            => Regex.IsMatch(qNorm, @"\bby\s+[a-z][a-z\.\-'\s]{1,60}\b", RegexOptions.IgnoreCase);

        private static bool HasArabicAuthorL(string qNorm)
            => Regex.IsMatch(qNorm, @"\bل\s*[اأإآء-ي][\p{L}\s\.\-']{1,60}\b", RegexOptions.IgnoreCase);

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

        private static string? ExtractCategory(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var q = text.Trim();
            var qLower = q.ToLower();
            var markers = new[] { "نوع", "تصنيف", "genre", "category" };

            foreach (var m in markers)
            {
                var idx = qLower.IndexOf(m, StringComparison.Ordinal);
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
                @"بحب\s ال?(?:نوع|تصنيف)\s+(?<cat>[^\.,;:!\?؟،\-]{2,})",
                @"i\s+like\s+the\s+(?:genre|category)\s+(?<cat>[^\.,;:!\?،\-]{2,})"
            };

            foreach (var pat in likePatterns)
            {
                var m = Regex.Match(q, pat, RegexOptions.IgnoreCase);
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

        private static string BuildWebsiteFallbackPrompt(string question) => $@"
أنت مساعد ودود لمنصّة أسير الكُتب (https://aseeralkotb.com).
- أجب بالعربية المبسطة وباختصار.
- إن كان السؤال عن الكتب (ترشيحات/ملخص/توافر)، اعتمد على المقتطفات/المصادر المرافقة فقط.
- إن لم تكن المقتطفات كافية، اذكر ذلك بلطف واقترح صياغات بحث بديلة.

❓ السؤال:
{question}

✅ الجواب:";

        private static string BuildWebsiteFallbackPromptEn(string question) => $@"
You are a friendly assistant for Aseer Alkotb (https://aseeralkotb.com).
- Answer briefly in simple English.
- If the question is about books (recommendations/summary/availability), rely on provided snippets/sources only.
- If snippets are not enough, say so politely and suggest better search wording.

❓ Question:
{question}

✅ Answer:";

        private static readonly string[] GreetingKeys = { "hi", "hello", "hey", "السلام", "مرحبا", "ازيك", "أهلاً", "اهلا", "هاي", "هلو" };

        private static bool LooksLikeGreeting(string qNorm)
            => GreetingKeys.Any(k => qNorm.Contains(k, StringComparison.OrdinalIgnoreCase));
        #endregion

        #region Localization & Language
        private enum Language { Arabic, English }

        private static Language DetectLanguage(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return Language.Arabic;
            // count Arabic vs Latin letters
            int ar = Regex.Matches(text, "[\\u0600-\\u06FF]").Count;
            int en = Regex.Matches(text, "[A-Za-z]").Count;
            return en > ar ? Language.English : Language.Arabic;
        }

        private static string FriendlyWrap(string answer, Language lang, bool addOutro)
        {
            if (!addOutro) return answer;

            var outro = lang == Language.English
                ? "— If you want to change the genre or search by title/author, just tell me in simple words 🙂"
                : "— لو حابب تغيّر التصنيف أو تبحث بعنوان/مؤلف، قلّي بكلمات بسيطة وأنا أظبطه لك 😊";

            return $"{answer}\n\n{outro}";
        }

        private readonly struct Localizer
        {
            private readonly Language _lang;
            public Localizer(Language lang) { _lang = lang; }
            public string T(string ar, string en) => _lang == Language.Arabic ? ar : en;
        }
        #endregion

        #region Keyword Buckets
        private static readonly string[] SummaryKeys = {
            "تلخيص","ملخص","لخص","خلاصة","شرح مختصر","ملخص سريع",
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

        private static readonly string[] AuthorBioKeys = {
            "سيرة","نبذة","من هو","عن المؤلف","عن الكاتب","bio","biography","about the author","who is"
        };
        #endregion
    }
}
