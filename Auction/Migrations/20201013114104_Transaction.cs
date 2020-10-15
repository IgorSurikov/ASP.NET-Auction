using Microsoft.EntityFrameworkCore.Migrations;

namespace Auction.Migrations
{
    public partial class Transaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionAmount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    OwnerAuctionUserId = table.Column<string>(nullable: true),
                    CustomerAuctionUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Transaction_AspNetUsers_CustomerAuctionUserId",
                        column: x => x.CustomerAuctionUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaction_AspNetUsers_OwnerAuctionUserId",
                        column: x => x.OwnerAuctionUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaction_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CustomerAuctionUserId",
                table: "Transaction",
                column: "CustomerAuctionUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_OwnerAuctionUserId",
                table: "Transaction",
                column: "OwnerAuctionUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ProductId",
                table: "Transaction",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transaction");
        }
    }
}
