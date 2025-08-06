using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Bio", "CreatedAt", "ImageUrl", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "أديب مصري", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, "نجيب محفوظ", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "كاتب مصري", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, "أحمد خالد توفيق", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "كاتب مصري", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, "يوسف إدريس", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
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
                    { 1, 1, "cover.jpg", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "رواية شهيرة لنجيب محفوظ", 10m, "ورقي", "1234567890123", true, 1, 240, 150m, new DateTime(1950, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 20, "زقاق المدق", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 2, 1, "new_cover.jpg", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "وصف الكتاب الجديد", 15m, "ورقي", "9876543210123", true, 1, 350, 200m, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 30, "كتاب جديد", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 3, 2, "another_cover.jpg", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "وصف الكتاب الآخر", 5m, "ورقي", "1234567890124", true, 1, 300, 250m, new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0, 15, "كتاب آخر", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0 }
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
        }
    }
}
