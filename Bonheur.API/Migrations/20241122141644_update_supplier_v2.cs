using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bonheur.API.Migrations
{
    /// <inheritdoc />
    public partial class update_supplier_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PriorityScore",
                table: "Suppliers",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "PriorityLevel",
                table: "SubscriptionPackages",
                newName: "Priority");

            migrationBuilder.AddColumn<int>(
                name: "OnBoardStatus",
                table: "Suppliers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Suppliers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnBoardStatus",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Suppliers");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Suppliers",
                newName: "PriorityScore");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "SubscriptionPackages",
                newName: "PriorityLevel");
        }
    }
}
