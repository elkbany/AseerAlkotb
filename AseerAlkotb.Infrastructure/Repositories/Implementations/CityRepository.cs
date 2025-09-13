using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace AseerAlkotb.Infrastructure.Repositories.Implementations
{
    public class CityRepository : GenericRepository<City, int>, ICityRepository
    {
        private readonly ApplicationDbContext context;

        public CityRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<City>> GetCitiesByGovernorateAsync(int governorateId)
        {
            return await context.Cities
                                .Where(c => c.GovernorateId == governorateId)
                                .OrderBy(c => c.Name)
                                .ToListAsync();
        }
    }
}