using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bonheur.API.Migrations
{
    /// <inheritdoc />
    public partial class Add_BadgeText_For_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BadgeColor",
                table: "SubscriptionPackages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BadgeText",
                table: "SubscriptionPackages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BadgeTextColor",
                table: "SubscriptionPackages",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BadgeColor",
                table: "SubscriptionPackages");

            migrationBuilder.DropColumn(
                name: "BadgeText",
                table: "SubscriptionPackages");

            migrationBuilder.DropColumn(
                name: "BadgeTextColor",
                table: "SubscriptionPackages");
        }
    }
}
