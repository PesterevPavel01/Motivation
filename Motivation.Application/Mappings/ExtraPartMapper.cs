using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;

namespace Motivation.Application.Mappings;

public static class ExtraPartMapper
{
    public static ExtraPartDto? MapToDto(this ExtraPart? source)
    {
        if (source is null)
        {
            return null;
        }

        return new ExtraPartDto(
            Title: source.Title.Value,
            Code: source.Code.Value,
            PositionCode: source.EmployeePosition?.Position?.Code.Value ?? string.Empty,
            EmployeeCode: source.EmployeePosition?.Employee?.Code.Value ?? string.Empty,
            Value: source.ExtraPartValue.Value,
            ValidFrom: source.ValidFrom,
            ValidTo: source.ValidTo
        );
    }
}
