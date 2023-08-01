using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class ItemInstanceNullableHolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemInstance_InventoryManagementUsers_HolderId",
                table: "Product");

            migrationBuilder.AlterColumn<string>(
                name: "HolderId",
                table: "Product",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemInstance_InventoryManagementUsers_HolderId",
                table: "Product",
                column: "HolderId",
                principalTable: "InventoryManagementUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemInstance_InventoryManagementUsers_HolderId",
                table: "Product");

            migrationBuilder.AlterColumn<string>(
                name: "HolderId",
                table: "Product",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemInstance_InventoryManagementUsers_HolderId",
                table: "Product",
                column: "HolderId",
                principalTable: "InventoryManagementUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
