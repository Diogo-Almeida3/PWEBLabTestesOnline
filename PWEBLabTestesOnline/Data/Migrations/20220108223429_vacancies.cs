using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PWEBLabTestesOnline.Data.Migrations
{
    public partial class vacancies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vacancies",
                columns: table => new
                {
                    VacanciesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryId = table.Column<int>(type: "int", nullable: false),
                    TypeAnalysisTestsId = table.Column<int>(type: "int", nullable: false),
                    DailyLimit = table.Column<int>(type: "int", nullable: false),
                    Opening = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Enclosure = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacancies", x => x.VacanciesId);
                    table.ForeignKey(
                        name: "FK_Vacancies_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalTable: "Laboratories",
                        principalColumn: "LaboratoriesId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vacancies_TypeAnalysisTests_TypeAnalysisTestsId",
                        column: x => x.TypeAnalysisTestsId,
                        principalTable: "TypeAnalysisTests",
                        principalColumn: "TypeAnalysisTestsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_LaboratoryId",
                table: "Vacancies",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_TypeAnalysisTestsId",
                table: "Vacancies",
                column: "TypeAnalysisTestsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vacancies");
        }
    }
}
