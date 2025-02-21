using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bonheur.API.Migrations
{
    /// <inheritdoc />
    public partial class Add_Longitude_Latitue_Supplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Suppliers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "Suppliers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Suppliers");
        }
    }
}
