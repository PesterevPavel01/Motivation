using Motivation.Domain.ValueObjects;

namespace Motivation.Contracts.Dto;

public sealed record AssignKpiToPositionDto(
    string PositionCode, 
    string KpiCode, 
    int OrderNumber,
    decimal Weight, 
    DateTime ValidFrom,
    DateTime ValidTo);
