using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanaFoodApi.Migrations
{
    /// <inheritdoc />
    public partial class ModifyNullableInCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Coupons_CouponCode",
                table: "Cart");

            migrationBuilder.AlterColumn<string>(
                name: "CouponCode",
                table: "Cart",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Coupon_CouponCode",
                table: "Cart",
                column: "CouponCode",
                principalTable: "Coupons",
                principalColumn: "CouponCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Coupons_CouponCode",
                table: "Cart");

            migrationBuilder.AlterColumn<string>(
                name: "CouponCode",
                table: "Cart",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Coupon_CouponCode",
                table: "Cart",
                column: "CouponCode",
                principalTable: "Coupons",
                principalColumn: "CouponCode",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
