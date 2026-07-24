using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.KpiCalculator.Core.Interfaces;

namespace Motivation.KpiCalculator.Core.Services;

public class HierarchyService : IHierarchyService
{
    public IEnumerable<string> GetCustomerGroupHierarchy(Customer customer)
    {
        var codes = new List<string>();
        codes.Add(customer.Code.Value);
        var currentGroup = customer.CustomerGroup;

        while (currentGroup is not null)
        {
            codes.Add(currentGroup.Code.Value);
            currentGroup = currentGroup.ParentGroup;
        }

        return codes;
    }

    public IEnumerable<string> GetProductGroupHierarchy(Product product)
    {
        var codes = new List<string>();
        codes.Add(product.Code.Value);
        var currentGroup = product.ProductGroup;

        while (currentGroup is not null)
        {
            codes.Add(currentGroup.Code.Value);
            currentGroup = currentGroup.ParentGroup;
        }

        return codes;
    }

    public IEnumerable<string> GetProductGroupHierarchy(ProductDto product)
    {
        if (product is null)
            return Enumerable.Empty<string>();

        var codes = new List<string>();

        codes.Add(product.Code);

        var currentGroup = product.ProductGroup;
        while (currentGroup is not null)
        {
            codes.Add(currentGroup.Code);
            currentGroup = currentGroup.ParentGroup;
        }

        return codes;
    }
}
