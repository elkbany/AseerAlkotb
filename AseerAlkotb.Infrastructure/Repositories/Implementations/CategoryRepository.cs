using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;


namespace AseerAlkotb.Infrastructure.Repositories.Implementations
{
    public class CategoryRepository : GenericRepository<Category, int>, ICategoryRepository
    {
        private readonly ApplicationDbContext context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

    }
}
