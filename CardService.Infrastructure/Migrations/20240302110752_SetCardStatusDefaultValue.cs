using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SetCardStatusDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cards_Name",
                table: "Cards");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Cards",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "ToDo",
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_Name",
                table: "Cards",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cards_Name",
                table: "Cards");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Cards",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64,
                oldDefaultValue: "ToDo");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_Name",
                table: "Cards",
                column: "Name");
        }
    }
}
