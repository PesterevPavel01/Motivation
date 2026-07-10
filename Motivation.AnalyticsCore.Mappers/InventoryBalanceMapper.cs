using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Mappers;

public sealed class InventoryBalanceMapper
{
    public static Operation<InventoryBalance, string> ToEntity(InventoryBalanceDto dto, Product product, Warehouse warehouse)
    {
        if (dto is null)
            return Operation.Error("InventoryBalance DTO is null.");
        if (product is null)
            return Operation.Error("Product is null.");
        if (warehouse is null)
            return Operation.Error("Warehouse is null.");

        var amount = AmountValue.Create(dto.TotalAmount);
        if (!amount.Ok)
            return Operation.Error(amount.Error);

        var quantity = QuantityValue.Create(dto.TotalQuantity);
        if (!quantity.Ok)
            return Operation.Error(quantity.Error);

        return InventoryBalance.Create(
            warehouse,
            product,
            quantity.Result,
            amount.Result,
            dto.AsOfDate);
    }
}
