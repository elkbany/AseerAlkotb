using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class wishlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItem_Books_BookId",
                table: "WishlistItem");

            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItem_Wishlists_WishlistId",
                table: "WishlistItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WishlistItem",
                table: "WishlistItem");

            migrationBuilder.DropColumn(
                name: "Governorate",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "WishlistItem",
                newName: "WishlistItems");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItem_WishlistId",
                table: "WishlistItems",
                newName: "IX_WishlistItems_WishlistId");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "Orders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WishlistItems",
                table: "WishlistItems",
                columns: new[] { "BookId", "WishlistId" });

            migrationBuilder.CreateTable(
                name: "UserFollows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: true),
                    PublisherId = table.Column<int>(type: "int", nullable: true),
                    FollowType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollows", x => x.Id);
                    table.CheckConstraint("CK_UserFollow_SingleFollowType", "([AuthorId] IS NOT NULL AND [PublisherId] IS NULL) OR ([AuthorId] IS NULL AND [PublisherId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_UserFollows_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFollows_Publishers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "Publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFollows_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_AuthorId",
                table: "UserFollows",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_PublisherId",
                table: "UserFollows",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_UserId_AuthorId",
                table: "UserFollows",
                columns: new[] { "UserId", "AuthorId" },
                unique: true,
                filter: "[AuthorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_UserId_PublisherId",
                table: "UserFollows",
                columns: new[] { "UserId", "PublisherId" },
                unique: true,
                filter: "[PublisherId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_Books_BookId",
                table: "WishlistItems",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_Wishlists_WishlistId",
                table: "WishlistItems",
                column: "WishlistId",
                principalTable: "Wishlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_Books_BookId",
                table: "WishlistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_Wishlists_WishlistId",
                table: "WishlistItems");

            migrationBuilder.DropTable(
                name: "UserFollows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WishlistItems",
                table: "WishlistItems");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "WishlistItems",
                newName: "WishlistItem");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItems_WishlistId",
                table: "WishlistItem",
                newName: "IX_WishlistItem_WishlistId");

            migrationBuilder.AddColumn<int>(
                name: "Governorate",
                table: "Orders",
                type: "int",
                maxLength: 500,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WishlistItem",
                table: "WishlistItem",
                columns: new[] { "BookId", "WishlistId" });

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItem_Books_BookId",
                table: "WishlistItem",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItem_Wishlists_WishlistId",
                table: "WishlistItem",
                column: "WishlistId",
                principalTable: "Wishlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
