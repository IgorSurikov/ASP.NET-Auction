using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Auction.Migrations
{
    public partial class ProductLotModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductLot",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LotName = table.Column<string>(maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    StartPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    CurrentPrice = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    StartTrading = table.Column<DateTime>(nullable: false),
                    EndTrading = table.Column<DateTime>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    OwnerAuctionUserId = table.Column<string>(nullable: true),
                    CustomerAuctionUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLot", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProductLot_AspNetUsers_CustomerAuctionUserId",
                        column: x => x.CustomerAuctionUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductLot_AspNetUsers_OwnerAuctionUserId",
                        column: x => x.OwnerAuctionUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductLot_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductLot_CustomerAuctionUserId",
                table: "ProductLot",
                column: "CustomerAuctionUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLot_OwnerAuctionUserId",
                table: "ProductLot",
                column: "OwnerAuctionUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLot_ProductId",
                table: "ProductLot",
                column: "ProductId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductLot");
        }
    }
}
