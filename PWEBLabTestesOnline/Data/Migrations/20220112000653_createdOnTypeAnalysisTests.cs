using Microsoft.EntityFrameworkCore.Migrations;

namespace PWEBLabTestesOnline.Data.Migrations
{
    public partial class createdOnTypeAnalysisTests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "TypeAnalysisTests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypeAnalysisTests_CreatedById",
                table: "TypeAnalysisTests",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_TypeAnalysisTests_AspNetUsers_CreatedById",
                table: "TypeAnalysisTests",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TypeAnalysisTests_AspNetUsers_CreatedById",
                table: "TypeAnalysisTests");

            migrationBuilder.DropIndex(
                name: "IX_TypeAnalysisTests_CreatedById",
                table: "TypeAnalysisTests");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "TypeAnalysisTests");
        }
    }
}
