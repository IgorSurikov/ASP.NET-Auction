using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Auction.Migrations
{
    public partial class update_transaction_model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InsertDateTime",
                table: "Transaction",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsertDateTime",
                table: "Transaction");
        }
    }
}
