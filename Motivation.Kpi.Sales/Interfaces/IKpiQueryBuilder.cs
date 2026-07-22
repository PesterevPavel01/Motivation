using Motivation.Contracts.Dto;

namespace Motivation.KpiSalesCalculator.Interfaces;

public interface IKpiQueryBuilder
{
    IKpiQueryBuilder SetPeriod(DateTime from, DateTime to);
    IKpiQueryBuilder ApplyCustomerFilter(IEnumerable<KpiFilterDto> filters);
    IKpiQueryBuilder ApplyProductSupplierFilter(IEnumerable<KpiFilterDto> filters);
    IKpiQueryBuilder ApplyProductFilter(IEnumerable<KpiFilterDto> filters);
    IKpiQueryBuilder ApplyProductStatusFilter(IEnumerable<KpiFilterDto> filters);
    IKpiQueryBuilder ApplyCompanyFilter(IEnumerable<KpiFilterDto> filters);
    IKpiQueryBuilder ApplyResponsibleFilters(
        IEnumerable<KpiFilterDto> customerResponsibleFilter,
        IEnumerable<KpiFilterDto> salesResponsibleFilter,
        IEnumerable<KpiFilterDto> productResponsibleFilter);
    Task<KpiQueryResult> BuildAsync(CancellationToken cancellationToken = default);
}
