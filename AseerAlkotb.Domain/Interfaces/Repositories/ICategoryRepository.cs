using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;

namespace AseerAlkotb.Domain.Interfaces.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category, int>
    {
    }
}