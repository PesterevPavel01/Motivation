using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;

namespace Motivation.Application.Mappings;

public static class DeductionMapper
{
    public static DeductionDto? MapToDto(this Deduction? source)
    {
        if (source is null)
        {
            return null;
        }

        return new DeductionDto(
            Title: source.Title.Value,
            Code: source.Code.Value,
            Value: source.DeductionValue.Value,
            Month: source.Month.Value,
            Year: source.Year.Value,
            PositionCode: source.EmployeePosition?.Position?.Code.Value ?? string.Empty,
            EmployeeCode: source.EmployeePosition?.Employee?.Code.Value ?? string.Empty
        );
    }
}
