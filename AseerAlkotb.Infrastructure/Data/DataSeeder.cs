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
        {
            var fixedDate = new DateTime(2024, 8, 1); // fixed date for consistency

            // Authors
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, Name = "نجيب محفوظ", Bio = "نجيب محفوظ عبد العزيز إبراهيم أحمد الباشا (11 ديسمبر 1911 – 30 أغسطس 2006) هو كاتب مصري. يُعد أول مصري وعربي حائز على جائزة نوبل في الأدب. كتب نجيب محفوظ منذ الثلاثينات واستمر حتى 2004. تدور أحداث جميع رواياته في مصر وتظهر فيها سمة متكررة، هي الحارة التي تعادل العالم. كتب نجيب محفوظ أكثر من ثلاثين رواية اشتهرت غالبيتها وتم إنتاجها سينمائيًا أو تلفزيونيًا وكانت أول رواياته هي عبث الأقدار (1939)، أما آخرها، فكانت قشتمر (1988)، كما كتب أكثر من عشرين قصة قصيرة وكان آخرها أحلام فترة النقاهة (2004). ومن أشهر أعماله: بداية ونهاية (1949)، والثلاثية (1956–1957)، وأولاد حارتنا (1959)، والتي مُنعت من النشر في مصر منذ صدورها وحتى وقتٍ قريب، واللص والكلاب (1961)، وثرثرة فوق النيل (1966)، والكرنك (1974)، والحرافيش (1977). بينما يُصنف أدب محفوظ باعتباره أدبًا واقعيًا، فإن مواضيعًا وجودية تظهر فيه. محفوظ أكثر أديب عربي نُقلت أعماله إلى السينما والتلفزيون.", ImageUrl = "/uploads/authors/8a48bbe0-f12a-4be4-b5bc-72d3a442dcae.jpg", CountryCode = CountryCode.EG, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Author { Id = 2, Name = "أحمد خالد توفيق", Bio = "أحمد خالد توفيق (10 يونيو 1962 – 2 أبريل 2018)، أستاذ جامعي، وطبيب، وكاتب، ومؤلف، ومترجم مصري. يُعد أول كاتب عربي في مجال أدب الرعب. والأشهر في مجال أدب الشباب، والفنتازيا، والخيال العلمي. لُقب بـ«العراب».\r\n\r\nبدأت رحلته الأدبية مع كتابة سلسلة ما وراء الطبيعة، ورغم أن أدب الرعب لم يكن سائدًا في ذلك الوقت، فإن السلسلة حققت نجاحًا كبيرًا، واستقبالًا جيدًا من الجمهور. ما شجعه على استكمالها، وأصدر بعدها سلسلة فانتازيا عام 1995، وسلسلة سفاري عام 1996. في عام 2006، سلسلة دبليو دبليو دبليو.", ImageUrl = "/uploads/authors/8a48bbe0-f12a-4be4-b5bc-72d3a442dcae.jpg", CountryCode = CountryCode.EG, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Author { Id = 3, Name = "يوسف إدريس", Bio = "كاتب مصري", ImageUrl = "/uploads/authors/8a48bbe0-f12a-4be4-b5bc-72d3a442dcae.jpg", CountryCode = CountryCode.EG, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Author { Id = 4, Name = "أحمد خيري العمري", Bio = "كاتب وطبيب أسنان عراقي من مواليد بغداد في عام 1970، ينتمي إلى الأسرة العمرية في الموصل التي يعود نسبها إلى الخليفة عمر بن الخطاب، والده مؤرخ وقاض عراقي هو خيري العمري. تخرج طبيب أسنان من جامعة بغداد عام 1993، لكنه عُرِف ككاتب إسلامي عبر مؤلفات جمعت بين منحى تجديدي في طرح الموضوعات والأسلوب الأدبي. اختير عام 2010 ليكون الشخصية الفكرية التي تكرمها دار الفكر في تقليدها السنوي، والذي سبق أن كُرم فيه أعلام مثل عبد الوهاب المسيري والبوطي والزحيلي، وبذلك يكون العمري هو أصغر هؤلاء المكرمين سناً حيث تم اختياره قبل أن يبلغ الأربعين.", ImageUrl = "/uploads/authors/Ahmed-2_(14896102021).jpg", CountryCode = CountryCode.IQ, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate }

            );

            // Publishers
            modelBuilder.Entity<Publisher>().HasData(
                new Publisher { Id = 1, Name = "عصير الكتب", Description = "دار نشر مصرية", ContactEmail = "info@aseeralkotob.com", CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Publisher { Id = 2, Name = "دار الشروق", Description = "دار نشر عربية", ContactEmail = "info2@aseeralkotob.com", CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Publisher { Id = 3, Name = "دار الساقي", Description = "دار نشر لبنانية", ContactEmail = "info3@aseeralkotob.com", CreatedAt = fixedDate, UpdatedAt = fixedDate }
            );


            // Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "روايات", Description = "كتب روائية تشمل الأدب العربي والعالمي", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 2, Name = "تاريخ", Description = "كتب تاريخية عن الحضارات والشعوب", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 3, Name = "علوم", Description = "كتب علمية في مجالات متنوعة", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 4, Name = "تنمية بشرية", Description = "كتب لتطوير الذات والمهارات", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 5, Name = "كتب دينية", Description = "كتب دينية وفكر إسلامي", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 6, Name = "أدب", Description = "كتب أدبية تشمل الشعر والنثر", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 7, Name = "كتب أطفال", Description = "كتب مخصصة للأطفال والناشئة", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 8, Name = "سياسة", Description = "كتب عن السياسة والعلاقات الدولية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 9, Name = "اقتصاد", Description = "كتب اقتصادية ومالية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 10, Name = "فلسفة", Description = "كتب فلسفية وفكرية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 11, Name = "طبخ", Description = "كتب عن الطبخ والوصفات", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 12, Name = "سير ذاتية", Description = "سير ذاتية ومذكرات", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 13, Name = "فنون", Description = "كتب عن الفنون والإبداع", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 14, Name = "تكنولوجيا", Description = "كتب عن التكنولوجيا والابتكار", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 15, Name = "طب", Description = "كتب طبية وصحية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 16, Name = "رياضة", Description = "كتب عن الرياضة واللياقة البدنية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 17, Name = "سفر", Description = "كتب عن السفر والرحلات", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 18, Name = "إدارة", Description = "كتب عن الإدارة والقيادة", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 19, Name = "قانون", Description = "كتب قانونية وتشريعات", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 20, Name = "تعليم", Description = "كتب تعليمية وتربوية", IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate }
            );

            // SubCategories
            modelBuilder.Entity<Category>().HasData(
                // فئات فرعية تحت الروايات
                new Category { Id = 21, Name = "روايات عربية", Description = "روايات مكتوبة باللغة العربية", ParentCategoryId = 1, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 22, Name = "روايات مترجمة", Description = "روايات مترجمة من لغات أخرى", ParentCategoryId = 1, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 23, Name = "روايات بوليسية", Description = "روايات الجريمة والغموض", ParentCategoryId = 1, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 24, Name = "روايات خيال علمي", Description = "روايات في عالم الخيال العلمي", ParentCategoryId = 1, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت التاريخ
                new Category { Id = 25, Name = "تاريخ العالم", Description = "كتب عن تاريخ الحضارات العالمية", ParentCategoryId = 2, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 26, Name = "تاريخ العرب", Description = "كتب عن تاريخ العرب والإسلام", ParentCategoryId = 2, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 27, Name = "تاريخ الحروب", Description = "كتب عن الحروب والصراعات", ParentCategoryId = 2, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت العلوم
                new Category { Id = 28, Name = "علوم طبيعية", Description = "كتب في الفيزياء والكيمياء", ParentCategoryId = 3, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 29, Name = "علوم حيوية", Description = "كتب في البيولوجيا والطب", ParentCategoryId = 3, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 30, Name = "علوم الفلك", Description = "كتب عن الفضاء والكون", ParentCategoryId = 3, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت التنمية البشرية
                new Category { Id = 31, Name = "تطوير الذات", Description = "كتب لتحسين المهارات الشخصية", ParentCategoryId = 4, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 32, Name = "القيادة", Description = "كتب عن مهارات القيادة", ParentCategoryId = 4, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 33, Name = "إدارة الوقت", Description = "كتب عن تنظيم الوقت", ParentCategoryId = 4, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت الكتب الدينية
                new Category { Id = 34, Name = "فكر إسلامي", Description = "كتب عن الفكر والفقه الإسلامي", ParentCategoryId = 5, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 35, Name = "تفسير القرآن", Description = "كتب تفسير القرآن الكريم", ParentCategoryId = 5, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت الأدب
                new Category { Id = 36, Name = "شعر عربي", Description = "دواوين الشعر العربي", ParentCategoryId = 6, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 37, Name = "نثر أدبي", Description = "كتب النثر الأدبي والمقالات", ParentCategoryId = 6, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت كتب الأطفال
                new Category { Id = 38, Name = "قصص أطفال", Description = "قصص مصورة للأطفال", ParentCategoryId = 7, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 39, Name = "كتب تعليمية للأطفال", Description = "كتب تعليمية للناشئة", ParentCategoryId = 7, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت السياسة
                new Category { Id = 40, Name = "علاقات دولية", Description = "كتب عن السياسة العالمية", ParentCategoryId = 8, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 41, Name = "سياسة محلية", Description = "كتب عن السياسة العربية", ParentCategoryId = 8, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت الاقتصاد
                new Category { Id = 42, Name = "اقتصاد كلي", Description = "كتب عن الاقتصاد العام", ParentCategoryId = 9, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 43, Name = "مالية شخصية", Description = "كتب عن إدارة الأموال الشخصية", ParentCategoryId = 9, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت الفلسفة
                new Category { Id = 44, Name = "فلسفة غربية", Description = "كتب عن الفلسفة الغربية", ParentCategoryId = 10, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 45, Name = "فلسفة شرقية", Description = "كتب عن الفلسفة الشرقية", ParentCategoryId = 10, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت الطبخ
                new Category { Id = 46, Name = "مطبخ عربي", Description = "وصفات الطعام العربي", ParentCategoryId = 11, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 47, Name = "مطبخ عالمي", Description = "وصفات من مطابخ العالم", ParentCategoryId = 11, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت السير الذاتية
                new Category { Id = 48, Name = "سير سياسيين", Description = "سير ذاتية لشخصيات سياسية", ParentCategoryId = 12, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 49, Name = "سير فنانين", Description = "سير ذاتية لفنانين ومبدعين", ParentCategoryId = 12, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت الفنون
                new Category { Id = 50, Name = "فنون تشكيلية", Description = "كتب عن الرسم والنحت", ParentCategoryId = 13, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 51, Name = "موسيقى", Description = "كتب عن الموسيقى والآلات", ParentCategoryId = 13, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت التكنولوجيا
                new Category { Id = 52, Name = "برمجة", Description = "كتب عن البرمجة وتطوير البرمجيات", ParentCategoryId = 14, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 53, Name = "ذكاء اصطناعي", Description = "كتب عن الذكاء الاصطناعي", ParentCategoryId = 14, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت الطب
                new Category { Id = 54, Name = "طب عام", Description = "كتب عن الطب العام والصحة", ParentCategoryId = 15, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 55, Name = "تغذية", Description = "كتب عن التغذية والصحة", ParentCategoryId = 15, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت الرياضة
                new Category { Id = 56, Name = "لياقة بدنية", Description = "كتب عن التمارين الرياضية", ParentCategoryId = 16, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 57, Name = "رياضات جماعية", Description = "كتب عن كرة القدم وغيرها", ParentCategoryId = 16, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت السفر
                new Category { Id = 58, Name = "رحلات مغامرة", Description = "كتب عن السفر والمغامرات", ParentCategoryId = 17, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },
                new Category { Id = 59, Name = "أدلة سياحية", Description = "أدلة للسفر والسياحة", ParentCategoryId = 17, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate },

                // فئات فرعية تحت الإدارة
                new Category { Id = 60, Name = "إدارة الأعمال", Description = "كتب عن إدارة الشركات", ParentCategoryId = 18, IsActive = true, CreatedAt = fixedDate, UpdatedAt = fixedDate }
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

            );

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
}
