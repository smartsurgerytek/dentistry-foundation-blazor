using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foundation.Migrations
{
    /// <inheritdoc />
    public partial class examination_report : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExaminationReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Carries18 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries17 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries16 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries15 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries14 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries13 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries12 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries11 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries21 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries22 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries23 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries24 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries25 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries26 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries27 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries28 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries48 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries47 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries46 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries45 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries44 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries43 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries42 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries41 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries31 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries32 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries33 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries34 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries35 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries36 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries37 = table.Column<bool>(type: "boolean", nullable: false),
                    Carries38 = table.Column<bool>(type: "boolean", nullable: false),
                    PE18 = table.Column<int>(type: "integer", nullable: true),
                    PE17 = table.Column<int>(type: "integer", nullable: true),
                    PE16 = table.Column<int>(type: "integer", nullable: true),
                    PE15 = table.Column<int>(type: "integer", nullable: true),
                    PE14 = table.Column<int>(type: "integer", nullable: true),
                    PE13 = table.Column<int>(type: "integer", nullable: true),
                    PE12 = table.Column<int>(type: "integer", nullable: true),
                    PE11 = table.Column<int>(type: "integer", nullable: true),
                    PE21 = table.Column<int>(type: "integer", nullable: true),
                    PE22 = table.Column<int>(type: "integer", nullable: true),
                    PE23 = table.Column<int>(type: "integer", nullable: true),
                    PE24 = table.Column<int>(type: "integer", nullable: true),
                    PE25 = table.Column<int>(type: "integer", nullable: true),
                    PE26 = table.Column<int>(type: "integer", nullable: true),
                    PE27 = table.Column<int>(type: "integer", nullable: true),
                    PE28 = table.Column<int>(type: "integer", nullable: true),
                    PE48 = table.Column<int>(type: "integer", nullable: true),
                    PE47 = table.Column<int>(type: "integer", nullable: true),
                    PE46 = table.Column<int>(type: "integer", nullable: true),
                    PE45 = table.Column<int>(type: "integer", nullable: true),
                    PE44 = table.Column<int>(type: "integer", nullable: true),
                    PE43 = table.Column<int>(type: "integer", nullable: true),
                    PE42 = table.Column<int>(type: "integer", nullable: true),
                    PE41 = table.Column<int>(type: "integer", nullable: true),
                    PE31 = table.Column<int>(type: "integer", nullable: true),
                    PE32 = table.Column<int>(type: "integer", nullable: true),
                    PE33 = table.Column<int>(type: "integer", nullable: true),
                    PE34 = table.Column<int>(type: "integer", nullable: true),
                    PE35 = table.Column<int>(type: "integer", nullable: true),
                    PE36 = table.Column<int>(type: "integer", nullable: true),
                    PE37 = table.Column<int>(type: "integer", nullable: true),
                    PE38 = table.Column<int>(type: "integer", nullable: true),
                    Desc18 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc17 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc16 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc15 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc14 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc13 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc12 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc11 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc21 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc22 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc23 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc24 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc25 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc26 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc27 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc28 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc48 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc47 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc46 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc45 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc44 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc43 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc42 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc41 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc31 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc32 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc33 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc34 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc35 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc36 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc37 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Desc38 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExaminationReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExaminationReports_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExaminationReports_PatientId",
                table: "ExaminationReports",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExaminationReports");
        }
    }
}
