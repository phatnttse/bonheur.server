using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bonheur.API.Migrations
{
    /// <inheritdoc />
    public partial class Update_OrderDetail_V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_AdPackages_AdPackageId",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "AdPackageId",
                table: "OrderDetails",
                newName: "AdvertisementId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_AdPackageId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_AdvertisementId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Advertisements_AdvertisementId",
                table: "OrderDetails",
                column: "AdvertisementId",
                principalTable: "Advertisements",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Advertisements_AdvertisementId",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "AdvertisementId",
                table: "OrderDetails",
                newName: "AdPackageId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_AdvertisementId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_AdPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_AdPackages_AdPackageId",
                table: "OrderDetails",
                column: "AdPackageId",
                principalTable: "AdPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
