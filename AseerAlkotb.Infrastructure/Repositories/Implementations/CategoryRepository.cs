using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;


namespace AseerAlkotb.Infrastructure.Repositories.Implementations
{
    public class CategoryRepository : GenericRepository<Category, int>, ICategoryRepository
    {
        private readonly ApplicationDbContext context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Category>> GetSubCategoriesAsync(int parentCategoryId)
        {
            return await context.Categories
                                 .Where(c => c.ParentCategoryId == parentCategoryId)
                                 .ToListAsync();
        }

    }
}
