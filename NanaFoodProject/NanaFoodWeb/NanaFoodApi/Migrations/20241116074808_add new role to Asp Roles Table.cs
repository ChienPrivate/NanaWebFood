﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanaFoodApi.Migrations
{
    /// <inheritdoc />
    public partial class addnewroletoAspRolesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b33f1cd7-07ed-4eb4-a0c4-2e5bb2cf4f7a", null, "employee", "EMPLOYEE" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b33f1cd7-07ed-4eb4-a0c4-2e5bb2cf4f7a");
        }
    }
}
