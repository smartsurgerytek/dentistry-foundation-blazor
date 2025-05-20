using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Migrations
{
    /// <inheritdoc />
    public partial class TestDbForMainUser_AddAgeAndPcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "MainUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PinCode",
                table: "MainUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "MainUsers");

            migrationBuilder.DropColumn(
                name: "PinCode",
                table: "MainUsers");
        }
    }
}
