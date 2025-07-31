using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PINChat.Persistence.Db.Migrations
{
    /// <inheritdoc />
    public partial class FixContactrelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_AspNetUsers_ApplicationUserId",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_ApplicationUserId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Contacts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Contacts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ApplicationUserId",
                table: "Contacts",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_AspNetUsers_ApplicationUserId",
                table: "Contacts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
