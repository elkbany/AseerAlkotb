using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace AseerAlkotb.Infrastructure.Repositories.Implementations
{
    public class WishlistRepository : GenericRepository<Wishlist, int>, IWishlistRepository
    {
        public WishlistRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<Wishlist> GetUserWishlistAsync(int userId)
        {
            return await _dbContext.Wishlists
           .Include(w => w.WishlistItems)
               .ThenInclude(wi => wi.Book)
           .FirstOrDefaultAsync(w => w.UserId == userId);
        }
        public async Task AddWishlistItemAsync(WishlistItem item)
        {
            await _dbContext.WishlistItems.AddAsync(item);
        }
        public async Task RemoveWishlistItemAsync(WishlistItem item)
        {
             _dbContext.WishlistItems.Remove(item);

        }
        public async Task ClearWishlistAsync(int userId)
        {
            var wishlist = await GetUserWishlistAsync(userId);
            _dbContext.WishlistItems.RemoveRange(wishlist.WishlistItems);
        }
        public async Task<bool> IsBookInWishlistAsync(int userId, int bookId)
        {
            return await _dbContext.WishlistItems
          .AnyAsync(wi => wi.Wishlist.UserId == userId && wi.BookId == bookId);
        }
        public async Task<int> GetWishlistItemCountAsync(int userId)
        {
            return await _dbContext.WishlistItems
                .CountAsync(wi => wi.Wishlist.UserId == userId);
        }
    }
}
