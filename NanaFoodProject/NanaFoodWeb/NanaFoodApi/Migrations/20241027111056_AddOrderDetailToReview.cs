using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanaFoodApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderDetailToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "Review",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Review",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Review_OrderId_ProductId",
                table: "Review",
                columns: new[] { "OrderId", "ProductId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_OrderDetails_OrderId_ProductId",
                table: "Review",
                columns: new[] { "OrderId", "ProductId" },
                principalTable: "OrderDetails",
                principalColumns: new[] { "ProductId", "OrderId" },
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_OrderDetails_OrderId_ProductId",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_OrderId_ProductId",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Review");
        }
    }
}
