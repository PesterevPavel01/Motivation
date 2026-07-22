using Motivation.AnalyticsCore.Domain.Entities;

namespace Motivation.KpiSalesCalculator.Interfaces;

public interface IHierarchyService
{
    IEnumerable<string> GetCustomerGroupHierarchy(Customer customer);
    IEnumerable<string> GetProductGroupHierarchy(Product product);
}