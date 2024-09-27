using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NanaFoodApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedingRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "59f54570-7ed9-4325-9fdc-29a48e14e7b1", null, "admin", "ADMIN" },
                    { "ce67e0e7-773d-46b0-8c67-c1d0b33dc482", null, "customer", "CUSTOMER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "59f54570-7ed9-4325-9fdc-29a48e14e7b1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ce67e0e7-773d-46b0-8c67-c1d0b33dc482");
        }
    }
}
