using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.Contracts.Dto;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiCalculator.Core.Specifications;

public static class FilterSpecifications
{
    public static ISpecification<CustomerTransaction> BuildCustomerFilterSpecification(
        IEnumerable<KpiFilterDto> filters,
        Func<Customer, IEnumerable<string>> getGroupHierarchy)
    {
        if (filters is null || !filters.Any())
            return new TrueSpecification<CustomerTransaction>();

        var includeCodes = filters.Where(f => f.Include).Select(f => f.Title).ToList();
        var excludeCodes = filters.Where(f => !f.Include).Select(f => f.Title).ToList();

        return new CustomerFilterSpecification(includeCodes, excludeCodes, getGroupHierarchy);
    }

    public static ISpecification<CustomerTransactionProduct> BuildProductFilterSpecification(
        IEnumerable<KpiFilterDto> filters,
        Func<Product, IEnumerable<string>> getGroupHierarchy)
    {
        if (filters is null || !filters.Any())
            return new TrueSpecification<CustomerTransactionProduct>();

        var includeCodes = filters.Where(f => f.Include).Select(f => f.Title).ToList();
        var excludeCodes = filters.Where(f => !f.Include).Select(f => f.Title).ToList();

        return new ProductFilterSpecification(includeCodes, excludeCodes, getGroupHierarchy);
    }

    public static ISpecification<CustomerTransactionProduct> BuildStatusProductFilterSpecification(
    IEnumerable<KpiFilterDto> filters)
    {
        if (filters is null || !filters.Any())
            return new TrueSpecification<CustomerTransactionProduct>();

        var includeCodes = filters.Where(f => f.Include).Select(f => f.Title).ToList();
        var excludeCodes = filters.Where(f => !f.Include).Select(f => f.Title).ToList();

        return new ProductStatusFilterSpecification(includeCodes, excludeCodes);
    }

    public static ISpecification<CustomerTransactionProduct> BuildProductSupplierFilterSpecification(
    IEnumerable<KpiFilterDto> filters)
    {
        if (filters is null || !filters.Any())
            return new TrueSpecification<CustomerTransactionProduct>();

        var includeCodes = filters.Where(f => f.Include).Select(f => f.Title).ToList();
        var excludeCodes = filters.Where(f => !f.Include).Select(f => f.Title).ToList();

        return new ProductSupplierFilterSpecification(includeCodes, excludeCodes);
    }

    public static ISpecification<CustomerTransaction> BuildCompanyFilterSpecification(
        IEnumerable<KpiFilterDto> filters)
    {
        if (filters is null || !filters.Any())
            return new TrueSpecification<CustomerTransaction>();

        var includeCodes = filters.Where(f => f.Include).Select(f => f.Title).ToList();
        var excludeCodes = filters.Where(f => !f.Include).Select(f => f.Title).ToList();

        return new CompanyFilterSpecification(includeCodes, excludeCodes);
    }
}
