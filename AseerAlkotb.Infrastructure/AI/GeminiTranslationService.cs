using System.Net.Http.Json;
using System.Text.Json;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Utils;
using Microsoft.Extensions.Configuration;

namespace AseerAlkotb.Infrastructure.AI
{
    public class GeminiTranslationService : ITranslationService
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _cfg;

        public GeminiTranslationService(IHttpClientFactory http, IConfiguration cfg)
        {
            _http = http;
            _cfg = cfg;
        }

        public async Task<string> TranslateToArabicAsync(string englishText)
        {
            if (string.IsNullOrWhiteSpace(englishText)) return string.Empty;

            var prompt = $@"
Translate the following English text to natural Arabic. Keep the same meaning and context.
Only return the Arabic translation, nothing else.

English text:
{englishText}

Arabic translation:";

            return await CallGeminiAsync(prompt);
        }

        public async Task<string> TranslateToEnglishAsync(string arabicText)
        {
            if (string.IsNullOrWhiteSpace(arabicText)) return string.Empty;

            var prompt = $@"
Translate the following Arabic text to natural English. Keep the same meaning and context.
Only return the English translation, nothing else.

- Book titles: ""بين القصرين"" → ""Bayn al-Qasrayn"" (NOT ""Between the Palaces"")
- Author names: ""نجيب محفوظ"" → ""Naguib Mahfouz"" (NOT translated)
- Publisher names: ""دار الشروق"" → ""Dar al-Shorouk"" (NOT ""House of Sunrise"")
- Regular text: Still translated normally for descriptions and content

IMPORTANT RULES:
- For book titles, author names, and publisher names: TRANSLITERATE (write in Latin script preserving Arabic pronunciation) instead of translating.
- Only translate regular descriptive text and keep proper nouns transliterated.

Arabic text:
{arabicText}

English translation:";

            return await CallGeminiAsync(prompt);
        }

        public async Task<bool> IsEnglishTextAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            // Use the existing LangUtils for quick detection
            var detectedLang = LangUtils.Detect(text);
            return detectedLang == Lang.English;
        }

        private async Task<string> CallGeminiAsync(string prompt)
        {
            var model = _cfg["Gemini:Model"] ?? "gemini-1.5-flash";
            var key = _cfg["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini:ApiKey missing");

            var payload = new
            {
                contents = new[] { new { role = "user", parts = new[] { new { text = prompt } } } },
                generationConfig = new
                {
                    temperature = 0.1,
                    maxOutputTokens = 1000
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

                    if ((sc == 429 || sc == 500 || sc == 502 || sc == 503 || sc == 504) && attempt < maxAttempts)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(500 * attempt * attempt));
                        continue;
                    }

                    if (!resp.IsSuccessStatusCode)
                    {
                        var errBody = await resp.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Gemini translation failed: {sc} {resp.StatusCode}. Body: {errBody}");
                    }

                    using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                    if (!doc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
                        return string.Empty;

                    var parts = candidates[0].GetProperty("content").GetProperty("parts");
                    if (parts.GetArrayLength() == 0) return string.Empty;

                    return parts[0].GetProperty("text").GetString()?.Trim() ?? string.Empty;
                }

                return string.Empty;
            }
            finally
            {
                GeminiConcurrencyGate.Gate.Release();
            }
        }
    }
}