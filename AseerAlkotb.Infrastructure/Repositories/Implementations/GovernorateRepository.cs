using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace AseerAlkotb.Infrastructure.Repositories.Implementations
{
    public class GovernorateRepository : GenericRepository<Governorate, int>, IGovernorateRepository
    {
        private readonly ApplicationDbContext context;

        public GovernorateRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Governorate>> GetAllGovernoratesAsync()
        {
            return await context.Governorates
                                .OrderBy(g => g.Name)
                                .ToListAsync();
        }
    }
}