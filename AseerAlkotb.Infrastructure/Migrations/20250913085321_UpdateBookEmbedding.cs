using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookEmbedding : Migration
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
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<int>(
                name: "ChunkIndex",
                table: "BookEmbeddings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TokenCount",
                table: "BookEmbeddings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookEmbeddings_BookId_ContentType_ChunkIndex",
                table: "BookEmbeddings",
                columns: new[] { "BookId", "ContentType", "ChunkIndex" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookEmbeddings_BookId_ContentType_ChunkIndex",
                table: "BookEmbeddings");

            migrationBuilder.DropColumn(
                name: "ChunkIndex",
                table: "BookEmbeddings");

            migrationBuilder.DropColumn(
                name: "TokenCount",
                table: "BookEmbeddings");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "BookEmbeddings",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

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
