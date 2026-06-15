using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motivation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToStandardHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_standard_hours_Month_Year_StandardHoursValue",
                table: "standard_hours",
                columns: new[] { "Month", "Year", "StandardHoursValue" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_standard_hours_Month_Year_StandardHoursValue",
                table: "standard_hours");
        }
    }
}
