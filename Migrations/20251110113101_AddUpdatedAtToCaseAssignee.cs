using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BERihalCodestackerChallenge2025.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedAtToCaseAssignee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CaseAssignees",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CaseAssignees");
        }
    }
}
