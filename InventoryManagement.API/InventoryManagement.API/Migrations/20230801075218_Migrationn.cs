using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class Migrationn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemInstance_InventoryManagementUsers_HolderId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemInstance_Items_ItemId",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemInstance",
                table: "Product");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "ItemInstances");

            migrationBuilder.RenameIndex(
                name: "IX_ItemInstance_ItemId",
                table: "ItemInstances",
                newName: "IX_ItemInstances_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemInstance_HolderId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                newName: "Product");

            migrationBuilder.RenameIndex(
                name: "IX_ItemInstances_ItemId",
                table: "Product",
                newName: "IX_ItemInstance_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemInstances_HolderId",
                table: "Product",
                newName: "IX_ItemInstance_HolderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemInstance",
                table: "Product",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemInstance_InventoryManagementUsers_HolderId",
                table: "Product",
                column: "HolderId",
                principalTable: "InventoryManagementUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemInstance_Items_ItemId",
                table: "Product",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
