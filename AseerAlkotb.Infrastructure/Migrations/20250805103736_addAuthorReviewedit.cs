using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAuthorReviewedit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReviewFor",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewFor",
                table: "Reviews");
        }
    }
}
