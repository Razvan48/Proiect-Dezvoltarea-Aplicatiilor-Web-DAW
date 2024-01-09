using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class awards : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Award",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Award_AnswerId",
                table: "Award",
                column: "AnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Award_Answers_AnswerId",
                table: "Award",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Award_Answers_AnswerId",
                table: "Award");

            migrationBuilder.DropIndex(
                name: "IX_Award_AnswerId",
                table: "Award");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Award");
        }
    }
}
