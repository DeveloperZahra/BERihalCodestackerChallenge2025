using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BERihalCodestackerChallenge2025.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCaseAndRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaseId",
                table: "CrimeReports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CrimeReports_CaseId",
                table: "CrimeReports",
                column: "CaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CrimeReports_Cases_CaseId",
                table: "CrimeReports",
                column: "CaseId",
                principalTable: "Cases",
                principalColumn: "CaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrimeReports_Cases_CaseId",
                table: "CrimeReports");

            migrationBuilder.DropIndex(
                name: "IX_CrimeReports_CaseId",
                table: "CrimeReports");

            migrationBuilder.DropColumn(
                name: "CaseId",
                table: "CrimeReports");
        }
    }
}
