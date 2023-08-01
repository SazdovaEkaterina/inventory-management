using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameProductHolderProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_InventoryManagementUsers_HolderId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "HolderId",
                table: "Products",
                newName: "InventoryManagementUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_HolderId",
                table: "Products",
                newName: "IX_Products_InventoryManagementUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_InventoryManagementUsers_InventoryManagementUserId",
                table: "Products",
                column: "InventoryManagementUserId",
                principalTable: "InventoryManagementUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_InventoryManagementUsers_InventoryManagementUserId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "InventoryManagementUserId",
                table: "Products",
                newName: "HolderId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_InventoryManagementUserId",
                table: "Products",
                newName: "IX_Products_HolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_InventoryManagementUsers_HolderId",
                table: "Products",
                column: "HolderId",
                principalTable: "InventoryManagementUsers",
                principalColumn: "Id");
        }
    }
}
