using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bonheur.API.Migrations
{
    /// <inheritdoc />
    public partial class Add_Role_Message : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverRole",
                table: "Messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderRole",
                table: "Messages",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverRole",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SenderRole",
                table: "Messages");
        }
    }
}
