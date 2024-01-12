using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class newdatabsecodespaces : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Codespaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerId = table.Column<int>(type: "int", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Codespaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Codespaces_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Codespaces_AnswerId",
                table: "Codespaces",
                column: "AnswerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Codespaces");
        }
    }
}
