using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPartesRazor.Migrations
{
    public partial class emaman : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "Orders",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CouponId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CouponId1",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalTotal",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CouponId1",
                table: "Orders",
                column: "CouponId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Coupons_CouponId1",
                table: "Orders",
                column: "CouponId1",
                principalTable: "Coupons",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Coupons_CouponId1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CouponId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CouponId1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OriginalTotal",
                table: "Orders");
        }
    }
}
