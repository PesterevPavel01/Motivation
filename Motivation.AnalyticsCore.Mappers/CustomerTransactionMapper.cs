using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Mappers;

public static class CustomerTransactionMapper
{
    public static Operation<CustomerTransaction, string> ToEntity(CustomerTransactionDto dto, Company company, Customer customer, List<Product> products)
    {
        if (dto is null)
            return Operation.Error("CustomerTransaction DTO is null.");

        var responsibleResult = ResponsibleValue.Create(dto.Responsible);
        if (!responsibleResult.Ok)
            return Operation.Error(responsibleResult.Error);

        var typeResult = CustomerTransactionTypeMapper.FromRussian(dto.Type);

        var amountResult = AmountValue.Create(dto.Amount);
        if (!amountResult.Ok)
            return Operation.Error(amountResult.Error);

        var code = CodeValue.Create(dto.Code);
        if (!code.Ok)
            return Operation.Error(code.Error);

        var transactionResult = CustomerTransaction.Create(
            code.Result,
            responsibleResult.Result,
            typeResult,
            amountResult.Result,
            dto.AsOfDate,
            customer,
            company
        );

        if (!transactionResult.Ok)
            return Operation.Error(transactionResult.Error);

        var transaction = transactionResult.Result;

        foreach (var productDto in dto.TransactionProducts)
        {
            var product = products.FirstOrDefault(x => x.Code.Value == productDto.Product.Code);
            if(product is null)
                return Operation.Error($"Product not found!");

            var quantityResult = QuantityValue.Create(productDto.Quantity);
            if (!quantityResult.Ok)
                return Operation.Error($"Quantity error: {quantityResult.Error}");

            var productAmountResult = AmountValue.Create(productDto.Amount);
            if (!productAmountResult.Ok)
                return Operation.Error($"Amount error: {productAmountResult.Error}");

            var transactionProductResult = CustomerTransactionProduct.Create(
                product,
                quantityResult.Result,
                productAmountResult.Result
            );

            if (!transactionProductResult.Ok)
                return Operation.Error($"TransactionProduct error: {transactionProductResult.Error}");
        }

        return transaction;
    }
}
