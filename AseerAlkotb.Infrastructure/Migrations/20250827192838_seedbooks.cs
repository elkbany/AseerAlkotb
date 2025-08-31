using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedbooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "CoverImageUrl", "CreatedAt", "Description", "DiscountPercentage", "Format", "ISBN", "IsActive", "Language", "PageCount", "Price", "PublishedDate", "PublisherId", "SalesCount", "StockQuantity", "Title", "UpdatedAt", "ViewCount" },
                values: new object[,]
                {
                    { 4, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رواية سياسية لنجيب محفوظ", 10m, "ورقي", "1234567890126", true, 1, 250, 170m, new DateTime(1974, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 18, "الكرنك", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 5, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رواية مثيرة للجدل لنجيب محفوظ", 20m, "ورقي", "1234567890127", true, 1, 300, 200m, new DateTime(1959, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 30, "أولاد حارتنا", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 6, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ملحمة أدبية لنجيب محفوظ", 12m, "ورقي", "1234567890128", true, 1, 350, 180m, new DateTime(1977, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 20, "الحرافيش", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 7, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "الجزء الأول من ثلاثية نجيب محفوظ", 8m, "ورقي", "1234567890129", true, 1, 270, 150m, new DateTime(1956, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 22, "بين القصرين", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 8, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "الجزء الثاني من ثلاثية نجيب محفوظ", 6m, "ورقي", "1234567890130", true, 1, 280, 160m, new DateTime(1957, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 20, "قصر الشوق", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 9, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "الجزء الثالث من ثلاثية نجيب محفوظ", 7m, "ورقي", "1234567890131", true, 1, 290, 170m, new DateTime(1957, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 18, "السكرية", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 10, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رواية اجتماعية لنجيب محفوظ", 5m, "ورقي", "1234567890132", true, 1, 210, 140m, new DateTime(1946, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 20, "خان الخليلي", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 11, 1, "/uploads/Books/6fdbe335-1a34-4564-9564-e30dc38cf6ea.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "وصف الكتاب الجديد", 15m, "ورقي", "9876543210178", true, 1, 350, 200m, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 30, "كتاب جديد", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 12, 2, "/uploads/Books/6fdbe335-1a34-4564-9564-e30dc38cf6ea.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "وصف الكتاب الآخر", 5m, "ورقي", "1234567890177", true, 1, 300, 250m, new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0, 15, "كتاب آخر", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 13, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رواية شهيرة ليحيى الطاهر عبدالله", 0m, "ورقي", "1234567890133", true, 1, 190, 120m, new DateTime(1975, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 20, "الطوق والأسورة", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 14, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رواية غسان كنفاني الشهيرة", 10m, "ورقي", "1234567890134", true, 1, 160, 130m, new DateTime(1963, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 20, "رجال في الشمس", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 15, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رواية الطيب صالح الأشهر", 15m, "ورقي", "1234567890135", true, 1, 220, 150m, new DateTime(1966, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 20, "موسم الهجرة إلى الشمال", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 16, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رواية محمد شكري الذاتية", 5m, "ورقي", "1234567890136", true, 1, 200, 140m, new DateTime(1973, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 20, "الخبز الحافي", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 17, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رواية يوسف زيدان التاريخية", 10m, "ورقي", "1234567890137", true, 1, 350, 160m, new DateTime(2008, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 20, "عزازيل", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 18, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رواية قصيرة لغسان كنفاني", 0m, "ورقي", "1234567890138", true, 1, 100, 110m, new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 20, "عائد إلى حيفا", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 19, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رواية غابرييل غارسيا ماركيز المترجمة للعربية", 20m, "ورقي", "1234567890139", true, 1, 420, 170m, new DateTime(1985, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 20, "الحب في زمن الكوليرا", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 20, 1, "/uploads/Books/94147f51-d713-47a3-86dd-f88eef6198d4.webp", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رواية عبد الرحمن منيف الشهيرة", 10m, "ورقي", "1234567890140", true, 1, 500, 180m, new DateTime(1984, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 20, "مدن الملح", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 }
                });

            migrationBuilder.InsertData(
                table: "BookCategories",
                columns: new[] { "BookId", "CategoryId" },
                values: new object[,]
                {
                    { 4, 2 },
                    { 4, 3 },
                    { 5, 1 },
                    { 5, 2 },
                    { 5, 4 },
                    { 6, 1 },
                    { 6, 2 },
                    { 6, 5 },
                    { 7, 1 },
                    { 7, 2 },
                    { 7, 6 },
                    { 8, 1 },
                    { 8, 2 },
                    { 8, 6 },
                    { 9, 1 },
                    { 9, 2 },
                    { 9, 6 },
                    { 10, 1 },
                    { 10, 2 },
                    { 11, 2 },
                    { 11, 7 },
                    { 12, 1 },
                    { 12, 3 },
                    { 13, 1 },
                    { 13, 2 },
                    { 14, 2 },
                    { 14, 3 },
                    { 15, 1 },
                    { 15, 2 },
                    { 15, 8 },
                    { 16, 1 },
                    { 16, 2 },
                    { 16, 9 },
                    { 17, 2 },
                    { 17, 10 },
                    { 18, 2 },
                    { 18, 3 },
                    { 19, 11 },
                    { 19, 12 },
                    { 20, 1 },
                    { 20, 2 },
                    { 20, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 5, 1 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 5, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 5, 4 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 6, 1 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 6, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 6, 5 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 7, 1 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 7, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 7, 6 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 8, 1 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 8, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 8, 6 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 9, 1 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 9, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 9, 6 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 10, 1 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 10, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 11, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 11, 7 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 12, 1 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 12, 3 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 13, 1 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 13, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 14, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 14, 3 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 15, 1 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 15, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 15, 8 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 16, 1 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 16, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 16, 9 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 17, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 17, 10 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 18, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 18, 3 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 19, 11 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 19, 12 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 20, 1 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 20, 2 });

            migrationBuilder.DeleteData(
                table: "BookCategories",
                keyColumns: new[] { "BookId", "CategoryId" },
                keyValues: new object[] { 20, 3 });

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 20);
        }
    }
}
