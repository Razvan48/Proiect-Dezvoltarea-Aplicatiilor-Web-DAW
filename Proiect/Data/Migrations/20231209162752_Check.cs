using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class Check : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Discussions_AnswerId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Answers_AnswerId",
                table: "Comments",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Answers_AnswerId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Discussions_AnswerId",
                table: "Comments",
                column: "AnswerId",
                principalTable: "Discussions",
                principalColumn: "Id");
        }
    }
}
