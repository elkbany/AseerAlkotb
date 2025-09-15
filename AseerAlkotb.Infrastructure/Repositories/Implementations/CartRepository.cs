using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace AseerAlkotb.Infrastructure.Repositories.Implementations
{
    public class CartRepository : GenericRepository<Cart, int>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<Cart> GetUserCartAsync(int userId)
        {
            return await _dbContext.Cart
                 .Include(cart => cart.CartItems)
                     .ThenInclude(cartitem => cartitem.Book)
                 .FirstOrDefaultAsync(Book => Book.UserId == userId);

        }
        public async Task ClearCartAsync(int userId)
        {
            var cart = await GetUserCartAsync(userId);
            _dbContext.CartItems.RemoveRange(cart.CartItems);
        }

        public async Task AddCartItemAsync(CartItem item)
        {
            await _dbContext.CartItems.AddAsync(item);

        }

        public async Task RemoveCartItemAsync(CartItem item)
        {
            _dbContext.CartItems.Remove(item);

        }

        public async Task UpdateCartItemAsync(CartItem item)
        {
            _dbContext.CartItems.Update(item);

        }

        public async Task<List<Cart>> GetCartsOlderThanAsync(DateTime expirationTime)
        {
            return await _dbContext.Cart
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .Where(c => c.UpdatedAt < expirationTime && c.CartItems.Any())
                .ToListAsync();
        }

    }

}
