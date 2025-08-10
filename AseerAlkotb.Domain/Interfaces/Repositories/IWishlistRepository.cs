using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Base;

namespace AseerAlkotb.Domain.Interfaces.Repositories
{
    public interface IWishlistRepository : IGenericRepository<Wishlist, int>
    {
        Task<Wishlist> GetUserWishlistAsync(int userId);
        Task AddWishlistItemAsync(WishlistItem item);
        Task RemoveWishlistItemAsync(WishlistItem item);
        Task ClearWishlistAsync(int userId);
        Task<bool> IsBookInWishlistAsync(int userId, int bookId);
        Task<int> GetWishlistItemCountAsync(int userId);
    }
}
