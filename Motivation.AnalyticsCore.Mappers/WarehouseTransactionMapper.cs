using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Mappers;

public static class WarehouseTransactionMapper
{
    public static Operation<WarehouseTransaction, string> ToEntity(WarehouseTransactionDto dto)
    {
        if (dto is null)
            return Operation.Error("WarehouseTransaction DTO is null.");

        var productResult = ProductMapper.ToEntity(dto.Product, [], []);
        if (!productResult.Ok)
            return Operation.Error(productResult.Error!);

        var typeResult = WarehouseTransactionTypeMapper.FromRussian(dto.Type);

        var quantityResult = QuantityValue.Create(dto.Quantity);
        if (!quantityResult.Ok)
            return Operation.Error(quantityResult.Error);

        var amountResult = AmountValue.Create(dto.Amount);
        if (!amountResult.Ok)
            return Operation.Error(amountResult.Error);

        var code = CodeValue.Create(Guid.CreateVersion7().ToString());
        if (!code.Ok)
            return Operation.Error(code.Error);

        var transactionResult = WarehouseTransaction.Create(
            code.Result,
            typeResult,
            quantityResult.Result,
            amountResult.Result,
            dto.AsOfDate,
            productResult.Result
        );

        if (!transactionResult.Ok)
            return Operation.Error(transactionResult.Error);

        return transactionResult.Result!;
    }
}
