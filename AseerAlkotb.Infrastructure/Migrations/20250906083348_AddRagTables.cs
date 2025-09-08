using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRagTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookEmbeddings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Embedding = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookEmbeddings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookEmbeddings_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RagQueries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Query = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    QueryType = table.Column<int>(type: "int", nullable: false),
                    SimilarityScore = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RagQueries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookEmbeddings_BookId",
                table: "BookEmbeddings",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookEmbeddings_ContentType",
                table: "BookEmbeddings",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_BookEmbeddings_LastUpdated",
                table: "BookEmbeddings",
                column: "LastUpdated");

            migrationBuilder.CreateIndex(
                name: "IX_RagQueries_CreatedAt",
                table: "RagQueries",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RagQueries_QueryType",
                table: "RagQueries",
                column: "QueryType");

            migrationBuilder.CreateIndex(
                name: "IX_RagQueries_UserId",
                table: "RagQueries",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookEmbeddings");

            migrationBuilder.DropTable(
                name: "RagQueries");
        }
    }
}
