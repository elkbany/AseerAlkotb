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
                new Author { Id = 1, Name = "نجيب محفوظ", Bio = "أديب مصري", ImageUrl= "/uploads/authors/8a48bbe0-f12a-4be4-b5bc-72d3a442dcae.jpg",CountryCode=CountryCode.EG, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Author { Id = 2, Name = "أحمد خالد توفيق", Bio = "كاتب مصري", ImageUrl = "/uploads/authors/8a48bbe0-f12a-4be4-b5bc-72d3a442dcae.jpg", CountryCode = CountryCode.EG, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Author { Id = 3, Name = "يوسف إدريس", Bio = "كاتب مصري", ImageUrl = "/uploads/authors/8a48bbe0-f12a-4be4-b5bc-72d3a442dcae.jpg", CountryCode = CountryCode.EG, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate }

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
                    CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
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
                    CoverImageUrl = "/uploads/Books/6fdbe335-1a34-4564-9564-e30dc38cf6ea.webp",
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
                    CoverImageUrl = "/uploads/Books/6fdbe335-1a34-4564-9564-e30dc38cf6ea.webp",
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

            //modelBuilder.Entity<Quote>().HasData(
            //    new Quote
            //    {
            //        Id = 1,
            //        Comment = "الحياة ليست مشكلة يجب حلها، بل هي واقع يجب تجربته.",
            //        AuthorId = 1,
            //        BookId = 1,
            //        QuoteFor = QuoteFor.Book,
            //        CreatedAt = fixedDate,
            //        UpdatedAt = fixedDate
            //    },
            //    new Quote
            //    {
            //        Id = 2,
            //        Comment = "الكتابة هي الوسيلة التي تجعلنا نعيش أكثر من حياة واحدة.",
            //        AuthorId = 2,
            //        BookId = 2,
            //        QuoteFor = QuoteFor.Author,
            //        CreatedAt = fixedDate,
            //        UpdatedAt = fixedDate
            //    },
            //    new Quote
            //    {
            //        Id = 3,
            //        Comment = "الكتب هي الأصدقاء الذين لا يخونون.",
            //        AuthorId = 3,
            //        BookId = 3,
            //        QuoteFor = QuoteFor.Book,
            //        CreatedAt = fixedDate,
            //        UpdatedAt = fixedDate
            //    }

            //);
        }
    }
}
