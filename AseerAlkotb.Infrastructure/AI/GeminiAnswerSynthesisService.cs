using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Rag.Responses;
using Microsoft.Extensions.Configuration;
namespace AseerAlkotb.Infrastructure.AI
{
    public class GeminiAnswerSynthesisService : IAnswerSynthesisService
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _cfg;

        public GeminiAnswerSynthesisService(IHttpClientFactory http, IConfiguration cfg)
        {
            _http = http; _cfg = cfg;
        }

        public async Task<string> SynthesizeAsync(string question, List<ChatSource> sources)
        {
            var q = question?.Trim() ?? string.Empty;
            bool looksEnglish = System.Text.RegularExpressions.Regex.IsMatch(q, @"[A-Za-z]");

            string prompt = looksEnglish
                ? """
          You are a concise assistant. Summarize clearly in simple English.
          If the question is about a book, give the central idea and main themes without fluff.

          Question:
          """ + q
                : """
          أنت مساعد مختصر وواضح. لخّص المطلوب بدقة وبالعربية المبسّطة.
          إن كان السؤال عن كتاب، اذكر الفكرة العامة وأهم المحاور بدون إطالة.

          السؤال:
          """ + q;

            var model = _cfg["Gemini:Model"] ?? "gemini-1.5-flash";
            var key = _cfg["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini:ApiKey missing");

            var payload = new
            {
                contents = new[]
                {
            new {
                role = "user",
                parts = new[] { new { text = prompt } }
            }
        }
            };

            var client = _http.CreateClient("gemini");

            const int maxAttempts = 3;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                using var resp = await client.PostAsJsonAsync($"/v1beta/models/{model}:generateContent?key={key}", payload);

                if ((int)resp.StatusCode == 429 && attempt < maxAttempts)
                {
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)));
                    continue;
                }

                if (!resp.IsSuccessStatusCode)
                {
                    var errBody = await resp.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Gemini generateContent failed: {(int)resp.StatusCode} {resp.StatusCode}. Body: {errBody}");
                }

                using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                if (!doc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
                    return string.Empty;

                var parts = candidates[0].GetProperty("content").GetProperty("parts");
                if (parts.GetArrayLength() == 0) return string.Empty;

                return parts[0].GetProperty("text").GetString() ?? string.Empty;
            }

            return string.Empty;
        }

    }
}
