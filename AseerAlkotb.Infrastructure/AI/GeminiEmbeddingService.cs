using System.Net.Http.Json;
using System.Text.Json;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Application.Utils;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AseerAlkotb.Infrastructure.AI
{
    public class GeminiEmbeddingService : IEmbeddingService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _cfg;

        private static readonly Dictionary<string, float[]> _embedCache = new(StringComparer.Ordinal);

        public GeminiEmbeddingService(ApplicationDbContext db, IHttpClientFactory http, IConfiguration cfg)
        {
            _db = db; _http = http; _cfg = cfg;
        }

        // ----- Core HTTP -----
        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            await GeminiConcurrencyGate.Gate.WaitAsync();
            try
            {
                var client = _http.CreateClient("gemini");
                var key = _cfg["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini:ApiKey missing");
                var model = _cfg["Gemini:EmbeddingModel"] ?? "text-embedding-004";

                var url = $"/v1beta/models/{model}:embedContent?key={key}";
                var payload = new
                {
                    model = $"models/{model}",
                    content = new { parts = new[] { new { text = text ?? string.Empty } } }
                };

                const int maxAttempts = 3;
                for (int attempt = 1; attempt <= maxAttempts; attempt++)
                {
                    using var resp = await client.PostAsJsonAsync(url, payload);
                    var sc = (int)resp.StatusCode;

                    if (!resp.IsSuccessStatusCode)
                    {
                        if ((sc == 429 || sc >= 500) && attempt < maxAttempts)
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(400 * attempt * attempt));
                            continue;
                        }
                        var err = await resp.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Gemini embed failed: {sc} {resp.StatusCode}. Body: {err}");
                    }

                    using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                    var values = doc.RootElement.GetProperty("embedding").GetProperty("values");

                    var vec = new float[values.GetArrayLength()];
                    int i = 0;
                    foreach (var v in values.EnumerateArray()) vec[i++] = (float)v.GetDouble();
                    return vec;
                }

                return Array.Empty<float>(); // should not reach
            }
            finally
            {
                GeminiConcurrencyGate.Gate.Release();
            }
        }

        private async Task<float[]> EmbedWithCacheAsync(string content)
        {
            var key = content.Trim();
            if (_embedCache.TryGetValue(key, out var cached)) return cached;
            var vec = await GenerateEmbeddingAsync(key);
            _embedCache[key] = vec;
            return vec;
        }

        private static string BuildEmbeddingQuery(string? query)
        {
            query ??= string.Empty;
            var (title, author) = QueryExtractor.Extract(query);

            if (!string.IsNullOrWhiteSpace(author) && string.IsNullOrWhiteSpace(title))
                return author!.Trim();

            if (!string.IsNullOrWhiteSpace(title))
                return title!.Trim();

            return query.Trim();
        }

        public async Task<List<BookEmbedding>> SearchSimilarBooksAsync(string query, int topK = 8)
        {
            float[] qv;
            try
            {
                var qText = BuildEmbeddingQuery(query);
                qv = await EmbedWithCacheAsync(qText); // كمان كاش للكويري
            }
            catch { return new List<BookEmbedding>(); }

            var dim = qv.Length;
            var all = (await GetAllEmbeddingsAsync())
                      .Where(e => e.Embedding != null && e.Embedding.Length == dim)
                      .ToList();

            return all.Select(e => new { e, sim = Cos(qv, e.Embedding) })
                      .OrderByDescending(x => x.sim)
                      .Take(topK * 3)
                      .GroupBy(x => x.e.BookId)
                      .Select(g => g.OrderByDescending(x => x.sim).First().e)
                      .Take(topK)
                      .ToList();
        }

        public async Task<List<BookEmbedding>> GetBookEmbeddingsAsync(int bookId)
            => await _db.BookEmbeddings.AsNoTracking().Where(e => e.BookId == bookId).ToListAsync();

        public async Task<List<BookEmbedding>> GetAllEmbeddingsAsync()
            => await _db.BookEmbeddings.AsNoTracking()
                .Include(e => e.Book).ThenInclude(b => b.Author)
                .ToListAsync();

        public async Task UpdateBookEmbeddingsAsync(int bookId)
        {
            var book = await _db.Books
                                .Include(b => b.Author)
                                .Include(b => b.Categories)
                                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book is null) return;

            var chunks = ChunkFactory.BuildBookChunks(book);

            // إعداد يسمح بتجاهل author_bio داخل كل كتاب (لتقليل التكرار)
            bool includeAuthorBioInBook = string.Equals(_cfg["Embeddings:IncludeAuthorBioInBookChunks"], "true", StringComparison.OrdinalIgnoreCase);

            var existing = await _db.BookEmbeddings.Where(e => e.BookId == bookId).ToListAsync();
            _db.BookEmbeddings.RemoveRange(existing);

            foreach (var c in chunks)
            {
                if (string.IsNullOrWhiteSpace(c.Content)) continue;
                if (!includeAuthorBioInBook && c.Type == "author_bio") continue;

                var vec = await EmbedWithCacheAsync(c.Content);
                _db.BookEmbeddings.Add(new BookEmbedding
                {
                    BookId = bookId,
                    ContentType = c.Type,
                    Content = c.Content,
                    Embedding = vec,
                    LastUpdated = DateTime.UtcNow
                });

                await Task.Delay(50); // تهدئة بسيطة
            }

            await _db.SaveChangesAsync();
        }

        public async Task DeleteBookEmbeddingsAsync(int bookId)
        {
            var rows = await _db.BookEmbeddings.Where(e => e.BookId == bookId).ToListAsync();
            if (rows.Count == 0) return;
            _db.BookEmbeddings.RemoveRange(rows);
            await _db.SaveChangesAsync();
        }

        private static double Cos(float[] a, float[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return 0;
            double dot = 0, na = 0, nb = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                na += a[i] * a[i];
                nb += b[i] * b[i];
            }
            if (na == 0 || nb == 0) return 0;
            return dot / (Math.Sqrt(na) * Math.Sqrt(nb));
        }
    }
}
