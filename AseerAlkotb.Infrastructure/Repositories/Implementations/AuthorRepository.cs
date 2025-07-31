using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;


namespace AseerAlkotb.Infrastructure.Repositories.Implementations
{
    public class AuthorRepository : GenericRepository<Author, int>, IAuthorRepository
    {
        private readonly ApplicationDbContext context;

        public AuthorRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

    }
}

