using Calabonga.OperationResults;
using Microsoft.Extensions.Logging;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.BasketComplexityKpiCalculator.Interfaces;
using Motivation.BasketComplexityKpiCalculator.Models;

namespace Motivation.BasketComplexityKpiCalculator.Mappers;

public class ComplexityItemsMapper
{
    private readonly ILogger<ComplexityItemsMapper> _logger;

    public ComplexityItemsMapper(
        ILogger<ComplexityItemsMapper> logger)
    {
        _logger = logger;
    }

    public Operation<List<ComplexityCustomerProductItem>, string> FromTransactionModels(
        IEnumerable<CustomerTransactionDto> transactions, IComplexityCalculationConfigurations configurations)
    {
        var complexityParamByProductCache = BuildСomplexityParamByProductCache(
            transactions, 
            configurations.FindNumberParamForProduct);

        var items = transactions
            .Where(t => t.Customer is not null && t.Customer.MasterCustomer is not null)
            .SelectMany(t => t.TransactionProducts.Select(p => new
            {
                Transaction = t,
                Product = p
            }))
            .Select(x => CreateComplexityItem(x.Transaction, x.Product, complexityParamByProductCache, configurations.IncludeZeroAmountProducts))
            .Where(x => x.HasSuccess && x.NumberParam != 0)
            .ToList();

        if (items.Count == 0)
        {
            _logger.LogWarning("No complexity items created!");
            return Operation.Error("No complexity items created!");
        }

        return items;
    }

    private ComplexityCustomerProductItem CreateComplexityItem(
        CustomerTransactionDto transaction,
        TransactionProductDto product,
        Dictionary<string, int> complexityParamByProductCache,
        bool includeZeroAmountProducts)
    {
        var productCode = product.Product?.Code ?? string.Empty;
        var numberParam = complexityParamByProductCache.GetValueOrDefault(productCode, 0);

        var amount = product.Amount;
        var quantity = product.Quantity;

        if (includeZeroAmountProducts && amount == 0)
        {
            amount = 1;
            quantity = 1;
        }

        return new ComplexityCustomerProductItem
        {
            NumberParam = numberParam,
            Quantity = quantity,
            Amount = amount,
            Customer = transaction.Customer!,
            Product = product.Product!,
            TransactionCode = transaction.Code ?? string.Empty
        };
    }

    private Dictionary<string, int> BuildСomplexityParamByProductCache(
        IEnumerable<CustomerTransactionDto> transactions,
        Func<ProductDto, int?> findNumberParamForProduct)
    {
        var cache = new Dictionary<string, int>();

        var allProducts = transactions
            .SelectMany(t => t.TransactionProducts)
            .Select(p => p.Product)
            .DistinctBy(p => p?.Code)
            .ToList();

        foreach (var product in allProducts)
        {
            if (product?.Code is null) continue;

            var numberParam = findNumberParamForProduct(product);
            if (numberParam.HasValue)
            {
                cache[product.Code] = numberParam.Value;
            }
        }
        return cache;
    }
}