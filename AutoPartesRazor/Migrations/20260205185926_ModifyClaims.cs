using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPartesRazor.Migrations
{
    public partial class ModifyClaims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AreaAsignada",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Claims",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Claims_OrderId",
                table: "Claims",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Orders_OrderId",
                table: "Claims",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Orders_OrderId",
                table: "Claims");

            migrationBuilder.DropIndex(
                name: "IX_Claims_OrderId",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "AreaAsignada",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Claims");
        }
    }
}
