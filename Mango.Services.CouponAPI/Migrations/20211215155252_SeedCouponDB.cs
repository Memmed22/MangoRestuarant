using Microsoft.EntityFrameworkCore.Migrations;

namespace Mango.Services.CouponAPI.Migrations
{
    public partial class SeedCouponDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Coupon",
                columns: new[] { "Id", "CouponCode", "Discount" },
                values: new object[] { 1, "100FF", 10.0 });

            migrationBuilder.InsertData(
                table: "Coupon",
                columns: new[] { "Id", "CouponCode", "Discount" },
                values: new object[] { 2, "200FF", 20.0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coupon",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Coupon",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
