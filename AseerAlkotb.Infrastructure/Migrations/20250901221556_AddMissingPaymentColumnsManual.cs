using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingPaymentColumnsManual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add missing columns to Payments table
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "EGP");
                
            migrationBuilder.AddColumn<long?>(
                name: "PaymobOrderId",
                table: "Payments",
                type: "bigint",
                nullable: true);
                
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 1);
                
            // Create index on UserId for foreign key relationship
            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");
                
            // Add foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UserId",
                table: "Payments");
                
            // Drop index
            migrationBuilder.DropIndex(
                name: "IX_Payments_UserId",
                table: "Payments");
                
            // Drop columns
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Payments");
                
            migrationBuilder.DropColumn(
                name: "PaymobOrderId",
                table: "Payments");
                
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Payments");
        }
    }
}
