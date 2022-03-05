using Microsoft.EntityFrameworkCore.Migrations;

namespace Mango.Services.ShoppingCardAPI.Migrations
{
    public partial class changeCardToCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardDetail_CardHeader_CardHeaderId",
                table: "CardDetail");

            migrationBuilder.RenameColumn(
                name: "CardHeaderId",
                table: "CardDetail",
                newName: "CartHeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_CardDetail_CardHeaderId",
                table: "CardDetail",
                newName: "IX_CardDetail_CartHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardDetail_CardHeader_CartHeaderId",
                table: "CardDetail",
                column: "CartHeaderId",
                principalTable: "CardHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CardDetail_CardHeader_CartHeaderId",
                table: "CardDetail");

            migrationBuilder.RenameColumn(
                name: "CartHeaderId",
                table: "CardDetail",
                newName: "CardHeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_CardDetail_CartHeaderId",
                table: "CardDetail",
                newName: "IX_CardDetail_CardHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CardDetail_CardHeader_CardHeaderId",
                table: "CardDetail",
                column: "CardHeaderId",
                principalTable: "CardHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
