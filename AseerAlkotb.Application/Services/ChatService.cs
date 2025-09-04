using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Chat.Requests;
using AseerAlkotb.Application.Features.Chat.Responses;
using AseerAlkotb.Application.ResponseHandler;
using AseerAlkotb.Domain.Interfaces.Base;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

using static AseerAlkotb.Application.ResponseHandler.ApiResponseHandler;

namespace AseerAlkotb.Application.Services
{
    public class ChatService : AppService, IChatService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public ChatService(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory, IConfiguration configuration, IServiceProvider serviceProvider, Microsoft.Extensions.Hosting.IHostEnvironment environment)
            : base(serviceProvider, environment)
        {
            this.unitOfWork = unitOfWork;
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        public async Task<ApiResponse<ChatResponse>> AskAsync(ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest<ChatResponse>("Question is required");
            }

            // 1) Try structured intents (availability, author suggestions, category recommendations)
            var intentResult = await TryHandleIntentsAsync(request);
            if (intentResult != null)
            {
                return Success(intentResult);
            }

            var query = unitOfWork.Books.GetQueryable();

            // basic keyword retrieval on title/description, filtered by language/category if provided
            if (!string.IsNullOrWhiteSpace(request.Language))
            {
                var lang = request.Language.Trim().ToLower();
                Domain.Enums.BookLanguage? parsed = null;
                if (lang is "arabic" or "ar" or "عربي" or "العربية") parsed = Domain.Enums.BookLanguage.Arabic;
                else if (lang is "english" or "en" or "انجليزي" or "الانجليزية") parsed = Domain.Enums.BookLanguage.English;
                else
                {
                    try { parsed = Enum.Parse<Domain.Enums.BookLanguage>(request.Language, ignoreCase: true); }
                    catch { }
                }
                if (parsed.HasValue)
                {
                    var languageValue = parsed.Value;
                    query = query.Where(b => b.Language == languageValue);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                var categoryLower = request.Category.Trim().ToLower();
                query = query.Where(b => b.Categories.Any(c => c.Name.ToLower().Contains(categoryLower)));
            }

            var normalizedQuestion = request.Question.Trim();
            var tokens = normalizedQuestion
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(t => t.ToLower())
                .Distinct()
                .ToList();

            // OR-match across tokens; if no tokens, attempt whole-question match
            if (tokens.Count > 0)
            {
                // Build an OR chain for EF (cannot use Aggregate with closures easily, so expand iteratively)
                var baseQuery = query.Where(b => false);
                foreach (var token in tokens)
                {
                    var t = token; // avoid modified closure
                    baseQuery = baseQuery
                        .Union(query.Where(b => b.Title.ToLower().Contains(t)))
                        .Union(query.Where(b => b.Description != null && b.Description.ToLower().Contains(t)));
                }
                query = baseQuery;
            }
            else
            {
                var qLower = normalizedQuestion.ToLower();
                query = query.Where(b => b.Title.ToLower().Contains(qLower) || (b.Description != null && b.Description.ToLower().Contains(qLower)));
            }

            var take = request.Limit.GetValueOrDefault(5);

            var candidates = await query
                .Select(b => new { b.Id, b.Title, b.Description, b.CoverImageUrl, b.ViewCount, b.SalesCount })
                .OrderByDescending(b => b.SalesCount)
                .ThenByDescending(b => b.ViewCount)
                .Take(take)
                .ToListAsync();

            // Fallback: if nothing matched, return popular books as suggestions
            if (candidates.Count == 0)
            {
                candidates = await unitOfWork.Books.GetQueryable()
                    .Select(b => new { b.Id, b.Title, b.Description, b.CoverImageUrl, b.ViewCount, b.SalesCount })
                    .OrderByDescending(b => b.SalesCount)
                    .ThenByDescending(b => b.ViewCount)
                    .Take(take)
                    .ToListAsync();
            }

            var sources = candidates.Select(c => new ChatSource
            {
                BookId = c.Id,
                Title = c.Title,
                CoverImageUrl = c.CoverImageUrl,
                Snippet = BuildSnippet(c.Description, request.Question)
            }).ToList();

            var answer = await TryGeminiAnswerAsync(request.Question, sources) ?? BuildHeuristicAnswer(request.Question, sources);

            var response = new ChatResponse
            {
                Answer = answer,
                Sources = sources
            };

            return Success(response);
        }

        private async Task<ChatResponse?> TryHandleIntentsAsync(ChatRequest request)
        {
            var q = request.Question.Trim();
            var qLower = q.ToLower();

            // Availability intent (Arabic/English)
            var availabilityKeywords = new[] { "متاح", "متوفر", "أشتري", "اشتري", "شراء", "available", "in stock", "buy" };
            var authorSuggestKeywords = new[] { "كاتب", "مؤلف", "له كتب", "كتب أخرى", "by", "other books" };
            var categoryKeywords = new[] { "نوع", "تصنيف", "فئة", "category", "genre", "بحب" };

            bool asksAvailability = availabilityKeywords.Any(k => qLower.Contains(k));
            bool asksAuthorSuggest = authorSuggestKeywords.Any(k => qLower.Contains(k));
            bool asksCategory = categoryKeywords.Any(k => qLower.Contains(k));

            // Extract potential title tokens (simple heuristic: remove common words)
            var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "عن","على","كتاب","كتب","انا","عايز","عايزه","عايزني","ابغى","احب","بحب",
                "i","want","need","book","books","the","a","an","is","are","that","this","of"
            };
            var titleTokens = q.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                               .Where(t => !stopWords.Contains(t))
                               .Select(t => t.ToLower())
                               .ToList();

            // Helper to find top matching book by title/description
            async Task<(int id,string title,string? desc,int authorId,bool available)?> FindTopBookAsync()
            {
                var baseQuery = unitOfWork.Books.GetQueryable();
                if (titleTokens.Count == 0)
                {
                    return null;
                }
                var bq = baseQuery.Where(b => false);
                foreach (var t in titleTokens)
                {
                    var tok = t;
                    bq = bq.Union(baseQuery.Where(b => b.Title.ToLower().Contains(tok)))
                           .Union(baseQuery.Where(b => b.Description != null && b.Description.ToLower().Contains(tok)));
                }
                var item = await bq
                    .Select(b => new { b.Id, b.Title, b.Description, b.AuthorId, b.StockQuantity, b.IsActive, b.SalesCount, b.ViewCount })
                    .OrderByDescending(x => x.SalesCount)
                    .ThenByDescending(x => x.ViewCount)
                    .FirstOrDefaultAsync();
                if (item == null) return null;
                var available = item.IsActive && item.StockQuantity > 0;
                return (item.Id, item.Title, item.Description, item.AuthorId, available);
            }

            // Availability intent flow
            if (asksAvailability)
            {
                var found = await FindTopBookAsync();
                if (found != null)
                {
                    var (id, title, desc, authorId, available) = found.Value;
                    var ans = available
                        ? $"نعم، كتاب \"{title}\" متاح للشراء حالياً."
                        : $"للأسف، كتاب \"{title}\" غير متاح حالياً للشراء.";

                    // Suggest other books by same author as part of availability
                    var suggestions = await unitOfWork.Books.GetQueryable()
                        .Where(b => b.AuthorId == authorId && b.Id != id)
                        .OrderByDescending(b => b.SalesCount)
                        .ThenByDescending(b => b.ViewCount)
                        .Take(5)
                        .Select(b => new { b.Id, b.Title, b.Description, b.CoverImageUrl })
                        .ToListAsync();

                    var sources = new List<ChatSource>
                    {
                        new ChatSource{ BookId = id, Title = title, Snippet = BuildSnippet(desc, q) }
                    };
                    sources.AddRange(suggestions.Select(s => new ChatSource{ BookId = s.Id, Title = s.Title, Snippet = BuildSnippet(s.Description, q), CoverImageUrl = s.CoverImageUrl }));

                    return new ChatResponse { Answer = ans, Sources = sources, IsAvailable = available, PrimaryBookId = id, PrimaryBookTitle = title, CanAddToCart = available };
                }
            }

            // Author suggestions flow (explicit or as follow-up to availability)
            if (asksAuthorSuggest)
            {
                var found = await FindTopBookAsync();
                if (found != null)
                {
                    var (id, title, desc, authorId, _) = found.Value;
                    var others = await unitOfWork.Books.GetQueryable()
                        .Where(b => b.AuthorId == authorId && b.Id != id)
                        .OrderByDescending(b => b.SalesCount)
                        .ThenByDescending(b => b.ViewCount)
                        .Take(5)
                        .Select(b => new { b.Id, b.Title, b.Description, b.CoverImageUrl })
                        .ToListAsync();

                    if (others.Count > 0)
                    {
                        var list = string.Join("، ", others.Select(o => o.Title));
                        var ans = $"كتب أخرى لنفس المؤلف المقترحة: {list}.";
                        var sources = new List<ChatSource> { new ChatSource{ BookId = id, Title = title, Snippet = BuildSnippet(desc, q) } };
                        sources.AddRange(others.Select(o => new ChatSource{ BookId = o.Id, Title = o.Title, Snippet = BuildSnippet(o.Description, q), CoverImageUrl = o.CoverImageUrl }));
                        return new ChatResponse { Answer = ans, Sources = sources, PrimaryBookId = id, PrimaryBookTitle = title };
                    }
                }
            }

            // Category preference flow
            if (asksCategory || !string.IsNullOrWhiteSpace(request.Category))
            {
                string? cat = request.Category;
                if (string.IsNullOrWhiteSpace(cat))
                {
                    // Try to extract a simple category name after words like نوع/تصنيف/genre/category
                    var markers = new[] { "نوع", "تصنيف", "genre", "category", "فئة" };
                    foreach (var m in markers)
                    {
                        var idx = qLower.IndexOf(m);
                        if (idx >= 0)
                        {
                            var tail = qLower.Substring(idx + m.Length).Trim(':', ' ', '-', '،');
                            if (!string.IsNullOrWhiteSpace(tail)) { cat = tail.Split(' ').FirstOrDefault(); }
                            if (!string.IsNullOrWhiteSpace(cat)) break;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(cat))
                {
                    var catLower = cat.ToLower();
                    var recs = await unitOfWork.Books.GetQueryable()
                        .Where(b => b.Categories.Any(c => c.Name.ToLower().Contains(catLower)))
                        .OrderByDescending(b => b.SalesCount)
                        .ThenByDescending(b => b.ViewCount)
                        .Take(request.Limit.GetValueOrDefault(5))
                        .Select(b => new { b.Id, b.Title, b.Description, b.CoverImageUrl })
                        .ToListAsync();

                    if (recs.Count > 0)
                    {
                        var list = string.Join("، ", recs.Select(r => r.Title));
                        var ans = $"ترشيحات لكتب ضمن التصنيف \"{cat}\": {list}.";
                        var sources = recs.Select(r => new ChatSource{ BookId = r.Id, Title = r.Title, Snippet = BuildSnippet(r.Description, q), CoverImageUrl = r.CoverImageUrl }).ToList();
                        return new ChatResponse { Answer = ans, Sources = sources }
                        ;
                    }
                }
            }

            return null;
        }

        private async Task<string?> TryGeminiAnswerAsync(string question, List<ChatSource> sources)
        {
            var apiKey = configuration["Gemini:ApiKey"];
            var model = configuration["Gemini:Model"] ?? "gemini-1.5-flash";
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return null; // no key configured -> fallback
            }

            var client = httpClientFactory.CreateClient();
            var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

            var contextText = BuildContextFromSources(sources);
            var systemPrompt = "أنت مساعد للكتب في موقع عصير الكتب. أجب بالعربية بدقة، واستند فقط إلى المقاطع التالية. لا تخترع معلومات. أعد قائمة بالمراجع المستخدمة في نهاية الرد إن أمكن.";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new object[]
                        {
                            new { text = systemPrompt },
                            new { text = "سؤال المستخدم:" },
                            new { text = question },
                            new { text = "مقاطع ذات صلة:" },
                            new { text = contextText }
                        }
                    }
                }
            };

            try
            {
                using var response = await client.PostAsJsonAsync(endpoint, payload);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                var data = await response.Content.ReadFromJsonAsync<GeminiResponseDto>();
                var reply = data?.candidates?.FirstOrDefault()?.content?.parts?.FirstOrDefault()?.text;
                return string.IsNullOrWhiteSpace(reply) ? null : reply;
            }
            catch
            {
                return null;
            }
        }

        private static string BuildContextFromSources(List<ChatSource> sources)
        {
            if (sources == null || sources.Count == 0) return string.Empty;
            var parts = sources.Select(s => $"- {s.Title}: {s.Snippet}");
            return string.Join("\n", parts);
        }

        private static string? BuildSnippet(string? description, string question)
        {
            if (string.IsNullOrWhiteSpace(description)) return null;
            var desc = description.Length > 400 ? description.Substring(0, 400) + "..." : description;
            return desc;
        }

        private static string BuildHeuristicAnswer(string question, List<ChatSource> sources)
        {
            if (sources.Count == 0)
            {
                return "لم أجد كتباً متعلقة مباشرة بسؤالك. جرّب كلمات مفتاحية مختلفة أو تصنيفاً آخر.";
            }

            var titles = string.Join("، ", sources.Select(s => s.Title));
            return $"بناءً على سؤالك، أنصحك بالاطلاع على: {titles}. ستجد التفاصيل في الوصف وروابط الكتب بالمراجع.";
        }
    }

    // Minimal DTOs to parse Gemini response
    internal class GeminiResponseDto
    {
        public List<Candidate>? candidates { get; set; }
    }

    internal class Candidate
    {
        public Content? content { get; set; }
    }

    internal class Content
    {
        public List<Part>? parts { get; set; }
    }

    internal class Part
    {
        public string? text { get; set; }
    }
}


