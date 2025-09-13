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
- ""نبذة عن كاتب أولاد حارتنا"" → author_bio + title
- ""كتب أخرى لنفس الكاتب نجيب محفوظ"" → more_by_author + author
- ""كتب نفس مؤلف كتاب الخيميائي"" → more_by_author + title
- ""رشّح كتب شبه كتاب أولاد حارتنا"" → similar_to_title + title
- ""هل العادات الذرية متاح؟"" → availability + title
- ""سعر كتاب العادات الذرية"" → price + title
- ""ترشيحات في تصنيف روايات"" → category_recs + category

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
                generationConfig = new { temperature = 0.0, maxOutputTokens = 200 }
            };

            await GeminiConcurrencyGate.Gate.WaitAsync();
            try
            {
                var client = _http.CreateClient("gemini");
                using var resp = await client.PostAsJsonAsync($"/v1beta/models/{model}:generateContent?key={key}", payload);
                var sc = (int)resp.StatusCode;

                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Gemini failed: {sc} {resp.StatusCode}. Body: {body}");
                }


                using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                var text = doc.RootElement.GetProperty("candidates")[0]
                                          .GetProperty("content")
                                          .GetProperty("parts")[0]
                                          .GetProperty("text").GetString() ?? "{}";

                var start = text.IndexOf('{'); var end = text.LastIndexOf('}');
                var json = (start >= 0 && end >= start) ? text.Substring(start, end - start + 1) : "{}";

                return System.Text.Json.JsonSerializer.Deserialize<RouteResult>(json) ?? new RouteResult();
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
