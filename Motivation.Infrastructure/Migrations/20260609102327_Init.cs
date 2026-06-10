using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Motivation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SecondName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FullName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "measurement_unit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_measurement_unit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "positions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "numeric", nullable: false),
                    WorkWeekType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MotivationPart = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "standard_hours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkWeekType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    StandardHoursValue = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_standard_hours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "kpis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CalculationType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MeasurementUnitId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kpis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_kpis_measurement_unit_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "measurement_unit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "employee_positions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employee_positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employee_positions_employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_employee_positions_positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "kpi_filters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    KpiFilterType = table.Column<int>(type: "integer", nullable: false),
                    KpiId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kpi_filters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_kpi_filters_kpis_KpiId",
                        column: x => x.KpiId,
                        principalTable: "kpis",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "position_kpis",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Target = table.Column<decimal>(type: "numeric", nullable: false),
                    Fact = table.Column<decimal>(type: "numeric", nullable: false),
                    Achievement = table.Column<decimal>(type: "numeric", nullable: false),
                    BonusAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    OrderNumber = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric", nullable: false),
                    PositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    KpiId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_position_kpis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_position_kpis_kpis_KpiId",
                        column: x => x.KpiId,
                        principalTable: "kpis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_position_kpis_positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    DeductionValue = table.Column<decimal>(type: "numeric", nullable: false),
                    EmployeePositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_deductions_employee_positions_EmployeePositionId",
                        column: x => x.EmployeePositionId,
                        principalTable: "employee_positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "extra_parts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExtraPartValue = table.Column<decimal>(type: "numeric", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmployeePositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_extra_parts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_extra_parts_employee_positions_EmployeePositionId",
                        column: x => x.EmployeePositionId,
                        principalTable: "employee_positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_deductions_Code",
                table: "deductions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_deductions_EmployeePositionId",
                table: "deductions",
                column: "EmployeePositionId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_positions_EmployeeId",
                table: "employee_positions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_employee_positions_PositionId_EmployeeId",
                table: "employee_positions",
                columns: new[] { "PositionId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_Code",
                table: "employees",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_FirstName_LastName",
                table: "employees",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_employees_FullName",
                table: "employees",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_extra_parts_Code",
                table: "extra_parts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_extra_parts_EmployeePositionId",
                table: "extra_parts",
                column: "EmployeePositionId");

            migrationBuilder.CreateIndex(
                name: "IX_kpi_filters_KpiId",
                table: "kpi_filters",
                column: "KpiId");

            migrationBuilder.CreateIndex(
                name: "IX_kpi_filters_Title",
                table: "kpi_filters",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_kpis_Code",
                table: "kpis",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_kpis_MeasurementUnitId",
                table: "kpis",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_kpis_Title",
                table: "kpis",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_measurement_unit_Code",
                table: "measurement_unit",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_measurement_unit_Title",
                table: "measurement_unit",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_position_kpis_KpiId",
                table: "position_kpis",
                column: "KpiId");

            migrationBuilder.CreateIndex(
                name: "IX_position_kpis_PositionId",
                table: "position_kpis",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_positions_Code",
                table: "positions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_positions_Title",
                table: "positions",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deductions");

            migrationBuilder.DropTable(
                name: "extra_parts");

            migrationBuilder.DropTable(
                name: "kpi_filters");

            migrationBuilder.DropTable(
                name: "position_kpis");

            migrationBuilder.DropTable(
                name: "standard_hours");

            migrationBuilder.DropTable(
                name: "employee_positions");

            migrationBuilder.DropTable(
                name: "kpis");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "positions");

            migrationBuilder.DropTable(
                name: "measurement_unit");
        }
    }
}
