using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editsinAI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookEmbeddings_BookId",
                table: "BookEmbeddings");

            migrationBuilder.DropIndex(
                name: "IX_BookEmbeddings_ContentType",
                table: "BookEmbeddings");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "BookEmbeddings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.CreateIndex(
                name: "IX_BookEmbeddings_BookId_ContentType",
                table: "BookEmbeddings",
                columns: new[] { "BookId", "ContentType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookEmbeddings_BookId_ContentType",
                table: "BookEmbeddings");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "BookEmbeddings",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_BookEmbeddings_BookId",
                table: "BookEmbeddings",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookEmbeddings_ContentType",
                table: "BookEmbeddings",
                column: "ContentType");
        }
    }
}
