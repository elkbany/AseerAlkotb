using AseerAlkotb.Domain.Entites.Models;

namespace AseerAlkotb.Application.Contracts
{
    public interface IEmbeddingService
    {
        Task<float[]> GenerateEmbeddingAsync(string text);
        Task UpdateBookEmbeddingsAsync(int bookId);
        Task DeleteBookEmbeddingsAsync(int bookId);
        Task<List<BookEmbedding>> GetBookEmbeddingsAsync(int bookId);
        Task<List<BookEmbedding>> GetAllEmbeddingsAsync();
        Task<List<BookEmbedding>> SearchSimilarBooksAsync(string query, int topK = 8);
    }
}
