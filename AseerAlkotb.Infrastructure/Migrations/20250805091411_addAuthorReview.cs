using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAuthorReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BookId",
                table: "Reviews",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AuthorId",
                table: "Reviews",
                column: "AuthorId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Review_OneTarget",
                table: "Reviews",
                sql: "(BookId IS NOT NULL AND AuthorId IS NULL) OR (BookId IS NULL AND AuthorId IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Authors_AuthorId",
                table: "Reviews",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Authors_AuthorId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_AuthorId",
                table: "Reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Review_OneTarget",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "BookId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
