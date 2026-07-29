using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;

namespace Motivation.KpiCalculator.Core.Interfaces;

public interface IHierarchyService
{
    IEnumerable<string> GetCustomerGroupHierarchy(Customer customer);
    IEnumerable<string> GetCustomerGroupHierarchy(CustomerDto customer);
    IEnumerable<string> GetProductGroupHierarchy(Product product);
    IEnumerable<string> GetProductGroupHierarchy(ProductDto product);
}