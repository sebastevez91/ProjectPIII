using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPartesRazor.Migrations
{
    public partial class pruebaclientes1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Client",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Client",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "lastName",
                table: "Client",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Client",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "Client",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Client",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Client",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Client",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Client",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Client",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Client",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Client",
                newName: "lastName");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Client",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Client",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Client",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Client",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "lastName",
                table: "Client",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "Client",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
