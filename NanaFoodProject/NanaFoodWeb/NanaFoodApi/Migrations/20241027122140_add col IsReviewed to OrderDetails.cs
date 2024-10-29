﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NanaFoodApi.Migrations
{
    /// <inheritdoc />
    public partial class addcolIsReviewedtoOrderDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedDate",
                table: "Review",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "OrderDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewedDate",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "OrderDetails");
        }
    }
}