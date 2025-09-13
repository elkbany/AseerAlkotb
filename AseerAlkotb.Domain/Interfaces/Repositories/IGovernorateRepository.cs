using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;

namespace AseerAlkotb.Domain.Interfaces.Repositories
{
    public interface IGovernorateRepository : IGenericRepository<Governorate, int>
    {
        Task<List<Governorate>> GetAllGovernoratesAsync();
    }
}