using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuoteApp.Migrations
{
    /// <inheritdoc />
    public partial class Createinitialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Quote",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Quote");
        }
    }
}
