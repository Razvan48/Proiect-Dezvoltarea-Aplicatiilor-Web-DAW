using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class _12345 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Award_Answers_AnswerId",
                table: "Award");

            migrationBuilder.DropForeignKey(
                name: "FK_Award_Discussions_DiscussionId",
                table: "Award");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Award",
                table: "Award");

            migrationBuilder.RenameTable(
                name: "Award",
                newName: "Awards");

            migrationBuilder.RenameIndex(
                name: "IX_Award_DiscussionId",
                table: "Awards",
                newName: "IX_Awards_DiscussionId");

            migrationBuilder.RenameIndex(
                name: "IX_Award_AnswerId",
                table: "Awards",
                newName: "IX_Awards_AnswerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Awards",
                table: "Awards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Awards_Answers_AnswerId",
                table: "Awards",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Awards_Discussions_DiscussionId",
                table: "Awards",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Awards_Answers_AnswerId",
                table: "Awards");

            migrationBuilder.DropForeignKey(
                name: "FK_Awards_Discussions_DiscussionId",
                table: "Awards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Awards",
                table: "Awards");

            migrationBuilder.RenameTable(
                name: "Awards",
                newName: "Award");

            migrationBuilder.RenameIndex(
                name: "IX_Awards_DiscussionId",
                table: "Award",
                newName: "IX_Award_DiscussionId");

            migrationBuilder.RenameIndex(
                name: "IX_Awards_AnswerId",
                table: "Award",
                newName: "IX_Award_AnswerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Award",
                table: "Award",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Award_Answers_AnswerId",
                table: "Award",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Award_Discussions_DiscussionId",
                table: "Award",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id");
        }
    }
}
