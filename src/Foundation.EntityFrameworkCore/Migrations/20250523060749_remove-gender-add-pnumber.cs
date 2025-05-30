using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Migrations
{
    /// <inheritdoc />
    public partial class removegenderaddpnumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Patients",
                newName: "PatientNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PatientNumber",
                table: "Patients",
                newName: "Gender");
        }
    }
}
