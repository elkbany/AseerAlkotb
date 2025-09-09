using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;

namespace AseerAlkotb.Infrastructure.Repositories.Implementations
{
    public class BookEmbeddingRepository : GenericRepository<BookEmbedding, int>, IBookEmbeddingRepository
    {
        public BookEmbeddingRepository(ApplicationDbContext db) : base(db) { }

    }
}
