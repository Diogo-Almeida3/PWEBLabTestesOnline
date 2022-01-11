using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PWEBLabTestesOnline.Data.Migrations
{
    public partial class removeAnalisesTestes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisTests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalysisTests",
                columns: table => new
                {
                    AnalysisTestsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TechnicianName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeAnalysisTestsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisTests", x => x.AnalysisTestsId);
                    table.ForeignKey(
                        name: "FK_AnalysisTests_TypeAnalysisTests_TypeAnalysisTestsId",
                        column: x => x.TypeAnalysisTestsId,
                        principalTable: "TypeAnalysisTests",
                        principalColumn: "TypeAnalysisTestsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalysisTests_TypeAnalysisTestsId",
                table: "AnalysisTests",
                column: "TypeAnalysisTestsId");
        }
    }
}
