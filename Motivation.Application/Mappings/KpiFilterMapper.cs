using Calabonga.OperationResults;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.Application.Mappings;

public static class KpiFilterMapper
{
    public static Operation<List<KpiFilter>, string> FromString(
        IEnumerable<string> customerFilter,
        IEnumerable<string> productFilter,
        IEnumerable<string> companyFilter)
    {
        List<KpiFilter> kpiFilters = [];

        if (customerFilter.Any())
        {
            foreach (var item in customerFilter)
            {
                var filterTitle = TitleValue.Create(item);
                if (!filterTitle.Ok)
                {
                    return Operation.Error(filterTitle.Error);
                }
                var filterItem = KpiFilterCustomer.Create(filterTitle.Result);
                if (!filterItem.Ok)
                {
                    return Operation.Error(filterItem.Error);
                }
                kpiFilters.Add(filterItem.Result);
            }
        }
        if (productFilter.Any())
        {
            foreach (var item in productFilter)
            {
                var filterTitle = TitleValue.Create(item);
                if (!filterTitle.Ok)
                {
                    return Operation.Error(filterTitle.Error);
                }
                var filterItem = KpiFilterProduct.Create(filterTitle.Result);
                if (!filterItem.Ok)
                {
                    return Operation.Error(filterItem.Error);
                }
                kpiFilters.Add(filterItem.Result);
            }
        }
        if (companyFilter.Any())
        {
            foreach (var item in companyFilter)
            {
                var filterTitle = TitleValue.Create(item);
                if (!filterTitle.Ok)
                {
                    return Operation.Error(filterTitle.Error);
                }
                var filterItem = KpiFilterCompany.Create(filterTitle.Result);
                if (!filterItem.Ok)
                {
                    return Operation.Error(filterItem.Error);
                }
                kpiFilters.Add(filterItem.Result);
            }
        }
        return kpiFilters;
    }
}
