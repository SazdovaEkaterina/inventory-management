using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InventoryManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedItemInstances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Items");

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SerialNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    HolderId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemInstance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemInstance_InventoryManagementUsers_HolderId",
                        column: x => x.HolderId,
                        principalTable: "InventoryManagementUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemInstance_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemInstance_HolderId",
                table: "Product",
                column: "HolderId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemInstance_ItemId",
                table: "Product",
                column: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
