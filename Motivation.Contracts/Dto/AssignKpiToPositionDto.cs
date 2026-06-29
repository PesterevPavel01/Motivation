namespace Motivation.Contracts.Dto;

public sealed record AssignKpiToPositionDto(
    string KpiCode, 
    int OrderNumber,
    decimal Weight, 
    DateTime ValidFrom,
    DateTime ValidTo);
