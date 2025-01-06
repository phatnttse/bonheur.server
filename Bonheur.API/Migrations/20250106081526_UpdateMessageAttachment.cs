using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bonheur.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessageAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageAttachments_Messages_QuotationMessageId",
                table: "MessageAttachments");

            migrationBuilder.RenameColumn(
                name: "QuotationMessageId",
                table: "MessageAttachments",
                newName: "MessageId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageAttachments_QuotationMessageId",
                table: "MessageAttachments",
                newName: "IX_MessageAttachments_MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageAttachments_Messages_MessageId",
                table: "MessageAttachments",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageAttachments_Messages_MessageId",
                table: "MessageAttachments");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "MessageAttachments",
                newName: "QuotationMessageId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageAttachments_MessageId",
                table: "MessageAttachments",
                newName: "IX_MessageAttachments_QuotationMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageAttachments_Messages_QuotationMessageId",
                table: "MessageAttachments",
                column: "QuotationMessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
