using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace AseerAlkotb.Infrastructure.Data
{
    public class DataSeeder
    {
        public static void SeedData(ModelBuilder modelBuilder)
        {
            var fixedDate = new DateTime(2024, 8, 1); // fixed date for consistency

            // Authors
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, Name = "نجيب محفوظ", Bio = "أديب مصري", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Author { Id = 2, Name = "أحمد خالد توفيق", Bio = "كاتب مصري", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Author { Id = 3, Name = "يوسف إدريس", Bio = "كاتب مصري", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate }

            );

            // Publishers
            modelBuilder.Entity<Publisher>().HasData(
                new Publisher { Id = 1, Name = "عصير الكتب", Description = "دار نشر مصرية", ContactEmail = "info@aseeralkotob.com", CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Publisher { Id = 2, Name = "دار الشروق", Description = "دار نشر عربية", ContactEmail = "info2@aseeralkotob.com", CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Publisher { Id = 3, Name = "دار الساقي", Description = "دار نشر لبنانية", ContactEmail = "info3@aseeralkotob.com", CreatedAt = fixedDate, UpdatedAt = fixedDate }
            );

            // Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "روايات", Description = "كتب روائية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 2, Name = "تاريخ", Description = "كتب تاريخية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 3, Name = "علوم", Description = "كتب علمية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate }
            );

            // Books
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "زقاق المدق",
                    Description = "رواية شهيرة لنجيب محفوظ",
                    ISBN = "1234567890123",
                    Price = 150,
                    DiscountPercentage = 10,
                    PublishedDate = new DateTime(1950, 1, 1),
                    PageCount = 240,
                    Language = BookLanguage.Arabic,
                    CoverImageUrl = "cover.jpg",
                    Format = "ورقي",
                    StockQuantity = 20,
                    IsActive = true,
                    ViewCount = 0,
                    SalesCount = 0,
                    AuthorId = 1,
                    PublisherId = 1,
                    CreatedAt = fixedDate,
                    UpdatedAt = fixedDate
                },
                new Book
                {
                    Id = 2,
                    Title = "كتاب جديد",
                    Description = "وصف الكتاب الجديد",
                    ISBN = "9876543210123",
                    Price = 200,
                    DiscountPercentage = 15,
                    PublishedDate = new DateTime(2023, 1, 1),
                    PageCount = 350,
                    Language = BookLanguage.Arabic,
                    CoverImageUrl = "new_cover.jpg",
                    Format = "ورقي",
                    StockQuantity = 30,
                    IsActive = true,
                    ViewCount = 0,
                    SalesCount = 0,
                    AuthorId = 1,
                    PublisherId = 1,
                    CreatedAt = fixedDate,
                    UpdatedAt = fixedDate
                },
                new Book
                {
                    Id = 3,
                    Title = "كتاب آخر",
                    Description = "وصف الكتاب الآخر",
                    ISBN = "1234567890124",
                    Price = 250,
                    DiscountPercentage = 5,
                    PublishedDate = new DateTime(2022, 1, 1),
                    PageCount = 300,
                    Language = BookLanguage.Arabic,
                    CoverImageUrl = "another_cover.jpg",
                    Format = "ورقي",
                    StockQuantity = 15,
                    IsActive = true,
                    ViewCount = 0,
                    SalesCount = 0,
                    AuthorId = 2,
                    PublisherId = 2,
                    CreatedAt = fixedDate,
                    UpdatedAt = fixedDate
                }
            );

            modelBuilder.Entity("BookCategories").HasData(
            new { BookId = 1, CategoryId = 1 },
            new { BookId = 1, CategoryId = 2 },
            new { BookId = 2, CategoryId = 2 },
            new { BookId = 3, CategoryId = 1 },
            new { BookId = 3, CategoryId = 3 }
);
        }
    }
}
