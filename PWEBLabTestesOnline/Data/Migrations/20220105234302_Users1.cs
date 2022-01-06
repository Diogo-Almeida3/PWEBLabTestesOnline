using Microsoft.EntityFrameworkCore.Migrations;

namespace PWEBLabTestesOnline.Data.Migrations
{
    public partial class Users1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientViewModelId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClientViewModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientViewModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ClientViewModelId",
                table: "AspNetUsers",
                column: "ClientViewModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ClientViewModel_ClientViewModelId",
                table: "AspNetUsers",
                column: "ClientViewModelId",
                principalTable: "ClientViewModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ClientViewModel_ClientViewModelId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ClientViewModel");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ClientViewModelId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ClientViewModelId",
                table: "AspNetUsers");
        }
    }
}
