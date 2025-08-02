using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
