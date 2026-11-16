using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motivation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteForKpiFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_kpi_filters_kpis_KpiId",
                table: "kpi_filters");

            migrationBuilder.AlterColumn<Guid>(
                name: "KpiId",
                table: "kpi_filters",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_kpi_filters_kpis_KpiId",
                table: "kpi_filters",
                column: "KpiId",
                principalTable: "kpis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_kpi_filters_kpis_KpiId",
                table: "kpi_filters");

            migrationBuilder.AlterColumn<Guid>(
                name: "KpiId",
                table: "kpi_filters",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_kpi_filters_kpis_KpiId",
                table: "kpi_filters",
                column: "KpiId",
                principalTable: "kpis",
                principalColumn: "Id");
        }
    }
}
