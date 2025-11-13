using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPartesRazor.Migrations
{
    public partial class AddTrackingToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "OrderItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "OrderItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "OrderItem");
        }
    }
}
