using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Migrations
{
    /// <inheritdoc />
    public partial class TestDbForTestUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MainUsers",
                table: "MainUsers");

            migrationBuilder.RenameTable(
                name: "MainUsers",
                newName: "MainUser");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MainUser",
                table: "MainUser",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MainUser",
                table: "MainUser");

            migrationBuilder.RenameTable(
                name: "MainUser",
                newName: "MainUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MainUsers",
                table: "MainUsers",
                column: "Id");
        }
    }
}
