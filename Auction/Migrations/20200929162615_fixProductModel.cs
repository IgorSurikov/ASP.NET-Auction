using Microsoft.EntityFrameworkCore.Migrations;

namespace Auction.Migrations
{
    public partial class fixProductModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_AspNetUsers_AuctionUserId1",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_AuctionUserId1",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "AuctionUserId1",
                table: "Product");

            migrationBuilder.AlterColumn<string>(
                name: "AuctionUserId",
                table: "Product",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Product_AuctionUserId",
                table: "Product",
                column: "AuctionUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_AspNetUsers_AuctionUserId",
                table: "Product",
                column: "AuctionUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_AspNetUsers_AuctionUserId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_AuctionUserId",
                table: "Product");

            migrationBuilder.AlterColumn<int>(
                name: "AuctionUserId",
                table: "Product",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuctionUserId1",
                table: "Product",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_AuctionUserId1",
                table: "Product",
                column: "AuctionUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_AspNetUsers_AuctionUserId1",
                table: "Product",
                column: "AuctionUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
