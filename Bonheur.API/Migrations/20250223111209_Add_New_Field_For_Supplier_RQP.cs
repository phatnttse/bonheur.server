using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bonheur.API.Migrations
{
    /// <inheritdoc />
    public partial class Add_New_Field_For_Supplier_RQP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseTime",
                table: "Suppliers");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ResponseTimeEnd",
                table: "Suppliers",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ResponseTimeStart",
                table: "Suppliers",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalReview",
                table: "Suppliers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "View",
                table: "Suppliers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "RequestPricings",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestPricings_UserId",
                table: "RequestPricings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestPricings_AspNetUsers_UserId",
                table: "RequestPricings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestPricings_AspNetUsers_UserId",
                table: "RequestPricings");

            migrationBuilder.DropIndex(
                name: "IX_RequestPricings_UserId",
                table: "RequestPricings");

            migrationBuilder.DropColumn(
                name: "ResponseTimeEnd",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ResponseTimeStart",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "TotalReview",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "View",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RequestPricings");

            migrationBuilder.AddColumn<string>(
                name: "ResponseTime",
                table: "Suppliers",
                type: "text",
                nullable: true);
        }
    }
}
