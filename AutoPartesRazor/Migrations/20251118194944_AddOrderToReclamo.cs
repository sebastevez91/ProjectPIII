using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPartesRazor.Migrations
{
    public partial class AddOrderToReclamo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Reclamo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reclamo_OrderId",
                table: "Reclamo",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reclamo_Orders_OrderId",
                table: "Reclamo",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reclamo_Orders_OrderId",
                table: "Reclamo");

            migrationBuilder.DropIndex(
                name: "IX_Reclamo_OrderId",
                table: "Reclamo");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Reclamo");
        }
    }
}
