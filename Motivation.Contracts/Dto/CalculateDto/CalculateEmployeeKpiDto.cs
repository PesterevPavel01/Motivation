namespace Motivation.Contracts.Dto.CalculateDto;

public sealed record CalculateEmployeeKpiDto(EmployeeDto Employee, string PositionCode, string KpiCode, DateTime PeriodFrom, DateTime PeriodTo);