namespace Motivation.Contracts.Dto;

public record PositionKpiDto(string KpiCode, string KpiTitle, int OrderNumber, decimal Weight, DateTime ValidFrom, DateTime ValidTo, KpiDto? Kpi);
