using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class AddRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Discussions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_UserId",
                table: "Discussions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discussions_AspNetUsers_UserId",
                table: "Discussions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discussions_AspNetUsers_UserId",
                table: "Discussions");

            migrationBuilder.DropIndex(
                name: "IX_Discussions_UserId",
                table: "Discussions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Discussions");
        }
    }
}
