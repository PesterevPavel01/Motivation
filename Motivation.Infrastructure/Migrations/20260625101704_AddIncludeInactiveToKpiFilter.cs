using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motivation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIncludeInactiveToKpiFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Include",
                table: "kpi_filters",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Include",
                table: "kpi_filters");
        }
    }
}
