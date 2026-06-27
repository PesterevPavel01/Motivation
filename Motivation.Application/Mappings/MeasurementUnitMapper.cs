using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;

namespace Motivation.Application.Mappings
{
    public static class MeasurementUnitMapper
    {
        public static MeasurementUnitDto? MapToDto(this MeasurementUnit? source)
        {
            if (source is null)
            {
                return null;
            }

            return new MeasurementUnitDto(
                Title: source.Title.Value,
                Code: source.Code.Value
            );
        }
    }
}
