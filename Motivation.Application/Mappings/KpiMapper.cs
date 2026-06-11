using Calabonga.OperationResults;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities.Kpis;
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
            KpiType = source.KpiType.ToRussianString(),
            MeasurementUnit = source.MeasurementUnit.Title.Value,
            CustomerFilter = source.Filters.Where(x => x.KpiFilterType == KpiFilterType.Customer).Select(x => x.Title.Value),
            CompanyFilter = source.Filters.Where(x => x.KpiFilterType == KpiFilterType.Company).Select(x => x.Title.Value),
            ProductFilter = source.Filters.Where(x => x.KpiFilterType == KpiFilterType.Product).Select(x => x.Title.Value),
        };
    }
}

public static class KpiTypeExtensions
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
            _ => Operation.Error($"Unknown kpi type: {type}")
        };
    }
}
