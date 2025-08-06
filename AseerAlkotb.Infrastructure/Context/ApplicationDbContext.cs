using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
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
            var fixedDate = new DateTime(2024, 8, 1); // fixed date for consistency
            var dateOfBirth = new DateTime(1990, 5, 15); // Example date of birth for user

            base.OnModelCreating(modelBuilder);

            DataSeeder.SeedData(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            modelBuilder.Entity<Review>().ToTable(t=>t.HasCheckConstraint("CK_Review_Rating", "Rating >= 1 AND Rating <= 5"));

            modelBuilder.Entity<Review>().ToTable(r => r.HasCheckConstraint("CK_Review_OneTarget",
                      "(BookId IS NOT NULL AND AuthorId IS NULL) OR (BookId IS NULL AND AuthorId IS NOT NULL)"));

            modelBuilder.Entity<Cart>()
                .HasOne(c=>c.User)
                .WithOne(u=>u.Cart).HasForeignKey<User>("CartId")
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<WishlistItem>().HasKey(wi => new { wi.BookId, wi.WishlistId });

            modelBuilder.Entity<Cart>().HasData(
              new Cart
              {
                  Id = 1,
                  UserId = 1,
                  CreatedAt = fixedDate,
                  UpdatedAt = fixedDate

              }
          );

            modelBuilder.Entity<User>().HasData(
    new User
    {
        Id = 1,
        FirstName = "Ahmed",
        LastName = "Hassan",
        DateOfBirth = dateOfBirth,
        Gender = Gender.Male,
        IsActive = true,
        CartId = 1,
        CreatedAt = fixedDate,
        UpdatedAt = fixedDate

    }
);

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
