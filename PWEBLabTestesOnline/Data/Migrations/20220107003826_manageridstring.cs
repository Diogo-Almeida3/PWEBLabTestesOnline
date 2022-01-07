using Microsoft.EntityFrameworkCore.Migrations;

namespace PWEBLabTestesOnline.Data.Migrations
{
    public partial class manageridstring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Laboratories_AspNetUsers_ManagerId1",
                table: "Laboratories");

            migrationBuilder.DropIndex(
                name: "IX_Laboratories_ManagerId1",
                table: "Laboratories");

            migrationBuilder.DropColumn(
                name: "ManagerId1",
                table: "Laboratories");

            migrationBuilder.AlterColumn<string>(
                name: "ManagerId",
                table: "Laboratories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Laboratories_ManagerId",
                table: "Laboratories",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Laboratories_AspNetUsers_ManagerId",
                table: "Laboratories",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Laboratories_AspNetUsers_ManagerId",
                table: "Laboratories");

            migrationBuilder.DropIndex(
                name: "IX_Laboratories_ManagerId",
                table: "Laboratories");

            migrationBuilder.AlterColumn<int>(
                name: "ManagerId",
                table: "Laboratories",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ManagerId1",
                table: "Laboratories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Laboratories_ManagerId1",
                table: "Laboratories",
                column: "ManagerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Laboratories_AspNetUsers_ManagerId1",
                table: "Laboratories",
                column: "ManagerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
