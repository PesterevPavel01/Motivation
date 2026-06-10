using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.Enum;

namespace Motivation.Application.Mappings;

public static class KpiMapper
{
    public static KpiDto MapToDto(this Kpi source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return new KpiDto()
        {
            Title = source.Title.Value,
            Code = source.Code.Value,
            Abbreviation = source.Abbreviation.Value,
            CalculationType = source.CalculationType.ToRussianString(),
            MeasurementUnit = source.MeasurementUnit.Title.Value,
            CustomerFilter = source.Filters.Where(x => x.KpiFilterType == KpiFilterType.Customer).Select(x => x.Title.Value),
            CompanyFilter = source.Filters.Where(x => x.KpiFilterType == KpiFilterType.Company).Select(x => x.Title.Value),
            ProductFilter = source.Filters.Where(x => x.KpiFilterType == KpiFilterType.Product).Select(x => x.Title.Value),
        };
    }
}
