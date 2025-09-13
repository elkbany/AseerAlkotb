using System.Net.Http.Json;
using System.Text.Json;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Rag.Models;
using Microsoft.Extensions.Configuration;

namespace AseerAlkotb.Infrastructure.AI
{
    public class GeminiQuestionRouterService : IQuestionRouterService
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _cfg;

        public GeminiQuestionRouterService(IHttpClientFactory http, IConfiguration cfg)
        {
            _http = http; _cfg = cfg;
        }

        public async Task<RouteResult> RouteAsync(string question)
        {
            var model = _cfg["Gemini:Model"] ?? "gemini-1.5-flash";
            var key = _cfg["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini:ApiKey missing");

            var prompt = @$"
أنت Router لروبوت دردشة متجر كتب. أعد فقط JSON صالح بدون أي نص آخر.
المهام:
1) intent ∈ [""summary"",""availability"",""price"",""author_bio"",""more_by_author"",""category_recs"",""similar_to_title"",""general_recs""]
2) entities: title, author, category (قد تكون null).
3) لا تختلق عناوين/أسماء. لو غير متأكد اترك null.
4) language = ""ar"" أو ""en"".
5) confidence رقم 0..1.

أمثلة:
- ""نبذة عن أولاد حارتنا"" → summary + title
- ""نبذة عن زقاق المدق"" → summary + title
- ""نبذة عن كاتب أولاد حارتنا"" → author_bio + title
- ""كتب أخرى لنفس الكاتب نجيب محفوظ"" → more_by_author + author
- ""كتب نفس مؤلف كتاب الخيميائي"" → more_by_author + title
- ""رشّح كتب شبه كتاب أولاد حارتنا"" → similar_to_title + title
- ""هل العادات الذرية متاح؟"" → availability + title
- ""سعر كتاب العادات الذرية"" → price + title

أعد JSON فقط بهذا الشكل:
{{
  ""intent"": ""..."",
  ""entities"": {{ ""title"": ""..."", ""author"": ""..."", ""category"": ""..."" }},
  ""language"": ""ar"",
  ""confidence"": 0.9
}}

السؤال:
{question}";

            var payload = new
            {
                contents = new[] { new { role = "user", parts = new[] { new { text = prompt } } } },
                generationConfig = new
                {
                    temperature = 0.0,
                    maxOutputTokens = 200,
                    // نطلب JSON فقط لتسهيل الـ parsing
                    responseMimeType = "application/json"
                }
            };

            await GeminiConcurrencyGate.Gate.WaitAsync();
            try
            {
                var client = _http.CreateClient("gemini");
                const int maxAttempts = 3;

                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    using var resp = await client.PostAsJsonAsync($"/v1beta/models/{model}:generateContent?key={key}", payload);
                    var sc = (int)resp.StatusCode;

                    // retries على الـ transient errors
                    if ((sc == 429 || sc == 500 || sc == 502 || sc == 503 || sc == 504) && attempt < maxAttempts)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(500 * attempt * attempt));
                        continue;
                    }

                    var body = await resp.Content.ReadAsStringAsync();
                    if (!resp.IsSuccessStatusCode)
                        throw new HttpRequestException($"Gemini route failed: {sc} {resp.StatusCode}. Body: {body}");

                    using var doc = JsonDocument.Parse(body);
                    var text = doc.RootElement.GetProperty("candidates")[0]
                                              .GetProperty("content")
                                              .GetProperty("parts")[0]
                                              .GetProperty("text").GetString() ?? "{}";

                    // بما أننا طلبنا JSON صِرف، نقدر نعمل Deserialize مباشرة
                    var result = System.Text.Json.JsonSerializer.Deserialize<RouteResult>(text) ?? new RouteResult();

                    // طبّع intent على اللائحة المسموحة
                    string? Normalize(string? x)
                    {
                        if (string.IsNullOrWhiteSpace(x)) return null;
                        var ok = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                        {
                            "summary","availability","price","author_bio",
                            "more_by_author","category_recs","similar_to_title","general_recs"
                        };
                        return ok.Contains(x) ? x : null;
                    }
                    result.intent = Normalize(result.intent) ?? "general_recs";
                    return result;
                }

                return new RouteResult(); // shouldn't reach
            }
            catch
            {
                return new RouteResult();
            }
            finally
            {
                GeminiConcurrencyGate.Gate.Release();
            }
        }
    }
}
