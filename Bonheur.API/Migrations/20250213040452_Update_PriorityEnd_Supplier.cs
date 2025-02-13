using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bonheur.API.Migrations
{
    /// <inheritdoc />
    public partial class Update_PriorityEnd_Supplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProrityEnd",
                table: "Suppliers",
                newName: "PriorityEnd");

            migrationBuilder.RenameIndex(
                name: "IX_Suppliers_ProrityEnd",
                table: "Suppliers",
                newName: "IX_Suppliers_PriorityEnd");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PriorityEnd",
                table: "Suppliers",
                newName: "ProrityEnd");

            migrationBuilder.RenameIndex(
                name: "IX_Suppliers_PriorityEnd",
                table: "Suppliers",
                newName: "IX_Suppliers_ProrityEnd");
        }
    }
}
