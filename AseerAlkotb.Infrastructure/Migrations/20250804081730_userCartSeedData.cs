using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class userCartSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cart",
                columns: new[] { "Id", "CreatedAt", "UpdatedAt", "UserId" },
                values: new object[] { 1, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CartId", "CreatedAt", "DateOfBirth", "FirstName", "Gender", "IsActive", "LastName", "UpdatedAt" },
                values: new object[] { 1, 1, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ahmed", 0, true, "Hassan", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cart",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
