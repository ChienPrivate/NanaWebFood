using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanaFoodApi.Migrations
{
    /// <inheritdoc />
    public partial class addcouponcodetoOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "Order",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Discount",
                table: "Order",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MinAmount",
                table: "Order",
                type: "float",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_CouponCode",
                table: "Order",
                column: "CouponCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Coupon_CouponCode",
                table: "Order",
                column: "CouponCode",
                principalTable: "Coupon",
                principalColumn: "CouponCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Coupon_CouponCode",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_CouponCode",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "MinAmount",
                table: "Order");
        }
    }
}
