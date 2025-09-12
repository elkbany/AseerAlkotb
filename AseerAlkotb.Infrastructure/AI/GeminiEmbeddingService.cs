using System.Net.Http.Json;
using System.Text.Json;
using AseerAlkotb.Application.Contracts;
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

        public GeminiEmbeddingService(ApplicationDbContext db, IHttpClientFactory http, IConfiguration cfg)
        {
            _db = db; _http = http; _cfg = cfg;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
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

            using var resp = await client.PostAsJsonAsync(url, payload);

            // لو حصل خطأ، اطبع لك البودي عشان تعرف السبب الحقيقي
            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Gemini embed  failed: {(int)resp.StatusCode} {resp.StatusCode}. Body: {err}");
            }

            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var values = doc.RootElement.GetProperty("embedding").GetProperty("values");

            var vec = new float[values.GetArrayLength()];
            int i = 0;
            foreach (var v in values.EnumerateArray()) vec[i++] = (float)v.GetDouble();
            return vec;
        }


        public async Task<List<BookEmbedding>> SearchSimilarBooksAsync(string query, int topK = 5)
        {
            var qv = await GenerateEmbeddingAsync(query ?? "");
            var all = await GetAllEmbeddingsAsync();

            return all.Select(e => new { e, sim = Cos(qv, e.Embedding) })
                      .OrderByDescending(x => x.sim)
                      .Take(topK * 3)
                      .GroupBy(x => x.e.BookId)
                      .Select(g => g.OrderByDescending(x => x.sim).First().e)
                      .Take(topK)
                      .ToList();
        }

        public async Task<List<BookEmbedding>> GetBookEmbeddingsAsync(int bookId)
            => await _db.BookEmbeddings.Where(e => e.BookId == bookId).ToListAsync();

        public async Task UpdateBookEmbeddingsAsync(int bookId)
        {
            var book = await _db.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null) return;

            var chunks = new List<(string Type, string Content)>
            {
                ("title", book.Title ?? ""),
                ("description", book.Description ?? ""),
                ("author", book.Author?.Name ?? ""),
                ("category", string.Join(", ", book.Categories?.Select(c => c.Name) ?? Enumerable.Empty<string>()))
            }
            .Where(x => !string.IsNullOrWhiteSpace(x.Content))
            .ToList();

            var existing = await _db.BookEmbeddings.Where(e => e.BookId == bookId).ToListAsync();
            _db.BookEmbeddings.RemoveRange(existing);

            foreach (var c in chunks)
            {
                var vec = await GenerateEmbeddingAsync(c.Content);
                _db.BookEmbeddings.Add(new BookEmbedding
                {
                    BookId = bookId,
                    ContentType = c.Type,
                    Content = c.Content,
                    Embedding = vec,
                    LastUpdated = DateTime.UtcNow
                });
            }

            await _db.SaveChangesAsync();
        }

        public async Task DeleteBookEmbeddingsAsync(int bookId)
        {
            var rows = await _db.BookEmbeddings.Where(e => e.BookId == bookId).ToListAsync();
            _db.BookEmbeddings.RemoveRange(rows);
            await _db.SaveChangesAsync();
        }

        public async Task<List<BookEmbedding>> GetAllEmbeddingsAsync()
    => await _db.BookEmbeddings
                .AsNoTracking()
                .Include(e => e.Book)
                    .ThenInclude(b => b.Author) 
                .ToListAsync();


        private static double Cos(float[] a, float[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return 0;
            double dot = 0, na = 0, nb = 0;
            for (int i = 0; i < a.Length; i++) { dot += a[i] * b[i]; na += a[i] * a[i]; nb += b[i] * b[i]; }
            if (na == 0 || nb == 0) return 0;
            return dot / (Math.Sqrt(na) * Math.Sqrt(nb));
        }
    }
}
