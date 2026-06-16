using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motivation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActualHoursToEmployeePosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StandardHoursValue",
                table: "standard_hours",
                newName: "Hours");

            migrationBuilder.RenameIndex(
                name: "IX_standard_hours_Month_Year_StandardHoursValue",
                table: "standard_hours",
                newName: "IX_standard_hours_Month_Year_Hours");

            migrationBuilder.AddColumn<string>(
                name: "ActualHoursHistory",
                table: "employee_positions",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualHoursHistory",
                table: "employee_positions");

            migrationBuilder.RenameColumn(
                name: "Hours",
                table: "standard_hours",
                newName: "StandardHoursValue");

            migrationBuilder.RenameIndex(
                name: "IX_standard_hours_Month_Year_Hours",
                table: "standard_hours",
                newName: "IX_standard_hours_Month_Year_StandardHoursValue");
        }
    }
}
