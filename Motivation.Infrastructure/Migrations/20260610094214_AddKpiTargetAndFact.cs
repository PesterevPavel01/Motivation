using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motivation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKpiTargetAndFact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Achievement",
                table: "position_kpis");

            migrationBuilder.DropColumn(
                name: "BonusAmount",
                table: "position_kpis");

            migrationBuilder.DropColumn(
                name: "Fact",
                table: "position_kpis");

            migrationBuilder.DropColumn(
                name: "Target",
                table: "position_kpis");

            migrationBuilder.CreateTable(
                name: "kpi_targets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Target = table.Column<decimal>(type: "numeric", nullable: false),
                    PositionKpiId = table.Column<Guid>(type: "uuid", nullable: false),
                    KpiFact = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kpi_targets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_kpi_targets_position_kpis_PositionKpiId",
                        column: x => x.PositionKpiId,
                        principalTable: "position_kpis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_kpi_targets_PositionKpiId",
                table: "kpi_targets",
                column: "PositionKpiId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "kpi_targets");

            migrationBuilder.AddColumn<decimal>(
                name: "Achievement",
                table: "position_kpis",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BonusAmount",
                table: "position_kpis",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Fact",
                table: "position_kpis",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Target",
                table: "position_kpis",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
