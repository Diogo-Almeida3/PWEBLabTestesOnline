using Microsoft.EntityFrameworkCore.Migrations;

namespace PWEBLabTestesOnline.Data.Migrations
{
    public partial class Techinicians : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LaboratoriesId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LaboratoriesId",
                table: "AspNetUsers",
                column: "LaboratoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Laboratories_LaboratoriesId",
                table: "AspNetUsers",
                column: "LaboratoriesId",
                principalTable: "Laboratories",
                principalColumn: "LaboratoriesId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Laboratories_LaboratoriesId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_LaboratoriesId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LaboratoriesId",
                table: "AspNetUsers");
        }
    }
}
