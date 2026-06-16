using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motivation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPositionKpiWithKpiId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_position_kpis_kpis_KpiId",
                table: "position_kpis");

            migrationBuilder.AddForeignKey(
                name: "FK_position_kpis_kpis_KpiId",
                table: "position_kpis",
                column: "KpiId",
                principalTable: "kpis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_position_kpis_kpis_KpiId",
                table: "position_kpis");

            migrationBuilder.AddForeignKey(
                name: "FK_position_kpis_kpis_KpiId",
                table: "position_kpis",
                column: "KpiId",
                principalTable: "kpis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
