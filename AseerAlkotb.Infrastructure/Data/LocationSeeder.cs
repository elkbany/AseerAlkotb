using AseerAlkotb.Domain.Entites.Models;
using Microsoft.EntityFrameworkCore;

namespace AseerAlkotb.Infrastructure.Data
{
    public static class LocationSeeder
    {
        private static readonly DateTime StaticDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        public static void SeedLocations(ModelBuilder modelBuilder)
        {
            // Seed Governorates
            var governorates = new[]
            {
                new Governorate { Id = 1, Name = "الدقهلية", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 2, Name = "البحيرة", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 3, Name = "الغربية", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 4, Name = "الإسكندرية", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 5, Name = "الإسماعيلية", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 6, Name = "المنوفية", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 7, Name = "القليوبية", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 8, Name = "الشرقية", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 9, Name = "دمياط", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 10, Name = "كفر الشيخ", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 11, Name = "الفيوم", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 12, Name = "المنيا", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 13, Name = "أسيوط", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 14, Name = "سوهاج", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 15, Name = "قنا", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 16, Name = "الأقصر", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 17, Name = "أسوان", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 18, Name = "بني سويف", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 19, Name = "القاهرة", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 20, Name = "الجيزة", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 21, Name = "بورسعيد", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 22, Name = "السويس", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 23, Name = "البحر الأحمر", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 24, Name = "الوادي الجديد", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 25, Name = "مطروح", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 26, Name = "شمال سيناء", CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new Governorate { Id = 27, Name = "جنوب سيناء", CreatedAt = StaticDate, UpdatedAt = StaticDate }
            };

            modelBuilder.Entity<Governorate>().HasData(governorates);

            // Seed Cities (comprehensive list for each governorate)
            var cities = new[]
            {
                // الدقهلية (1)
                new City { Id = 1, Name = "المنصورة", GovernorateId = 1, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 2, Name = "طلخا", GovernorateId = 1, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 3, Name = "المطرية", GovernorateId = 1, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 4, Name = "بلقاس", GovernorateId = 1, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 5, Name = "دكرنس", GovernorateId = 1, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // البحيرة (2)
                new City { Id = 6, Name = "دمنهور", GovernorateId = 2, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 7, Name = "كفر الدوار", GovernorateId = 2, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 8, Name = "رشيد", GovernorateId = 2, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 9, Name = "إدكو", GovernorateId = 2, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 10, Name = "أبو حمص", GovernorateId = 2, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // الغربية (3)
                new City { Id = 11, Name = "طنطا", GovernorateId = 3, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 12, Name = "المحلة الكبرى", GovernorateId = 3, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 13, Name = "كفر الزيات", GovernorateId = 3, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 14, Name = "زفتى", GovernorateId = 3, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 15, Name = "السنطة", GovernorateId = 3, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // الإسكندرية (4)
                new City { Id = 16, Name = "الإسكندرية", GovernorateId = 4, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 17, Name = "المنتزة", GovernorateId = 4, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 18, Name = "العامرية", GovernorateId = 4, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 19, Name = "برج العرب", GovernorateId = 4, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 20, Name = "الدخيلة", GovernorateId = 4, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // الإسماعيلية (5)
                new City { Id = 21, Name = "الإسماعيلية", GovernorateId = 5, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 22, Name = "فايد", GovernorateId = 5, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 23, Name = "القنطرة شرق", GovernorateId = 5, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 24, Name = "أبو صوير", GovernorateId = 5, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // المنوفية (6)
                new City { Id = 25, Name = "شبين الكوم", GovernorateId = 6, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 26, Name = "منوف", GovernorateId = 6, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 27, Name = "أشمون", GovernorateId = 6, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 28, Name = "قويسنا", GovernorateId = 6, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // القليوبية (7)
                new City { Id = 29, Name = "بنها", GovernorateId = 7, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 30, Name = "شبرا الخيمة", GovernorateId = 7, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 31, Name = "القناطر الخيرية", GovernorateId = 7, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 32, Name = "طوخ", GovernorateId = 7, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 33, Name = "كفر شكر", GovernorateId = 7, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // الشرقية (8)
                new City { Id = 34, Name = "الزقازيق", GovernorateId = 8, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 35, Name = "العاشر من رمضان", GovernorateId = 8, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 36, Name = "بلبيس", GovernorateId = 8, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 37, Name = "مينا القمح", GovernorateId = 8, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 38, Name = "فاقوس", GovernorateId = 8, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // دمياط (9)
                new City { Id = 39, Name = "دمياط", GovernorateId = 9, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 40, Name = "رأس البر", GovernorateId = 9, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 41, Name = "فارسكور", GovernorateId = 9, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 42, Name = "كفر سعد", GovernorateId = 9, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // كفر الشيخ (10)
                new City { Id = 43, Name = "كفر الشيخ", GovernorateId = 10, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 44, Name = "دسوق", GovernorateId = 10, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 45, Name = "فوه", GovernorateId = 10, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 46, Name = "مطوبس", GovernorateId = 10, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // الفيوم (11)
                new City { Id = 47, Name = "الفيوم", GovernorateId = 11, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 48, Name = "سنورس", GovernorateId = 11, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 49, Name = "طامية", GovernorateId = 11, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 50, Name = "يوسف الصديق", GovernorateId = 11, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // المنيا (12)
                new City { Id = 51, Name = "المنيا", GovernorateId = 12, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 52, Name = "ملوي", GovernorateId = 12, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 53, Name = "سمالوط", GovernorateId = 12, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 54, Name = "أبو قرقاص", GovernorateId = 12, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 55, Name = "بني مزار", GovernorateId = 12, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // أسيوط (13)
                new City { Id = 56, Name = "أسيوط", GovernorateId = 13, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 57, Name = "ديروط", GovernorateId = 13, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 58, Name = "منفلوط", GovernorateId = 13, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 59, Name = "القوصية", GovernorateId = 13, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 60, Name = "أبنوب", GovernorateId = 13, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // سوهاج (14)
                new City { Id = 61, Name = "سوهاج", GovernorateId = 14, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 62, Name = "أخميم", GovernorateId = 14, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 63, Name = "طهطا", GovernorateId = 14, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 64, Name = "جرجا", GovernorateId = 14, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 65, Name = "المراغة", GovernorateId = 14, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // قنا (15)
                new City { Id = 66, Name = "قنا", GovernorateId = 15, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 67, Name = "قوص", GovernorateId = 15, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 68, Name = "نقادة", GovernorateId = 15, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 69, Name = "فرشوط", GovernorateId = 15, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // الأقصر (16)
                new City { Id = 70, Name = "الأقصر", GovernorateId = 16, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 71, Name = "إسنا", GovernorateId = 16, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 72, Name = "الطود", GovernorateId = 16, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 73, Name = "أرمنت", GovernorateId = 16, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // أسوان (17)
                new City { Id = 74, Name = "أسوان", GovernorateId = 17, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 75, Name = "كوم أمبو", GovernorateId = 17, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 76, Name = "إدفو", GovernorateId = 17, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 77, Name = "دراو", GovernorateId = 17, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // بني سويف (18)
                new City { Id = 78, Name = "بني سويف", GovernorateId = 18, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 79, Name = "الواسطى", GovernorateId = 18, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 80, Name = "ناصر", GovernorateId = 18, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 81, Name = "إهناسيا", GovernorateId = 18, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // القاهرة (19)
                new City { Id = 82, Name = "وسط البلد", GovernorateId = 19, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 83, Name = "مصر الجديدة", GovernorateId = 19, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 84, Name = "مدينة نصر", GovernorateId = 19, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 85, Name = "المعادي", GovernorateId = 19, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 86, Name = "الزمالك", GovernorateId = 19, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 87, Name = "شبرا", GovernorateId = 19, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 88, Name = "مصر القديمة", GovernorateId = 19, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // الجيزة (20)
                new City { Id = 89, Name = "الجيزة", GovernorateId = 20, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 90, Name = "6 أكتوبر", GovernorateId = 20, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 91, Name = "الشيخ زايد", GovernorateId = 20, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 92, Name = "الهرم", GovernorateId = 20, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 93, Name = "إمبابة", GovernorateId = 20, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 94, Name = "كرداسة", GovernorateId = 20, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // بورسعيد (21)
                new City { Id = 95, Name = "بورسعيد", GovernorateId = 21, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 96, Name = "بور فؤاد", GovernorateId = 21, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 97, Name = "الضواحي", GovernorateId = 21, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // السويس (22)
                new City { Id = 98, Name = "السويس", GovernorateId = 22, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 99, Name = "الأربعين", GovernorateId = 22, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 100, Name = "عتاقة", GovernorateId = 22, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // البحر الأحمر (23)
                new City { Id = 101, Name = "الغردقة", GovernorateId = 23, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 102, Name = "سفاجا", GovernorateId = 23, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 103, Name = "مرسى علم", GovernorateId = 23, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 104, Name = "القصير", GovernorateId = 23, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // الوادي الجديد (24)
                new City { Id = 105, Name = "الخارجة", GovernorateId = 24, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 106, Name = "الداخلة", GovernorateId = 24, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 107, Name = "الفرافرة", GovernorateId = 24, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 108, Name = "باريس", GovernorateId = 24, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // مطروح (25)
                new City { Id = 109, Name = "مرسى مطروح", GovernorateId = 25, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 110, Name = "العلمين", GovernorateId = 25, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 111, Name = "الحمام", GovernorateId = 25, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 112, Name = "سيدي براني", GovernorateId = 25, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // شمال سيناء (26)
                new City { Id = 113, Name = "العريش", GovernorateId = 26, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 114, Name = "رفح", GovernorateId = 26, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 115, Name = "الشيخ زويد", GovernorateId = 26, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 116, Name = "بئر العبد", GovernorateId = 26, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                
                // جنوب سيناء (27)
                new City { Id = 117, Name = "شرم الشيخ", GovernorateId = 27, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 118, Name = "دهب", GovernorateId = 27, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 119, Name = "نويبع", GovernorateId = 27, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 120, Name = "طابا", GovernorateId = 27, CreatedAt = StaticDate, UpdatedAt = StaticDate },
                new City { Id = 121, Name = "سانت كاترين", GovernorateId = 27, CreatedAt = StaticDate, UpdatedAt = StaticDate }
            };

            modelBuilder.Entity<City>().HasData(cities);
        }
    }
}