using Microsoft.EntityFrameworkCore.Migrations;

namespace PWEBLabTestesOnline.Data.Migrations
{
    public partial class checklists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentChecklistId",
                table: "Vacancies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Schedules",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChecklistId",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChecklistId",
                table: "Procedure",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Checklist",
                columns: table => new
                {
                    ChecklistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checklist", x => x.ChecklistId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vacancies_CurrentChecklistId",
                table: "Vacancies",
                column: "CurrentChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_ChecklistId",
                table: "Schedules",
                column: "ChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_ChecklistId",
                table: "Procedure",
                column: "ChecklistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Procedure_Checklist_ChecklistId",
                table: "Procedure",
                column: "ChecklistId",
                principalTable: "Checklist",
                principalColumn: "ChecklistId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Checklist_ChecklistId",
                table: "Schedules",
                column: "ChecklistId",
                principalTable: "Checklist",
                principalColumn: "ChecklistId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vacancies_Checklist_CurrentChecklistId",
                table: "Vacancies",
                column: "CurrentChecklistId",
                principalTable: "Checklist",
                principalColumn: "ChecklistId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Procedure_Checklist_ChecklistId",
                table: "Procedure");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Checklist_ChecklistId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacancies_Checklist_CurrentChecklistId",
                table: "Vacancies");

            migrationBuilder.DropTable(
                name: "Checklist");

            migrationBuilder.DropIndex(
                name: "IX_Vacancies_CurrentChecklistId",
                table: "Vacancies");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_ChecklistId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Procedure_ChecklistId",
                table: "Procedure");

            migrationBuilder.DropColumn(
                name: "CurrentChecklistId",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "ChecklistId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "ChecklistId",
                table: "Procedure");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Schedules",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
