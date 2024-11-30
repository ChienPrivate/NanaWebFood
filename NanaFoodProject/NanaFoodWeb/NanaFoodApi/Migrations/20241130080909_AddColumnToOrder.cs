using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanaFoodApi.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0bd424b3-04a9-4440-8076-f3f034d74c27");*/

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelDate",
                table: "Order",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelUserFullName",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CancelUserId",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelUserName",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelUserRoles",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelDate",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CancelUserFullName",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CancelUserId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CancelUserName",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CancelUserRoles",
                table: "Order");

            /*migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0bd424b3-04a9-4440-8076-f3f034d74c27", null, "employee", "EMPLOYEE" });*/
        }
    }
}
