using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Entites.Models;

namespace AseerAlkotb.Domain.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<User> GetUserWithRelatedData(int userId);
    }
}
