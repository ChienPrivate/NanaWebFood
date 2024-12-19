using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanaFoodApi.Migrations
{
    /// <inheritdoc />
    public partial class updatedeletebehaviourbetweenorderandcoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thay đổi hành vi khóa ngoại của CouponCode trong Order
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Coupon_CouponCode",  // Tên khóa ngoại (có thể thay đổi tùy theo tên đã đặt)
                table: "Order");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Coupon_CouponCode",
                table: "Order",
                column: "CouponCode",
                principalTable: "Coupon",
                principalColumn: "CouponCode",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
