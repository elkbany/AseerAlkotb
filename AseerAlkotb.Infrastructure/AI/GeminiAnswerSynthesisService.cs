using System.Net.Http.Json;
using System.Text.Json;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Features.Rag.Responses;
using AseerAlkotb.Application.Utils;
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
            int contextChars = sources?.Sum(s => (s.Snippet ?? string.Empty).Length) ?? 0;
            bool hasRichContext = contextChars >= 160;
            var lang = LangUtils.Detect(q);

            string BuildSourcesBlock(List<ChatSource> srcs)
            {
                if (srcs == null || srcs.Count == 0) return "- (no snippets).";
                var take = Math.Min(8, srcs.Count);
                const int MAX_SNIPPET = 2000;
                var lines = new List<string>(take);
                for (int i = 0; i < take; i++)
                {
                    var s = srcs[i];
                    var title = string.IsNullOrWhiteSpace(s.Title) ? "Untitled" : s.Title.Trim();
                    var snippet = (s.Snippet ?? "").Trim();
                    if (snippet.Length > MAX_SNIPPET) snippet = snippet[..MAX_SNIPPET] + "...";
                    lines.Add($"- {title}{(string.IsNullOrWhiteSpace(snippet) ? "" : $": {snippet}")}");
                }
                return string.Join("\n", lines);
            }

            var sourcesBlock = BuildSourcesBlock(sources ?? new List<ChatSource>());

            string prompt = lang == Lang.English
                ? (hasRichContext
                    ? $@"You are a friendly, concise assistant for Aseer Alkotb (عصير الكتب).
- Write a 3–5 bullet summary directly from the CONTEXT below.
- Do NOT say the context is insufficient when a description is present.

USER QUESTION:
{q}

CONTEXT:
{sourcesBlock}

Answer in simple English:"
                    : $@"You are a friendly, concise assistant for Aseer Alkotb (عصير الكتب).
- Summarize or answer briefly based on general knowledge if needed.

USER QUESTION:
{q}

Answer in simple English:")
                : (hasRichContext
                    ? $@"أنت مساعد ودود وواضح لمنصّة عصير الكتب (Aseer Alkotb).
- اكتب ملخّصًا من 3–5 نقاط اعتمادًا على «السياق» أدناه مباشرة.
- لا تقل إن السياق غير كافٍ عند وجود وصف.

سؤال المستخدم:
{q}

السياق:
{sourcesBlock}

الجواب بالعربية المبسّطة:"
                    : $@"أنت مساعد ودود وواضح لمنصّة عصير الكتب (Aseer Alkotb).
- لخّص/أجب باختصار، ويمكنك استخدام معرفتك العامة عند الحاجة.

سؤال المستخدم:
{q}

الجواب بالعربية المبسّطة:");

            var model = _cfg["Gemini:Model"] ?? "gemini-1.5-flash";
            var key = _cfg["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini:ApiKey missing");

            var payload = new
            {
                contents = new[] { new { role = "user", parts = new[] { new { text = prompt } } } },
                generationConfig = new { temperature = 0.2, topP = 0.95, maxOutputTokens = 350 }
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
                        throw new HttpRequestException($"Gemini generateContent failed: {sc} {resp.StatusCode}. Body: {errBody}");
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
            finally
            {
                GeminiConcurrencyGate.Gate.Release();
            }
        }
    }
}
