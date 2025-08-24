using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;

namespace AseerAlkotb.Domain.Interfaces.Repositories
{
    public interface ICartRepository : IGenericRepository<Cart,int>
    {
        Task<Cart> GetUserCartAsync(int userId);

        Task AddCartItemAsync(CartItem item);


        Task RemoveCartItemAsync(CartItem item);


        Task UpdateCartItemAsync(CartItem item);


        Task ClearCartAsync(int userId);

    }
}
