using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserInContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_InventoryManagementUsers_InventoryManagementUserId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryManagementUsers",
                table: "InventoryManagementUsers");

            migrationBuilder.RenameTable(
                name: "InventoryManagementUsers",
                newName: "Users");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_InventoryManagementUserId",
                table: "Products",
                column: "InventoryManagementUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_InventoryManagementUserId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "InventoryManagementUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryManagementUsers",
                table: "InventoryManagementUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_InventoryManagementUsers_InventoryManagementUserId",
                table: "Products",
                column: "InventoryManagementUserId",
                principalTable: "InventoryManagementUsers",
                principalColumn: "Id");
        }
    }
}
