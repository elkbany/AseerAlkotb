using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AseerAlkotb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addEnglishAtt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add English columns to all tables
            migrationBuilder.AddColumn<string>(
                name: "Bio_en",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName_en",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName_en",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nationality_en",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description_en",
                table: "Publishers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name_en",
                table: "Publishers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description_en",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name_en",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description_en",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title_en",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bio_en",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name_en",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: true);

            // Populate English columns with Arabic data for all existing records
            // This will copy Arabic data to English columns as fallback

            // Users table
            migrationBuilder.Sql(@"
                UPDATE Users 
                SET Bio_en = Bio,
                    FirstName_en = FirstName,
                    LastName_en = LastName,
                    Nationality_en = Nationality
                WHERE Bio_en IS NULL OR FirstName_en IS NULL OR LastName_en IS NULL OR Nationality_en IS NULL
            ");
            // Publishers table
            migrationBuilder.Sql(@"
                UPDATE Publishers 
                SET Description_en = Description,
                    Name_en = Name
                WHERE Description_en IS NULL OR Name_en IS NULL
            ");

            // Categories table
            migrationBuilder.Sql(@"
                UPDATE Categories 
                SET Description_en = Description,
                    Name_en = Name
                WHERE Description_en IS NULL OR Name_en IS NULL
            ");

            // Books table
            migrationBuilder.Sql(@"
                UPDATE Books 
                SET Description_en = Description,
                    Title_en = Title
                WHERE Description_en IS NULL OR Title_en IS NULL
            ");

            // Authors table
            migrationBuilder.Sql(@"
                UPDATE Authors 
                SET Bio_en = Bio,
                    Name_en = Name
                WHERE Bio_en IS NULL OR Name_en IS NULL
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio_en",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName_en",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName_en",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Nationality_en",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Comment_en",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Comment_en",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "Description_en",
                table: "Publishers");

            migrationBuilder.DropColumn(
                name: "Name_en",
                table: "Publishers");

            migrationBuilder.DropColumn(
                name: "Name_en",
                table: "Governorates");

            migrationBuilder.DropColumn(
                name: "Name_en",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "Description_en",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Name_en",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Description_en",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Title_en",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Bio_en",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "Name_en",
                table: "Authors");
        }
    }
}