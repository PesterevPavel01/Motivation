using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.KpiSalesCalculator.Interfaces;

namespace Motivation.KpiSalesCalculator.Services;

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
}
