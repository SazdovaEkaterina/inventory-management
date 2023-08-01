using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemInstances_InventoryManagementUsers_HolderId",
                table: "ItemInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemInstances_Items_ItemId",
                table: "ItemInstances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemInstances",
                table: "ItemInstances");

            migrationBuilder.RenameTable(
                name: "ItemInstances",
                newName: "Products");

            migrationBuilder.RenameIndex(
                name: "IX_ItemInstances_ItemId",
                table: "Products",
                newName: "IX_Products_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemInstances_HolderId",
                table: "Products",
                newName: "IX_Products_HolderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_InventoryManagementUsers_HolderId",
                table: "Products",
                column: "HolderId",
                principalTable: "InventoryManagementUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Items_ItemId",
                table: "Products",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_InventoryManagementUsers_HolderId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Items_ItemId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "ItemInstances");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ItemId",
                table: "ItemInstances",
                newName: "IX_ItemInstances_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_HolderId",
                table: "ItemInstances",
                newName: "IX_ItemInstances_HolderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemInstances",
                table: "ItemInstances",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemInstances_InventoryManagementUsers_HolderId",
                table: "ItemInstances",
                column: "HolderId",
                principalTable: "InventoryManagementUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemInstances_Items_ItemId",
                table: "ItemInstances",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
