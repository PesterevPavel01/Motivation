using Calabonga.OperationResults;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.Enum;

namespace Motivation.Application.Mappings;

public static class KpiMapper
{
    public static Kpi MapToConcrete(this Kpi source)
    {
        return source.KpiType switch
        {
            KpiType.Sales => (SalesKpi)source,
            KpiType.Complexity => (ComplexityKpi)source,
            _ => throw new ArgumentOutOfRangeException(nameof(source.KpiType), $"Unknown KPI type: {source.KpiType}")
        };
    }

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
            KpiType = source.KpiType.ToRussianString(),
            MeasurementUnit = source.MeasurementUnit.MapToDto()!,
            CustomerFilter = source.Filters.Where(x => x.KpiFilterType == KpiFilterType.Customer).Select(x => new KpiFilterDto(x.Title.Value, x.Include)),
            CompanyFilter = source.Filters.Where(x => x.KpiFilterType == KpiFilterType.Company).Select(x => new KpiFilterDto(x.Title.Value, x.Include)),
            ProductFilter = source.Filters.Where(x => x.KpiFilterType == KpiFilterType.Product).Select(x => new KpiFilterDto(x.Title.Value, x.Include)),
            CustomerResponsibleFilter = source.Filters.Where(x => x.KpiFilterType == KpiFilterType.CustomerResponsible).Select(x => new KpiFilterDto(x.Title.Value, x.Include)),
            ProductResponsibleFilter = source.Filters.Where(x => x.KpiFilterType == KpiFilterType.ProductResponsible).Select(x => new KpiFilterDto(x.Title.Value, x.Include)),
            SalesResponsibleFilter = source.Filters.Where(x => x.KpiFilterType == KpiFilterType.SalesResponsible).Select(x => new KpiFilterDto(x.Title.Value, x.Include))
        };
    }
}

public static class KpiTypeMapper
{
    public static string ToRussianString(this KpiType type)
    {
        return type switch
        {
            KpiType.Sales => "Продажи",
            KpiType.Complexity => "Комплексность",
            KpiType.CustomerDebt => "Дебиторская задолженность",
            KpiType.AcceptedOrders => "Принятые заказы",
            KpiType.Upd => "УПД",
            KpiType.PercentageOfSales => "Процент от продаж",
            KpiType.BonusPerSalesUnit => "Бонус за продажу",
            KpiType.CompanyShareInTotalSales => "Доля организации в общих продажах предприятия",
            _ => type.ToString()
        };
    }

    public static Operation<KpiType, string> FromRussianString(string type)
    {
        return type switch
        {
            "Продажи" => KpiType.Sales,
            "Комплексность" => KpiType.Complexity,
            "Дебиторская задолженность" => KpiType.CustomerDebt,
            "Принятые заказы" => KpiType.AcceptedOrders,
            "УПД" => KpiType.Upd,
            "Процент от продаж" => KpiType.PercentageOfSales,
            "Бонус за продажу" => KpiType.BonusPerSalesUnit,
            "Доля организации в общих продажах предприятия" => KpiType.CompanyShareInTotalSales,
            _ => Operation.Error($"Unknown kpi type: {type}")
        };
    }
}
