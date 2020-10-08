using Microsoft.EntityFrameworkCore.Migrations;

namespace Auction.Migrations
{
    public partial class ProductsLots : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductLot_ProductId",
                table: "ProductLot");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLot_ProductId",
                table: "ProductLot",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductLot_ProductId",
                table: "ProductLot");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLot_ProductId",
                table: "ProductLot",
                column: "ProductId",
                unique: true);
        }
    }
}
