using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoPartesRazor.Migrations
{
    public partial class ChangesInTableClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Brand_Brandid",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_Categoryid",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_Brandid",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_Categoryid",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Brandid",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Categoryid",
                table: "Product");

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

            migrationBuilder.AlterColumn<int>(
                name: "idCategory",
                table: "Product",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "idBrand",
                table: "Product",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Client",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.CreateIndex(
                name: "IX_Product_idBrand",
                table: "Product",
                column: "idBrand");

            migrationBuilder.CreateIndex(
                name: "IX_Product_idCategory",
                table: "Product",
                column: "idCategory");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Brand_idBrand",
                table: "Product",
                column: "idBrand",
                principalTable: "Brand",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_idCategory",
                table: "Product",
                column: "idCategory",
                principalTable: "Category",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Brand_idBrand",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_idCategory",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_idBrand",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_idCategory",
                table: "Product");

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

            migrationBuilder.AlterColumn<int>(
                name: "idCategory",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "idBrand",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Brandid",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Categoryid",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "phone",
                table: "Client",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

            migrationBuilder.CreateIndex(
                name: "IX_Product_Brandid",
                table: "Product",
                column: "Brandid");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Categoryid",
                table: "Product",
                column: "Categoryid");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Brand_Brandid",
                table: "Product",
                column: "Brandid",
                principalTable: "Brand",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_Categoryid",
                table: "Product",
                column: "Categoryid",
                principalTable: "Category",
                principalColumn: "id");
        }
    }
}
