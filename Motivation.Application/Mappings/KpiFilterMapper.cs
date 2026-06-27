using Calabonga.OperationResults;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.Application.Mappings;

public static class KpiFilterMapper
{
    public static Operation<List<KpiFilter>, string> FromString(
        IEnumerable<KpiFilterDto> customerFilter,
        IEnumerable<KpiFilterDto> productFilter,
        IEnumerable<KpiFilterDto> companyFilter)
    {
        List<KpiFilter> kpiFilters = [];

        if (customerFilter.Any())
        {
            foreach (var item in customerFilter)
            {
                var filterTitle = TitleValue.Create(item.Title);
                if (!filterTitle.Ok)
                {
                    return Operation.Error(filterTitle.Error);
                }
                var filterItem = KpiFilterCustomer.Create(filterTitle.Result, item.Include);
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
                var filterTitle = TitleValue.Create(item.Title);
                if (!filterTitle.Ok)
                {
                    return Operation.Error(filterTitle.Error);
                }
                var filterItem = KpiFilterProduct.Create(filterTitle.Result, item.Include);
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
                var filterTitle = TitleValue.Create(item.Title);
                if (!filterTitle.Ok)
                {
                    return Operation.Error(filterTitle.Error);
                }
                var filterItem = KpiFilterCompany.Create(filterTitle.Result, item.Include);
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
