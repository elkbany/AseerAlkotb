using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;

namespace AseerAlkotb.Domain.Interfaces.Repositories
{
    public interface ICityRepository : IGenericRepository<City, int>
    {
        Task<List<City>> GetCitiesByGovernorateAsync(int governorateId);
    }
}