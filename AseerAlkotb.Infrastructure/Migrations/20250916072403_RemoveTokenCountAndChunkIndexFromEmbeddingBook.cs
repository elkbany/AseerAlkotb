using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTokenCountAndChunkIndexFromEmbeddingBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

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
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
    }
}
