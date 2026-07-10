using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Mappers;

public static class WarehouseTransactionMapper
{
    public static Operation<WarehouseTransaction, string> ToEntity(WarehouseTransactionDto model, Product product, Warehouse warehouse)
    {
        if (model is null)
            return Operation.Error("WarehouseTransaction DTO is null.");

        var typeResult = WarehouseTransactionTypeMapper.FromRussian(model.Type);

        var quantityResult = QuantityValue.Create(model.Quantity);
        if (!quantityResult.Ok)
            return Operation.Error(quantityResult.Error);

        var amountResult = AmountValue.Create(model.Amount);
        if (!amountResult.Ok)
            return Operation.Error(amountResult.Error);

        var code = CodeValue.Create(model.Code);
        if (!code.Ok)
            return Operation.Error(code.Error);

        var transactionResult = WarehouseTransaction.Create(
            code.Result,
            typeResult,
            quantityResult.Result,
            amountResult.Result,
            model.AsOfDate,
            product,
            warehouse
        );

        if (!transactionResult.Ok)
            return Operation.Error(transactionResult.Error);

        return transactionResult.Result!;
    }

    public static Operation<bool, string> EditEntity(
        this WarehouseTransaction transaction,
        WarehouseTransactionDto model,
        Warehouse warehouse,
        Product product)
    {
        if (model is null)
            return Operation.Error("WarehouseTransaction DTO is null.");

        var typeResult = WarehouseTransactionTypeMapper.FromRussian(model.Type);

        var quantityResult = QuantityValue.Create(model.Quantity);
        if (!quantityResult.Ok)
            return Operation.Error(quantityResult.Error);

        var amountResult = AmountValue.Create(model.Amount);
        if (!amountResult.Ok)
            return Operation.Error(amountResult.Error);

        var transactionResult = transaction.Edit(
            typeResult,
            quantityResult.Result,
            amountResult.Result,
            model.AsOfDate,
            product,
            warehouse);

        if (!transactionResult.Ok)
            return Operation.Error(transactionResult.Error);

        return true;
    }
}
