using Microsoft.Extensions.Logging;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.BasketComplexityKpiCalculator.Interfaces;
using Motivation.BasketComplexityKpiCalculator.Models;

namespace Motivation.BasketComplexityKpiCalculator;

public class ComplexityCalculateProcessor : IComplexityCalculateProcessor
{
    private readonly ILogger<ComplexityCalculateProcessor> _logger;

    public ComplexityCalculateProcessor(
        ILogger<ComplexityCalculateProcessor> logger)
    {
        _logger = logger;
    }

    public decimal Process(List<ComplexityCustomerProductItem> items, IComplexityCalculationConfigurations complexityCalculationConfigurations)
    {
        var groupedByProductAndCustomer = GroupByProductAndCustomer(items);
        var groupedByMasterAndParam = GroupByMasterAndParam(groupedByProductAndCustomer);
        var totalsByMaster = CalculateTotalsByMaster(items, complexityCalculationConfigurations.GetCorrectionCoefficientByProduct);

        return CalculateSuccessRate(
            groupedByMasterAndParam,
            totalsByMaster,
            complexityCalculationConfigurations.MinUniqueGroups,
            complexityCalculationConfigurations.MinRatio,
            complexityCalculationConfigurations.ExcludedNumberParams);
    }

    private List<ProductCustomerGroup> GroupByProductAndCustomer(List<ComplexityCustomerProductItem> items)
    {
        return items
            .GroupBy(x => new { ProductCode = x.Product.Code, CustomerCode = x.Customer.Code })
            .Select(g => new ProductCustomerGroup
            {
                ProductCode = g.Key.ProductCode,
                CustomerCode = g.Key.CustomerCode,
                NumberParam = g.First().NumberParam,
                TotalQuantity = g.Sum(x => x.Quantity),
                TotalAmount = g.Sum(x => x.Amount),
                Customer = g.First().Customer,
                Product = g.First().Product
            })
            .ToList();
    }

    private List<MasterParamGroup> GroupByMasterAndParam(List<ProductCustomerGroup> groups)
    {
        return groups
            .Where(x => x.Customer.MasterCustomer is not null)
            .GroupBy(x => new { MasterCode = x.Customer.MasterCustomer!.Code, x.NumberParam })
            .Select(g => new MasterParamGroup
            {
                MasterCode = g.Key.MasterCode,
                NumberParam = g.Key.NumberParam,
                TotalQuantity = g.Sum(x => x.TotalQuantity),
                TotalAmount = g.Sum(x => x.TotalAmount)
            })
            .ToList();
    }

    private Dictionary<string, decimal> CalculateTotalsByMaster(List<ComplexityCustomerProductItem> items, Func<ProductDto, decimal> getCorrectionCoefficient)
    {
        return items
            .GroupBy(x => x.Customer.MasterCustomer!.Code)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => getCorrectionCoefficient(x.Product) * x.Amount)
            );
    }

    private decimal CalculateSuccessRate(
        List<MasterParamGroup> masterGroups,
        Dictionary<string, decimal> totalsByMaster,
        int MinUniqueGroups,
        decimal MinRatio,
        List<int> excludedNumberParams)
    {
        var results = masterGroups
            .GroupBy(x => x.MasterCode)
            .Select(g => new
            {
                MasterCode = g.Key,
                SumNumberParams = g
                    .Where(x => !excludedNumberParams.Contains(x.NumberParam))
                    .Sum(x => x.NumberParam),
                FilteredAmount = g.Sum(x => x.TotalAmount)
            })
            .Where(x => totalsByMaster.TryGetValue(x.MasterCode, out var total) && total > 0)
            .Select(x =>
            {
                var totalAmount = totalsByMaster[x.MasterCode];
                var ratio = x.FilteredAmount / totalAmount;

                return new ComplexityResult
                {
                    MasterCode = x.MasterCode,
                    SumNumberParams = x.SumNumberParams,
                    FilteredAmount = x.FilteredAmount,
                    TotalAmount = totalAmount,
                    Ratio = ratio,
                    IsSuccess = x.SumNumberParams == MinUniqueGroups
                                && ratio >= MinRatio
                };
            })
            .ToList();

        return results.Count(x => x.IsSuccess);
    }
}