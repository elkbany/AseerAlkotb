using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AseerAlkotb.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            DataSeeder.SeedData(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<Review>().ToTable(t=>t.HasCheckConstraint("CK_Review_Rating", "Rating >= 1 AND Rating <= 5"));
            modelBuilder.Entity<Cart>()
                .HasOne(c=>c.User)
                .WithOne(u=>u.Cart).HasForeignKey<User>("CartId")
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<WishlistItem>().HasKey(wi => new { wi.BookId, wi.WishlistId });

        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }

    }
}
