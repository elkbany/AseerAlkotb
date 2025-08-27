using AseerAlkotb.Domain.Interfaces.Base;
using AseerAlkotb.Domain.Interfaces.Repositories;
using AseerAlkotb.Infrastructure.Context;
using AseerAlkotb.Infrastructure.Repositories.Implementations;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Infrastructure.Repositories.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext dbContext;

        // Properties - كل property مرة واحدة بس
        public IAuthorRepository Authors { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IBookRepository Books { get; private set; }
        public ICartRepository Carts { get; private set; }
        public IReviewRepository Reviews { get; private set; }
        public IQuoteRepository Quotes { get; private set; }
        public IWishlistRepository Wishlists { get; private set; }
        public IPublisherRepository Publishers { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IPaymentRepository Payments { get; private set; }
        public INotificationRepository Notifications { get; private set; }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;

            // Initialize all repositories
            Authors = new AuthorRepository(dbContext);
            Categories = new CategoryRepository(dbContext);
            Books = new BookRepository(dbContext);
            Orders = new OrderRepository(dbContext);
            Carts = new CartRepository(dbContext);
            Publishers = new PublisherRepository(dbContext);
            Reviews = new ReviewRepository(dbContext);
            Quotes = new QuoteRepository(dbContext);
            Wishlists = new WishlistRepository(dbContext);
            Payments = new PaymentRepository(dbContext);
            Notifications = new NotificationRepository(dbContext);
        }

        public async Task<int> CommitAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}