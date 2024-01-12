using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class newlins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Codespaces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Codespaces");
        }
    }
}
