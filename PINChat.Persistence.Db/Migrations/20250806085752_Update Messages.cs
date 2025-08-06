using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PINChat.Persistence.Db.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MessageType",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "Messages");
        }
    }
}
