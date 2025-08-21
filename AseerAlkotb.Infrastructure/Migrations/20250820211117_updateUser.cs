using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

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
                    { 1, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب روائية", true, "روايات", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب تاريخية", true, "تاريخ", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "كتب علمية", true, "علوم", null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
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
    }
}
