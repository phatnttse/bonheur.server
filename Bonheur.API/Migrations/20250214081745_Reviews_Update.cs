using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bonheur.API.Migrations
{
    /// <inheritdoc />
    public partial class Reviews_Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rate",
                table: "Reviews",
                newName: "ValueForMoney");

            migrationBuilder.AddColumn<int>(
                name: "Flexibility",
                table: "Reviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Professionalism",
                table: "Reviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QualityOfService",
                table: "Reviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ResponseTime",
                table: "Reviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SummaryExperience",
                table: "Reviews",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Flexibility",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Professionalism",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "QualityOfService",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ResponseTime",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "SummaryExperience",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "ValueForMoney",
                table: "Reviews",
                newName: "Rate");
        }
    }
}
