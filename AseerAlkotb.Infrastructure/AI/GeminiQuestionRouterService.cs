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
            var model = _cfg["Gemini:Model"] ?? null;
            var key = _cfg["Gemini:ApiKey"] ?? null;

            var prompt = @$"
أنت Router ذكي لروبوت دردشة متجر كتب عربي. مهمتك تحديد النية واستخراج الكيانات بدقة عالية.

أعد فقط JSON صالح بدون أي نص آخر.

المهام:
1) حدد intent من: [""summary"",""availability"",""price"",""author_bio"",""more_by_author"",""category_recs"",""similar_to_title"",""publisher_info"",""publisher_books"",""general_recs""]
2) استخرج entities بدقة: title, author, category, publisher (اتركها null لو مش متأكد)
3) لا تختلق عناوين أو أسماء. لو مش واضح 100% اترك null
4) حدد language = ""ar"" أو ""en""
5) أعطِ confidence من 0 إلى 1

أمثلة تفصيلية:

الملخصات:
- ""نبذة عن أولاد حارتنا"" → summary + title:""أولاد حارتنا""
- ""ملخص كتاب زقاق المدق"" → summary + title:""زقاق المدق""
- ""إيه قصة رواية الأسود يليق بك"" → summary + title:""الأسود يليق بك""

نبذة المؤلف:
- ""نبذة عن كاتب أولاد حارتنا"" → author_bio + title:""أولاد حارتنا""
- ""معلومات عن نجيب محفوظ"" → author_bio + author:""نجيب محفوظ""
- ""مين مؤلف كتاب الخيميائي"" → author_bio + title:""الخيميائي""

كتب المؤلف:
- ""كتب أخرى لنفس الكاتب نجيب محفوظ"" → more_by_author + author:""نجيب محفوظ""
- ""كتب نفس مؤلف كتاب الخيميائي"" → more_by_author + title:""الخيميائي""
- ""إيه الكتب التانية لـ أحمد خالد توفيق"" → more_by_author + author:""أحمد خالد توفيق""

الترشيحات المشابهة:
- ""رشّح كتب شبه كتاب أولاد حارتنا"" → similar_to_title + title:""أولاد حارتنا""
- ""كتب مثل هاري بوتر"" → similar_to_title + title:""هاري بوتر""

التوافر والسعر:
- ""هل العادات الذرية متاح؟"" → availability + title:""العادات الذرية""
- ""سعر كتاب العادات الذرية"" → price + title:""العادات الذرية""
- ""كام سعر أولاد حارتنا"" → price + title:""أولاد حارتنا""

ترشيحات التصنيف:
- ""رشّح كتب روايات"" → category_recs + category:""روايات""
- ""كتب تطوير ذات"" → category_recs + category:""تطوير ذات""

ملاحظات مهمة:
- استخرج أسماء الكتب بدقة (أزل ""كتاب"" و""رواية"" من البداية)
- أسماء المؤلفين يجب أن تكون واضحة ومحددة
- لو السؤال غامض أو عام حاول تجيب النية صح ولو معرفتش → general_recs
- confidence عالي (0.8+) للأسئلة الواضحة، منخفض للغامضة

أعد JSON فقط بهذا الشكل:
{{
  ""intent"": ""..."",
  ""entities"": {{ ""title"": ""..."", ""author"": ""..."", ""category"": ""..."", ""publisher"": ""..."" }},
  ""language"": ""ar"",
  ""confidence"": 0.9
}}

دور النشر (جديد):
- ""معلومات عن دار الشروق"" → publisher_info + publisher:""دار الشروق""
- ""نبذة عن منشورات عصير الكتب"" → publisher_info + publisher:""عصير الكتب""
- ""الكتاب دا من أي دار نشر؟"" → publisher_info + title:""[عنوان الكتاب]""
- ""مين الناشر بتاع رواية زقاق المدق؟"" → publisher_info + title:""زقاق المدق""

كتب دار النشر:
- ""كتب دار الشروق"" → publisher_books + publisher:""دار الشروق""
- ""إيه الكتب الموجودة في عصير الكتب؟"" → publisher_books + publisher:""عصير الكتب""

السؤال:
{question}";

            var payload = new
            {
                contents = new[] { new { role = "user", parts = new[] { new { text = prompt } } } },
                generationConfig = new
                {
                    temperature = 0.0,
                    maxOutputTokens = 200,
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

                    var result = System.Text.Json.JsonSerializer.Deserialize<RouteResult>(text) ?? new RouteResult();

                    string? Normalize(string? x)
                    {
                        if (string.IsNullOrWhiteSpace(x)) return null;
                        var ok = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                        {
                            "summary","availability","price","author_bio",
                            "more_by_author","category_recs","similar_to_title",
                            "publisher_info","publisher_books","general_recs"
                        };
                        return ok.Contains(x) ? x : null;
                    }
                    result.intent = Normalize(result.intent) ?? "general_recs";
                    return result;
                }

                return new RouteResult();
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
