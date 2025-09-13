using AseerAlkotb.Domain.Entites;
using AseerAlkotb.Domain.Entites.Models;
using AseerAlkotb.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace AseerAlkotb.Infrastructure.Data
{
    public class DataSeeder
    {
        public static void SeedData(ModelBuilder modelBuilder)
        { }
            //var fixedDate = new DateTime(2024, 8, 1); // fixed date for consistency

            //// Categories
            //modelBuilder.Entity<Category>().HasData(
            //    new Category { Id = 1, Name = "روايات", Description = "كتب روائية تشمل الأدب العربي والعالمي", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 2, Name = "تاريخ", Description = "كتب تاريخية عن الحضارات والشعوب", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 3, Name = "علوم", Description = "كتب علمية في مجالات متنوعة", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 4, Name = "تنمية بشرية", Description = "كتب لتطوير الذات والمهارات", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 5, Name = "كتب دينية", Description = "كتب دينية وفكر إسلامي", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 6, Name = "أدب", Description = "كتب أدبية تشمل الشعر والنثر", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 7, Name = "كتب أطفال", Description = "كتب مخصصة للأطفال والناشئة", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 8, Name = "سياسة", Description = "كتب عن السياسة والعلاقات الدولية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 9, Name = "اقتصاد", Description = "كتب اقتصادية ومالية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 10, Name = "فلسفة", Description = "كتب فلسفية وفكرية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 11, Name = "طبخ", Description = "كتب عن الطبخ والوصفات", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 12, Name = "سير ذاتية", Description = "سير ذاتية ومذكرات", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 13, Name = "فنون", Description = "كتب عن الفنون والإبداع", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 14, Name = "تكنولوجيا", Description = "كتب عن التكنولوجيا والابتكار", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 15, Name = "طب", Description = "كتب طبية وصحية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 16, Name = "رياضة", Description = "كتب عن الرياضة واللياقة البدنية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 17, Name = "سفر", Description = "كتب عن السفر والرحلات", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 18, Name = "إدارة", Description = "كتب عن الإدارة والقيادة", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 19, Name = "قانون", Description = "كتب قانونية وتشريعات", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 20, Name = "تعليم", Description = "كتب تعليمية وتربوية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate }
            //);

            //// SubCategories
            //modelBuilder.Entity<Category>().HasData(
            //    // فئات فرعية تحت الروايات
            //    new Category { Id = 21, Name = "روايات عربية", Description = "روايات مكتوبة باللغة العربية", ParentCategoryId = 1, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 22, Name = "روايات مترجمة", Description = "روايات مترجمة من لغات أخرى", ParentCategoryId = 1, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 23, Name = "روايات بوليسية", Description = "روايات الجريمة والغموض", ParentCategoryId = 1, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 24, Name = "روايات خيال علمي", Description = "روايات في عالم الخيال العلمي", ParentCategoryId = 1, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت التاريخ
            //    new Category { Id = 25, Name = "تاريخ العالم", Description = "كتب عن تاريخ الحضارات العالمية", ParentCategoryId = 2, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 26, Name = "تاريخ العرب", Description = "كتب عن تاريخ العرب والإسلام", ParentCategoryId = 2, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 27, Name = "تاريخ الحروب", Description = "كتب عن الحروب والصراعات", ParentCategoryId = 2, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت العلوم
            //    new Category { Id = 28, Name = "علوم طبيعية", Description = "كتب في الفيزياء والكيمياء", ParentCategoryId = 3, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 29, Name = "علوم حيوية", Description = "كتب في البيولوجيا والطب", ParentCategoryId = 3, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 30, Name = "علوم الفلك", Description = "كتب عن الفضاء والكون", ParentCategoryId = 3, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت التنمية البشرية
            //    new Category { Id = 31, Name = "تطوير الذات", Description = "كتب لتحسين المهارات الشخصية", ParentCategoryId = 4, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 32, Name = "القيادة", Description = "كتب عن مهارات القيادة", ParentCategoryId = 4, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 33, Name = "إدارة الوقت", Description = "كتب عن تنظيم الوقت", ParentCategoryId = 4, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت الكتب الدينية
            //    new Category { Id = 34, Name = "فكر إسلامي", Description = "كتب عن الفكر والفقه الإسلامي", ParentCategoryId = 5, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 35, Name = "تفسير القرآن", Description = "كتب تفسير القرآن الكريم", ParentCategoryId = 5, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت الأدب
            //    new Category { Id = 36, Name = "شعر عربي", Description = "دواوين الشعر العربي", ParentCategoryId = 6, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 37, Name = "نثر أدبي", Description = "كتب النثر الأدبي والمقالات", ParentCategoryId = 6, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت كتب الأطفال
            //    new Category { Id = 38, Name = "قصص أطفال", Description = "قصص مصورة للأطفال", ParentCategoryId = 7, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 39, Name = "كتب تعليمية للأطفال", Description = "كتب تعليمية للناشئة", ParentCategoryId = 7, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت السياسة
            //    new Category { Id = 40, Name = "علاقات دولية", Description = "كتب عن السياسة العالمية", ParentCategoryId = 8, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 41, Name = "سياسة محلية", Description = "كتب عن السياسة العربية", ParentCategoryId = 8, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت الاقتصاد
            //    new Category { Id = 42, Name = "اقتصاد كلي", Description = "كتب عن الاقتصاد العام", ParentCategoryId = 9, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 43, Name = "مالية شخصية", Description = "كتب عن إدارة الأموال الشخصية", ParentCategoryId = 9, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت الفلسفة
            //    new Category { Id = 44, Name = "فلسفة غربية", Description = "كتب عن الفلسفة الغربية", ParentCategoryId = 10, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 45, Name = "فلسفة شرقية", Description = "كتب عن الفلسفة الشرقية", ParentCategoryId = 10, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت الطبخ
            //    new Category { Id = 46, Name = "مطبخ عربي", Description = "وصفات الطعام العربي", ParentCategoryId = 11, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 47, Name = "مطبخ عالمي", Description = "وصفات من مطابخ العالم", ParentCategoryId = 11, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت السير الذاتية
            //    new Category { Id = 48, Name = "سير سياسيين", Description = "سير ذاتية لشخصيات سياسية", ParentCategoryId = 12, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 49, Name = "سير فنانين", Description = "سير ذاتية لفنانين ومبدعين", ParentCategoryId = 12, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت الفنون
            //    new Category { Id = 50, Name = "فنون تشكيلية", Description = "كتب عن الرسم والنحت", ParentCategoryId = 13, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 51, Name = "موسيقى", Description = "كتب عن الموسيقى والآلات", ParentCategoryId = 13, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت التكنولوجيا
            //    new Category { Id = 52, Name = "برمجة", Description = "كتب عن البرمجة وتطوير البرمجيات", ParentCategoryId = 14, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 53, Name = "ذكاء اصطناعي", Description = "كتب عن الذكاء الاصطناعي", ParentCategoryId = 14, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت الطب
            //    new Category { Id = 54, Name = "طب عام", Description = "كتب عن الطب العام والصحة", ParentCategoryId = 15, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 55, Name = "تغذية", Description = "كتب عن التغذية والصحة", ParentCategoryId = 15, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت الرياضة
            //    new Category { Id = 56, Name = "لياقة بدنية", Description = "كتب عن التمارين الرياضية", ParentCategoryId = 16, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 57, Name = "رياضات جماعية", Description = "كتب عن كرة القدم وغيرها", ParentCategoryId = 16, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت السفر
            //    new Category { Id = 58, Name = "رحلات مغامرة", Description = "كتب عن السفر والمغامرات", ParentCategoryId = 17, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
            //    new Category { Id = 59, Name = "أدلة سياحية", Description = "أدلة للسفر والسياحة", ParentCategoryId = 17, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

            //    // فئات فرعية تحت الإدارة
            //    new Category { Id = 60, Name = "إدارة الأعمال", Description = "كتب عن إدارة الشركات", ParentCategoryId = 18, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate }
            //);


            //// Books
            //modelBuilder.Entity<Book>().HasData(
            //    new Book
            //    {
            //        Id = 1,
            //        Title = "زقاق المدق",
            //        Description = "رواية شهيرة لنجيب محفوظ",
            //        ISBN = "1234567890123",
            //        Price = 150,
            //        DiscountPercentage = 10,
            //        PublishedDate = new DateTime(1950, 1, 1),
            //        PageCount = 240,
            //        Language = BookLanguage.Arabic,
            //        CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //        Format = "ورقي",
            //        StockQuantity = 20,
            //        IsActive = true,
            //        ViewCount = 0,
            //        SalesCount = 0,
            //        AuthorId = 1,
            //        PublisherId = 1,
            //        CreatedAt = fixedDate,
            //        UpdatedAt = fixedDate
            //    },
            //    new Book
            //    {
            //        Id = 2,
            //        Title = "كتاب جديد",
            //        Description = "وصف الكتاب الجديد",
            //        ISBN = "9876543210123",
            //        Price = 200,
            //        DiscountPercentage = 15,
            //        PublishedDate = new DateTime(2023, 1, 1),
            //        PageCount = 350,
            //        Language = BookLanguage.Arabic,
            //        CoverImageUrl = "/uploads/Books/6fdbe335-1a34-4564-9564-e30dc38cf6ea.webp",
            //        Format = "ورقي",
            //        StockQuantity = 30,
            //        IsActive = true,
            //        ViewCount = 0,
            //        SalesCount = 0,
            //        AuthorId = 1,
            //        PublisherId = 1,
            //        CreatedAt = fixedDate,
            //        UpdatedAt = fixedDate
            //    },
            //    new Book
            //    {
            //        Id = 3,
            //        Title = "كتاب آخر",
            //        Description = "وصف الكتاب الآخر",
            //        ISBN = "1234567890124",
            //        Price = 250,
            //        DiscountPercentage = 5,
            //        PublishedDate = new DateTime(2022, 1, 1),
            //        PageCount = 300,
            //        Language = BookLanguage.Arabic,
            //        CoverImageUrl = "/uploads/Books/6fdbe335-1a34-4564-9564-e30dc38cf6ea.webp",
            //        Format = "ورقي",
            //        StockQuantity = 15,
            //        IsActive = true,
            //        ViewCount = 0,
            //        SalesCount = 0,
            //        AuthorId = 2,
            //        PublisherId = 2,
            //        CreatedAt = fixedDate,
            //        UpdatedAt = fixedDate
            //    },         

            //new Book
            //{
            //    Id = 4,
            //    Title = "الكرنك",
            //    Description = "رواية سياسية لنجيب محفوظ",
            //    ISBN = "1234567890126",
            //    Price = 170,
            //    DiscountPercentage = 10,
            //    PublishedDate = new DateTime(1974, 1, 1),
            //    PageCount = 250,
            //    Language = BookLanguage.Arabic,
            //    CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //    Format = "ورقي",
            //    StockQuantity = 18,
            //    IsActive = true,
            //    ViewCount = 0,
            //    SalesCount = 0,
            //    AuthorId = 1,
            //    PublisherId = 1,
            //    CreatedAt = fixedDate,
            //    UpdatedAt = fixedDate
            //},
            //     new Book
            //     {
            //         Id = 5,
            //         Title = "أولاد حارتنا",
            //         Description = "رواية مثيرة للجدل لنجيب محفوظ",
            //         ISBN = "1234567890127",
            //         Price = 200,
            //         DiscountPercentage = 20,
            //         PublishedDate = new DateTime(1959, 1, 1),
            //         PageCount = 300,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //         Format = "ورقي",
            //         StockQuantity = 30,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },
            //     new Book
            //     {
            //         Id = 6,
            //         Title = "الحرافيش",
            //         Description = "ملحمة أدبية لنجيب محفوظ",
            //         ISBN = "1234567890128",
            //         Price = 180,
            //         DiscountPercentage = 12,
            //         PublishedDate = new DateTime(1977, 1, 1),
            //         PageCount = 350,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //         Format = "ورقي",
            //         StockQuantity = 20,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },
            //     new Book
            //     {
            //         Id = 7,
            //         Title = "بين القصرين",
            //         Description = "الجزء الأول من ثلاثية نجيب محفوظ",
            //         ISBN = "1234567890129",
            //         Price = 150,
            //         DiscountPercentage = 8,
            //         PublishedDate = new DateTime(1956, 1, 1),
            //         PageCount = 270,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //         Format = "ورقي",
            //         StockQuantity = 22,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },
            //     new Book
            //     {
            //         Id = 8,
            //         Title = "قصر الشوق",
            //         Description = "الجزء الثاني من ثلاثية نجيب محفوظ",
            //         ISBN = "1234567890130",
            //         Price = 160,
            //         DiscountPercentage = 6,
            //         PublishedDate = new DateTime(1957, 1, 1),
            //         PageCount = 280,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //         Format = "ورقي",
            //         StockQuantity = 20,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },
            //     new Book
            //     {
            //         Id = 9,
            //         Title = "السكرية",
            //         Description = "الجزء الثالث من ثلاثية نجيب محفوظ",
            //         ISBN = "1234567890131",
            //         Price = 170,
            //         DiscountPercentage = 7,
            //         PublishedDate = new DateTime(1957, 1, 1),
            //         PageCount = 290,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //         Format = "ورقي",
            //         StockQuantity = 18,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },
            //     new Book
            //     {
            //         Id = 10,
            //         Title = "خان الخليلي",
            //         Description = "رواية اجتماعية لنجيب محفوظ",
            //         ISBN = "1234567890132",
            //         Price = 140,
            //         DiscountPercentage = 5,
            //         PublishedDate = new DateTime(1946, 1, 1),
            //         PageCount = 210,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //         Format = "ورقي",
            //         StockQuantity = 20,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },
            //     new Book
            //     {
            //         Id = 11,
            //         Title = "كتاب جديد",
            //         Description = "وصف الكتاب الجديد",
            //         ISBN = "9876543210178",
            //         Price = 200,
            //         DiscountPercentage = 15,
            //         PublishedDate = new DateTime(2023, 1, 1),
            //         PageCount = 350,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/6fdbe335-1a34-4564-9564-e30dc38cf6ea.webp",
            //         Format = "ورقي",
            //         StockQuantity = 30,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },
            //     new Book
            //     {
            //         Id = 12,
            //         Title = "كتاب آخر",
            //         Description = "وصف الكتاب الآخر",
            //         ISBN = "1234567890177",
            //         Price = 250,
            //         DiscountPercentage = 5,
            //         PublishedDate = new DateTime(2022, 1, 1),
            //         PageCount = 300,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/6fdbe335-1a34-4564-9564-e30dc38cf6ea.webp",
            //         Format = "ورقي",
            //         StockQuantity = 15,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 2,
            //         PublisherId = 2,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },
            //     new Book
            //     {
            //         Id = 13,
            //         Title = "الطوق والأسورة",
            //         Description = "رواية شهيرة ليحيى الطاهر عبدالله",
            //         ISBN = "1234567890133",
            //         Price = 120,
            //         DiscountPercentage = 0,
            //         PublishedDate = new DateTime(1975, 1, 1),
            //         PageCount = 190,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //         Format = "ورقي",
            //         StockQuantity = 20,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },
            //     new Book
            //     {
            //         Id = 14,
            //         Title = "رجال في الشمس",
            //         Description = "رواية غسان كنفاني الشهيرة",
            //         ISBN = "1234567890134",
            //         Price = 130,
            //         DiscountPercentage = 10,
            //         PublishedDate = new DateTime(1963, 1, 1),
            //         PageCount = 160,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //         Format = "ورقي",
            //         StockQuantity = 20,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },
            //     new Book
            //     {
            //         Id = 15,
            //         Title = "موسم الهجرة إلى الشمال",
            //         Description = "رواية الطيب صالح الأشهر",
            //         ISBN = "1234567890135",
            //         Price = 150,
            //         DiscountPercentage = 15,
            //         PublishedDate = new DateTime(1966, 1, 1),
            //         PageCount = 220,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //         Format = "ورقي",
            //         StockQuantity = 20,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },
            //     new Book
            //     {
            //         Id = 16,
            //         Title = "الخبز الحافي",
            //         Description = "رواية محمد شكري الذاتية",
            //         ISBN = "1234567890136",
            //         Price = 140,
            //         DiscountPercentage = 5,
            //         PublishedDate = new DateTime(1973, 1, 1),
            //         PageCount = 200,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //         Format = "ورقي",
            //         StockQuantity = 20,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },
            //     new Book
            //     {
            //         Id = 17,
            //         Title = "عزازيل",
            //         Description = "رواية يوسف زيدان التاريخية",
            //         ISBN = "1234567890137",
            //         Price = 160,
            //         DiscountPercentage = 10,
            //         PublishedDate = new DateTime(2008, 1, 1),
            //         PageCount = 350,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //         Format = "ورقي",
            //         StockQuantity = 20,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },
            //     new Book
            //     {
            //         Id = 18,
            //         Title = "عائد إلى حيفا",
            //         Description = "رواية قصيرة لغسان كنفاني",
            //         ISBN = "1234567890138",
            //         Price = 110,
            //         DiscountPercentage = 0,
            //         PublishedDate = new DateTime(1970, 1, 1),
            //         PageCount = 100,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //         Format = "ورقي",
            //         StockQuantity = 20,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },

            //     new Book
            //     {
            //         Id = 19,
            //         Title = "الحب في زمن الكوليرا",
            //         Description = "رواية غابرييل غارسيا ماركيز المترجمة للعربية",
            //         ISBN = "1234567890139",
            //         Price = 170,
            //         DiscountPercentage = 20,
            //         PublishedDate = new DateTime(1985, 1, 1),
            //         PageCount = 420,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //         Format = "ورقي",
            //         StockQuantity = 20,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     },
            //     new Book
            //     {
            //         Id = 20,
            //         Title = "مدن الملح",
            //         Description = "رواية عبد الرحمن منيف الشهيرة",
            //         ISBN = "1234567890140",
            //         Price = 180,
            //         DiscountPercentage = 10,
            //         PublishedDate = new DateTime(1984, 1, 1),
            //         PageCount = 500,
            //         Language = BookLanguage.Arabic,
            //         CoverImageUrl = "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp",
            //         Format = "ورقي",
            //         StockQuantity = 20,
            //         IsActive = true,
            //         ViewCount = 0,
            //         SalesCount = 0,
            //         AuthorId = 1,
            //         PublisherId = 1,
            //         CreatedAt = fixedDate,
            //         UpdatedAt = fixedDate
            //     }
            //);

            //modelBuilder.Entity("BookCategories").HasData(
            //new { BookId = 1, CategoryId = 1 },
            //new { BookId = 1, CategoryId = 2 },
            //new { BookId = 2, CategoryId = 2 },
            //new { BookId = 3, CategoryId = 1 },
            //new { BookId = 3, CategoryId = 3 },
            //new { BookId = 4, CategoryId = 2 }, // الكرنك - أدب عربي
            //new { BookId = 4, CategoryId = 3 }, // الكرنك - روايات سياسية
            //new { BookId = 5, CategoryId = 1 }, // أولاد حارتنا - روايات اجتماعية
            //new { BookId = 5, CategoryId = 2 }, // أولاد حارتنا - أدب عربي
            //new { BookId = 5, CategoryId = 4 }, // أولاد حارتنا - روايات دينية (لأنها مثيرة للجدل بموضوع ديني)
            //new { BookId = 6, CategoryId = 1 }, // الحرافيش - روايات اجتماعية
            //new { BookId = 6, CategoryId = 2 }, // الحرافيش - أدب عربي
            //new { BookId = 6, CategoryId = 5 }, // الحرافيش - ملحمة أدبية
            //new { BookId = 7, CategoryId = 1 }, // بين القصرين - روايات اجتماعية
            //new { BookId = 7, CategoryId = 2 }, // بين القصرين - أدب عربي
            //new { BookId = 7, CategoryId = 6 }, // بين القصرين - ثلاثيات أدبية
            //new { BookId = 8, CategoryId = 1 }, // قصر الشوق - روايات اجتماعية
            //new { BookId = 8, CategoryId = 2 }, // قصر الشوق - أدب عربي
            //new { BookId = 8, CategoryId = 6 }, // قصر الشوق - ثلاثيات أدبية
            //new { BookId = 9, CategoryId = 1 }, // السكرية - روايات اجتماعية
            //new { BookId = 9, CategoryId = 2 }, // السكرية - أدب عربي
            //new { BookId = 9, CategoryId = 6 }, // السكرية - ثلاثيات أدبية
            //new { BookId = 10, CategoryId = 1 }, // خان الخليلي - روايات اجتماعية
            //new { BookId = 10, CategoryId = 2 }, // خان الخليلي - أدب عربي
            //new { BookId = 11, CategoryId = 2 }, // كتاب جديد - أدب عربي
            //new { BookId = 11, CategoryId = 7 }, // كتاب جديد - روايات معاصرة
            //new { BookId = 12, CategoryId = 1 }, // كتاب آخر - روايات اجتماعية
            //new { BookId = 12, CategoryId = 3 }, // كتاب آخر - روايات سياسية
            //new { BookId = 13, CategoryId = 1 }, // الطوق والأسورة - روايات اجتماعية
            //new { BookId = 13, CategoryId = 2 }, // الطوق والأسورة - أدب عربي
            //new { BookId = 14, CategoryId = 2 }, // رجال في الشمس - أدب عربي
            //new { BookId = 14, CategoryId = 3 }, // رجال في الشمس - روايات سياسية
            //new { BookId = 15, CategoryId = 1 }, // موسم الهجرة إلى الشمال - روايات اجتماعية
            //new { BookId = 15, CategoryId = 2 }, // موسم الهجرة إلى الشمال - أدب عربي
            //new { BookId = 15, CategoryId = 8 }, // موسم الهجرة إلى الشمال - روايات ثقافية
            //new { BookId = 16, CategoryId = 1 }, // الخبز الحافي - روايات اجتماعية
            //new { BookId = 16, CategoryId = 2 }, // الخبز الحافي - أدب عربي
            //new { BookId = 16, CategoryId = 9 }, // الخبز الحافي - سيرة ذاتية
            //new { BookId = 17, CategoryId = 2 }, // عزازيل - أدب عربي
            //new { BookId = 17, CategoryId = 10 }, // عزازيل - روايات تاريخية
            //new { BookId = 18, CategoryId = 2 }, // عائد إلى حيفا - أدب عربي
            //new { BookId = 18, CategoryId = 3 }, // عائد إلى حيفا - روايات سياسية
            //new { BookId = 19, CategoryId = 11 }, // الحب في زمن الكوليرا - روايات مترجمة
            //new { BookId = 19, CategoryId = 12 }, // الحب في زمن الكوليرا - روايات رومانسية
            //new { BookId = 20, CategoryId = 1 }, // مدن الملح - روايات اجتماعية
            //new { BookId = 20, CategoryId = 2 }, // مدن الملح - أدب عربي
            //new { BookId = 20, CategoryId = 3 } // 




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

            //);

            //Users
            //modelBuilder.Entity<User>().HasData(
            //    new User
            //    {
            //        Id = 1,
            //        FirstName = "أحمد",
            //        LastName = "محمد",
            //        UserName = "ahmed.mohamed",
            //        Email = "Ahmed@dasdsa.com",
            //        NormalizedEmail = "Ahmed@dasdsa.com".ToUpper(),
            //        PhoneNumber = "01234567890",

            //        CreatedAt = fixedDate,
            //        UpdatedAt = fixedDate
            //    }
            //);

            //    modelBuilder.Entity<Order>().HasData(
            //    new Order
            //    {
            //        Id = 1,
            //        UserId = 1,
            //        OrderDate = new DateTime(2022, 1, 1),
            //        TotalAmount = 50,
            //        Status = OrderStatus.Pending,
            //        TrackingNumber = "TRK123456789",
            //    },
            //    new Order
            //    {
            //        Id = 2,
            //        UserId = 1,
            //        OrderDate = new DateTime(2022, 1, 1),
            //        TotalAmount = 100,
            //        Status = OrderStatus.Pending,
            //        TrackingNumber = "TRK987654321",
            //    }
            //);


        }
    
    }

