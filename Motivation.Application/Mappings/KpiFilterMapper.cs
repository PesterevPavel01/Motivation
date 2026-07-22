using Calabonga.OperationResults;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.ValueObjects;

namespace Motivation.Application.Mappings;

public static class KpiFilterMapper
{
    public static Operation<List<KpiFilter>, string> FromString(
        IEnumerable<KpiFilterDto> customerFilter,
        IEnumerable<KpiFilterDto> supplierFilter,
        IEnumerable<KpiFilterDto> productFilter,
        IEnumerable<KpiFilterDto> companyFilter,
        IEnumerable<KpiFilterDto> customerResponsibleFilter,
        IEnumerable<KpiFilterDto> productResponsibleFilter,
        IEnumerable<KpiFilterDto> salesResponsibleFilter,
        IEnumerable<KpiFilterDto> productStatusesFilter)
    {
        List<KpiFilter> kpiFilters = [];

        if (customerFilter.Any())
        {
            var filters = CreateFilters(customerFilter, KpiFilterCustomer.Create);
            if(!filters.Ok)
            {
                return Operation.Error(filters.Error);
            }
            kpiFilters.AddRange(filters.Result);
        }
        if (supplierFilter.Any())
        {
            var filters = CreateFilters(supplierFilter, KpiFilterSupplier.Create);
            if (!filters.Ok)
            {
                return Operation.Error(filters.Error);
            }
            kpiFilters.AddRange(filters.Result);
        }
        if (productFilter.Any())
        {
            var filters = CreateFilters(productFilter, KpiFilterProduct.Create);
            if (!filters.Ok)
            {
                return Operation.Error(filters.Error);
            }
            kpiFilters.AddRange(filters.Result);
        }
        if (companyFilter.Any())
        {
            var filters = CreateFilters(companyFilter, KpiFilterCompany.Create);
            if (!filters.Ok)
            {
                return Operation.Error(filters.Error);
            }
            kpiFilters.AddRange(filters.Result);
        }
        if (customerResponsibleFilter.Any())
        {
            var filters = CreateFilters(customerResponsibleFilter, KpiFilterCustomerResponsible.Create);
            if (!filters.Ok)
            {
                return Operation.Error(filters.Error);
            }
            kpiFilters.AddRange(filters.Result);
        }
        if (productResponsibleFilter.Any())
        {
            var filters = CreateFilters(productResponsibleFilter, KpiFilterProductResponsible.Create);
            if (!filters.Ok)
            {
                return Operation.Error(filters.Error);
            }
            kpiFilters.AddRange(filters.Result);
        }
        if (salesResponsibleFilter.Any())
        {
            var filters = CreateFilters(salesResponsibleFilter, KpiFilterSalesResponsible.Create);
            if (!filters.Ok)
            {
                return Operation.Error(filters.Error);
            }
            kpiFilters.AddRange(filters.Result);
        }
        if (productStatusesFilter.Any())
        {
            var filters = CreateFilters(productStatusesFilter, KpiFilterProductStatus.Create);
            if (!filters.Ok)
            {
                return Operation.Error(filters.Error);
            }
            kpiFilters.AddRange(filters.Result);
        }
        return kpiFilters;
    }

    private static Operation<List<KpiFilter>, string> CreateFilters<TEntity>(IEnumerable<KpiFilterDto> customerFilter, Func<TitleValue, bool, Operation<TEntity, string>> createFilter)
        where TEntity: KpiFilter
    {
        var filters = new List<KpiFilter>();
        foreach (var item in customerFilter)
        {
            var filterTitle = TitleValue.Create(item.Title);
            if (!filterTitle.Ok)
            {
                return Operation.Error(filterTitle.Error);
            }
            var filterItem = createFilter(filterTitle.Result, item.Include);
            if (!filterItem.Ok)
            {
                return Operation.Error(filterItem.Error);
            }
            filters.Add(filterItem.Result);
        }
        return filters;
    }
}
