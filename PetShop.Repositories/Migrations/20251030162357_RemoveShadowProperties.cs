using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShadowProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa tất cả shadow foreign key columns
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderId1",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId1",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "Payments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId1",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId1",
                table: "Payments",
                column: "OrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderId1",
                table: "Payments",
                column: "OrderId1",
                principalTable: "Orders",
                principalColumn: "OrderId");
        }
    }
}
