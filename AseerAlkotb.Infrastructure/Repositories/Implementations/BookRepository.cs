using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Infrastructure.Repositories.Implementations
{
    public class BookRepository : GenericRepository<Book, int>, IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext Context) : base(Context)
        {
            _context = Context;
        }

        public IQueryable<Book> GetQueryable()
        {
            return _context.Books.AsQueryable();
        }
        public async Task<List<Book>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _dbContext.Books
                .Where(b => ids.Contains(b.Id))
                .ToListAsync();
        }

    }
}
