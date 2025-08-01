using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuthorEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookId1",
                table: "Wishlists",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Authors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_BookId1",
                table: "Wishlists",
                column: "BookId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_Books_BookId1",
                table: "Wishlists",
                column: "BookId1",
                principalTable: "Books",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_Books_BookId1",
                table: "Wishlists");

            migrationBuilder.DropIndex(
                name: "IX_Wishlists_BookId1",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "BookId1",
                table: "Wishlists");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Authors",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
