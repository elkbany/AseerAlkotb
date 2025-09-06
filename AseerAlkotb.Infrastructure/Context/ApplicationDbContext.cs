﻿using AseerAlkotb.Domain.Entites;
using AseerAlkotb.Domain.Entites.Base;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using AseerAlkotb.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>,int>
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

            modelBuilder.Entity<Review>().ToTable(r => r.HasCheckConstraint("CK_Review_OneTarget",
                      "(BookId IS NOT NULL AND AuthorId IS NULL) OR (BookId IS NULL AND AuthorId IS NOT NULL)"));

            // Payment-Order relationship is configured in OrderConfiguration.cs
            // No need to configure it here to avoid conflicts

            //modelBuilder.Entity<Cart>()
            //    .HasOne(c => c.User)
            //    .WithOne(u => u.Cart).HasForeignKey<User>("CartId")
            //    .OnDelete(DeleteBehavior.Cascade);
            //modelBuilder.Entity<WishlistItem>().HasKey(wi => new { wi.BookId, wi.WishlistId });

            //modelBuilder.Entity<Cart>().HasData(
            //  new Cart
            //  {
            //      Id = 1,
            //      UserId = 1,
            //      CreatedAt = fixedDate,
            //      UpdatedAt = fixedDate

            ////  }
            //);

            //modelBuilder.Entity<User>().HasData(
            //    new User
            //    {
            //        Id = 1,
            //        FirstName = "Ahmed",
            //        LastName = "Hassan",
            //        DateOfBirth = dateOfBirth,
            //        Gender = Gender.Male,
            //        IsActive = true,
            //        CartId = 1,
            //        CreatedAt = fixedDate,
            //        UpdatedAt = fixedDate

            //    }
            //);

        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Cart>Cart { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<LikeDisLike> LikeDisLikes { get; set; }
        public override DbSet<User> Users { get; set; }
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }
        #region Commented int version 
        //private void UpdateTimestamps()
        //{
        //    var entries = ChangeTracker.Entries<Entity<int>>();

        //    foreach (var entry in entries)
        //    {
        //        switch (entry.State)
        //        {
        //            case EntityState.Added:
        //                entry.Entity.CreatedAt = DateTime.UtcNow;
        //                entry.Entity.UpdatedAt = DateTime.UtcNow;
        //                break;

        //            case EntityState.Modified:
        //                entry.Entity.UpdatedAt = DateTime.UtcNow;
        //                // Prevent CreatedAt from being updated
        //                entry.Property(nameof(Entity<int>.CreatedAt)).IsModified = false;
        //                break;
        //        }
        //    }
        //} 
        #endregion
        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity.GetType().BaseType?.IsGenericType == true &&
                           e.Entity.GetType().BaseType.GetGenericTypeDefinition() == typeof(Entity<>));

            foreach (var entry in entries)
            {
                var entity = (dynamic)entry.Entity;
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreatedAt = DateTime.UtcNow;
                        entity.UpdatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entity.UpdatedAt = DateTime.UtcNow;
                        entry.Property("CreatedAt").IsModified = false;
                        break;
                }
            }
        }
    }
}
