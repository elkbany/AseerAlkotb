using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedingCategoriesAndSubCategores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Bio", "CountryCode", "CreatedAt", "ImageUrl", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "نجيب محفوظ عبد العزيز إبراهيم أحمد الباشا (11 ديسمبر 1911 – 30 أغسطس 2006) هو كاتب مصري. يُعد أول مصري وعربي حائز على جائزة نوبل في الأدب. كتب نجيب محفوظ منذ الثلاثينات واستمر حتى 2004. تدور أحداث جميع رواياته في مصر وتظهر فيها سمة متكررة، هي الحارة التي تعادل العالم. كتب نجيب محفوظ أكثر من ثلاثين رواية اشتهرت غالبيتها وتم إنتاجها سينمائيًا أو تلفزيونيًا وكانت أول رواياته هي عبث الأقدار (1939)، أما آخرها، فكانت قشتمر (1988)، كما كتب أكثر من عشرين قصة قصيرة وكان آخرها أحلام فترة النقاهة (2004). ومن أشهر أعماله: بداية ونهاية (1949)، والثلاثية (1956–1957)، وأولاد حارتنا (1959)، والتي مُنعت من النشر في مصر منذ صدورها وحتى وقتٍ قريب، واللص والكلاب (1961)، وثرثرة فوق النيل (1966)، والكرنك (1974)، والحرافيش (1977). بينما يُصنف أدب محفوظ باعتباره أدبًا واقعيًا، فإن مواضيعًا وجودية تظهر فيه. محفوظ أكثر أديب عربي نُقلت أعماله إلى السينما والتلفزيون.", "EG", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "/uploads/authors/8a48bbe0-f12a-4be4-b5bc-72d3a442dcae.jpg", true, "نجيب محفوظ", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "أحمد خالد توفيق (10 يونيو 1962 – 2 أبريل 2018)، أستاذ جامعي، وطبيب، وكاتب، ومؤلف، ومترجم مصري. يُعد أول كاتب عربي في مجال أدب الرعب. والأشهر في مجال أدب الشباب، والفنتازيا، والخيال العلمي. لُقب بـ«العراب».\r\n\r\nبدأت رحلته الأدبية مع كتابة سلسلة ما وراء الطبيعة، ورغم أن أدب الرعب لم يكن سائدًا في ذلك الوقت، فإن السلسلة حققت نجاحًا كبيرًا، واستقبالًا جيدًا من الجمهور. ما شجعه على استكمالها، وأصدر بعدها سلسلة فانتازيا عام 1995، وسلسلة سفاري عام 1996. في عام 2006، سلسلة دبليو دبليو دبليو.", "EG", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "/uploads/authors/8a48bbe0-f12a-4be4-b5bc-72d3a442dcae.jpg", true, "أحمد خالد توفيق", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "كاتب مصري", "EG", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "/uploads/authors/8a48bbe0-f12a-4be4-b5bc-72d3a442dcae.jpg", true, "يوسف إدريس", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "كاتب وطبيب أسنان عراقي من مواليد بغداد في عام 1970، ينتمي إلى الأسرة العمرية في الموصل التي يعود نسبها إلى الخليفة عمر بن الخطاب، والده مؤرخ وقاض عراقي هو خيري العمري. تخرج طبيب أسنان من جامعة بغداد عام 1993، لكنه عُرِف ككاتب إسلامي عبر مؤلفات جمعت بين منحى تجديدي في طرح الموضوعات والأسلوب الأدبي. اختير عام 2010 ليكون الشخصية الفكرية التي تكرمها دار الفكر في تقليدها السنوي، والذي سبق أن كُرم فيه أعلام مثل عبد الوهاب المسيري والبوطي والزحيلي، وبذلك يكون العمري هو أصغر هؤلاء المكرمين سناً حيث تم اختياره قبل أن يبلغ الأربعين.", "IQ", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "/uploads/authors/Ahmed-2_(14896102021).jpg", true, "أحمد خيري العمري", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "ParentCategoryId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب روائية تشمل الأدب العربي والعالمي", true, "روايات", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب تاريخية عن الحضارات والشعوب", true, "تاريخ", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب علمية في مجالات متنوعة", true, "علوم", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب لتطوير الذات والمهارات", true, "تنمية بشرية", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب دينية وفكر إسلامي", true, "كتب دينية", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب أدبية تشمل الشعر والنثر", true, "أدب", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب مخصصة للأطفال والناشئة", true, "كتب أطفال", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن السياسة والعلاقات الدولية", true, "سياسة", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب اقتصادية ومالية", true, "اقتصاد", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب فلسفية وفكرية", true, "فلسفة", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن الطبخ والوصفات", true, "طبخ", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "سير ذاتية ومذكرات", true, "سير ذاتية", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن الفنون والإبداع", true, "فنون", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن التكنولوجيا والابتكار", true, "تكنولوجيا", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 15, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب طبية وصحية", true, "طب", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 16, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن الرياضة واللياقة البدنية", true, "رياضة", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 17, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن السفر والرحلات", true, "سفر", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 18, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن الإدارة والقيادة", true, "إدارة", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 19, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب قانونية وتشريعات", true, "قانون", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 20, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب تعليمية وتربوية", true, "تعليم", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "Id", "ContactEmail", "CreatedAt", "Description", "LogoUrl", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "info@aseeralkotob.com", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "دار نشر مصرية", null, "عصير الكتب", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "info2@aseeralkotob.com", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "دار نشر عربية", null, "دار الشروق", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "info3@aseeralkotob.com", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "دار نشر لبنانية", null, "دار الساقي", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "CoverImageUrl", "CreatedAt", "Description", "DiscountPercentage", "Format", "ISBN", "IsActive", "Language", "PageCount", "Price", "PublishedDate", "PublisherId", "SalesCount", "StockQuantity", "Title", "UpdatedAt", "ViewCount" },
                values: new object[,]
                {
                    { 1, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رواية شهيرة لنجيب محفوظ", 10m, "ورقي", "1234567890123", true, 1, 240, 150m, new DateTime(1950, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 20, "زقاق المدق", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 2, 1, "/uploads/Books/6fdbe335-1a34-4564-9564-e30dc38cf6ea.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "وصف الكتاب الجديد", 15m, "ورقي", "9876543210123", true, 1, 350, 200m, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 30, "كتاب جديد", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 3, 2, "/uploads/Books/6fdbe335-1a34-4564-9564-e30dc38cf6ea.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "وصف الكتاب الآخر", 5m, "ورقي", "1234567890124", true, 1, 300, 250m, new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0, 15, "كتاب آخر", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "ParentCategoryId", "UpdatedAt" },
                values: new object[,]
                {
                    { 21, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "روايات مكتوبة باللغة العربية", true, "روايات عربية", 1, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 22, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "روايات مترجمة من لغات أخرى", true, "روايات مترجمة", 1, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 23, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "روايات الجريمة والغموض", true, "روايات بوليسية", 1, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 24, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "روايات في عالم الخيال العلمي", true, "روايات خيال علمي", 1, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 25, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن تاريخ الحضارات العالمية", true, "تاريخ العالم", 2, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 26, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن تاريخ العرب والإسلام", true, "تاريخ العرب", 2, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 27, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن الحروب والصراعات", true, "تاريخ الحروب", 2, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 28, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب في الفيزياء والكيمياء", true, "علوم طبيعية", 3, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 29, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب في البيولوجيا والطب", true, "علوم حيوية", 3, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 30, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن الفضاء والكون", true, "علوم الفلك", 3, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 31, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب لتحسين المهارات الشخصية", true, "تطوير الذات", 4, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 32, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن مهارات القيادة", true, "القيادة", 4, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 33, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن تنظيم الوقت", true, "إدارة الوقت", 4, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 34, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن الفكر والفقه الإسلامي", true, "فكر إسلامي", 5, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 35, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب تفسير القرآن الكريم", true, "تفسير القرآن", 5, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 36, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "دواوين الشعر العربي", true, "شعر عربي", 6, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 37, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب النثر الأدبي والمقالات", true, "نثر أدبي", 6, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 38, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "قصص مصورة للأطفال", true, "قصص أطفال", 7, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 39, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب تعليمية للناشئة", true, "كتب تعليمية للأطفال", 7, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 40, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن السياسة العالمية", true, "علاقات دولية", 8, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 41, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن السياسة العربية", true, "سياسة محلية", 8, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 42, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن الاقتصاد العام", true, "اقتصاد كلي", 9, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 43, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن إدارة الأموال الشخصية", true, "مالية شخصية", 9, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 44, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن الفلسفة الغربية", true, "فلسفة غربية", 10, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 45, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن الفلسفة الشرقية", true, "فلسفة شرقية", 10, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 46, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "وصفات الطعام العربي", true, "مطبخ عربي", 11, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 47, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "وصفات من مطابخ العالم", true, "مطبخ عالمي", 11, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 48, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "سير ذاتية لشخصيات سياسية", true, "سير سياسيين", 12, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 49, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "سير ذاتية لفنانين ومبدعين", true, "سير فنانين", 12, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 50, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن الرسم والنحت", true, "فنون تشكيلية", 13, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 51, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن الموسيقى والآلات", true, "موسيقى", 13, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 52, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن البرمجة وتطوير البرمجيات", true, "برمجة", 14, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 53, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن الذكاء الاصطناعي", true, "ذكاء اصطناعي", 14, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 54, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن الطب العام والصحة", true, "طب عام", 15, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 55, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن التغذية والصحة", true, "تغذية", 15, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 56, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن التمارين الرياضية", true, "لياقة بدنية", 16, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 57, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن كرة القدم وغيرها", true, "رياضات جماعية", 16, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 58, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن السفر والمغامرات", true, "رحلات مغامرة", 17, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 59, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "أدلة للسفر والسياحة", true, "أدلة سياحية", 17, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 60, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب عن إدارة الشركات", true, "إدارة الأعمال", 18, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "BookCategories",
                columns: new[] { "BookId", "CategoryId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 1 },
                    { 3, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
