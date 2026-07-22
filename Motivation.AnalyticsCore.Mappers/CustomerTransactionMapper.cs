using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Contracts.Models;
using Motivation.AnalyticsCore.Domain.Entities;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Application.Mappers;

public static class CustomerTransactionMapper
{
    public static Operation<CustomerTransaction, string> ToEntity(CustomerTransactionDto model, Company company, Customer customer, Dictionary<string, Product> products)
    {
        if (model is null)
            return Operation.Error("CustomerTransaction DTO is null.");

        var responsibleResult = ResponsibleValue.Create(model.Responsible);
        if (!responsibleResult.Ok)
            return Operation.Error(responsibleResult.Error);

        var typeResult = CustomerTransactionTypeMapper.FromRussian(model.Type);

        var amountResult = AmountValue.Create(model.Amount);
        if (!amountResult.Ok)
            return Operation.Error(amountResult.Error);

        var code = CodeValue.Create(model.Code);
        if (!code.Ok)
            return Operation.Error(code.Error);

        List<CustomerTransactionProduct> transactionProducts = [];
        foreach (var productModel in model.TransactionProducts)
        {
            if (!products.TryGetValue(productModel.Product.Code, out var product))
                return Operation.Error($"Product not found!");
            if (product is null)
                return Operation.Error($"Product is null!");

            var quantityResult = QuantityValue.Create(productModel.Quantity);
            if (!quantityResult.Ok)
                return Operation.Error($"Quantity error: {quantityResult.Error}");

            var productAmountResult = AmountValue.Create(productModel.Amount);
            if (!productAmountResult.Ok)
                return Operation.Error($"Amount error: {productAmountResult.Error}");

            var transactionProductResult = CustomerTransactionProduct.Create(
                product,
                quantityResult.Result,
                productAmountResult.Result
            );

            if (!transactionProductResult.Ok)
                return Operation.Error($"TransactionProduct error: {transactionProductResult.Error}");
            transactionProducts.Add(transactionProductResult.Result);
        }

        var transactionResult = CustomerTransaction.Create(
            code.Result,
            responsibleResult.Result,
            typeResult,
            amountResult.Result,
            model.AsOfDate,
            customer,
            company,
            transactionProducts
        );

        if (!transactionResult.Ok)
            return Operation.Error(transactionResult.Error);

        return transactionResult.Result;
    }

    public static Operation<bool, string> EditEntity(
        this CustomerTransaction customerTransaction, 
        CustomerTransactionDto model,
        Company company, 
        Customer customer, 
        Dictionary<string, Product> products)
    {
        if (model is null)
            return Operation.Error("CustomerTransaction DTO is null.");

        var responsibleResult = ResponsibleValue.Create(model.Responsible);
        if (!responsibleResult.Ok)
            return Operation.Error(responsibleResult.Error);

        var typeResult = CustomerTransactionTypeMapper.FromRussian(model.Type);

        var amountResult = AmountValue.Create(model.Amount);
        if (!amountResult.Ok)
            return Operation.Error(amountResult.Error);

        List<CustomerTransactionProduct> transactionProducts = [];

        foreach (var productModel in model.TransactionProducts)
        {
            if (!products.TryGetValue(productModel.Product.Code, out var product))
                return Operation.Error($"Product not found!");
            if (product is null)
                return Operation.Error($"Product is null!");

            var quantityResult = QuantityValue.Create(productModel.Quantity);
            if (!quantityResult.Ok)
                return Operation.Error($"Quantity error: {quantityResult.Error}");

            var productAmountResult = AmountValue.Create(productModel.Amount);
            if (!productAmountResult.Ok)
                return Operation.Error($"Amount error: {productAmountResult.Error}");

            var transactionProductResult = CustomerTransactionProduct.Create(
                product,
                quantityResult.Result,
                productAmountResult.Result
            );

            if (!transactionProductResult.Ok)
                return Operation.Error($"TransactionProduct error: {transactionProductResult.Error}");

            transactionProducts.Add(transactionProductResult.Result);
        }

        var transactionResult = customerTransaction.Edit(
            responsibleResult.Result,
            typeResult,
            amountResult.Result,
            model.AsOfDate,
            customer,
            company,
            transactionProducts
        );

        if (!transactionResult.Ok)
            return Operation.Error(transactionResult.Error);

        return true;
    }

    public static CustomerTransactionDto ToDto(CustomerTransaction entity)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var customerDto = new CustomerDto
        {
            Code = entity.Customer?.Code?.Value ?? string.Empty,
            Title = entity.Customer?.Title?.Value ?? string.Empty,
            FullName = entity.Customer?.FullName?.Value ?? string.Empty,
            Manager = entity.Customer?.Manager?.Value?.ToString() ?? string.Empty,
        };

        var companyDto = new CompanyDto
        {
            Code = entity.Company?.Code?.Value ?? string.Empty,
            Title = entity.Company?.Title?.Value ?? string.Empty
        };

        var transactionDto = new CustomerTransactionDto
        {
            Code = entity.Code?.Value ?? string.Empty,
            Amount = entity.TotalAmount?.Value ?? 0,
            Customer = customerDto,
            AsOfDate = entity.AsOfDate,
            Responsible = entity.Responsible?.Value?.ToString() ?? string.Empty,
            Type = entity.Type.ToString(),
            Company = companyDto,
            TransactionProducts = []
        };

        foreach (var ctp in entity.CustomerTransactionProducts)
        {
            var productDto = new ProductDto
            {
                Code = ctp.Product?.Code?.Value ?? string.Empty,
                Title = ctp.Product?.Title?.Value ?? string.Empty,
                Purchaser = ctp.Product?.Purchaser?.Value?.ToString() ?? string.Empty,
                Supplier = ctp.Product?.Supplier?.Value?.ToString() ?? string.Empty,
                Status = ctp.Product?.Status?.Value ?? string.Empty
            };

            transactionDto.TransactionProducts.Add(new TransactionProductDto
            {
                Quantity = ctp.Quantity?.Value ?? 0,
                Amount = ctp.Amount?.Value ?? 0,
                Product = productDto
            });
        }

        return transactionDto;
    }

    public static CustomerTransactionDto ToDto(
       CustomerTransaction entity,
       Func<CustomerTransactionProduct, bool> productFilter)
    {
        if (entity is null)
            throw new ArgumentNullException(nameof(entity));

        var customerDto = new CustomerDto
        {
            Code = entity.Customer?.Code?.Value ?? string.Empty,
            Title = entity.Customer?.Title?.Value ?? string.Empty,
            FullName = entity.Customer?.FullName?.Value ?? string.Empty,
            Manager = entity.Customer?.Manager?.Value?.ToString() ?? string.Empty,
        };

        var companyDto = new CompanyDto
        {
            Code = entity.Company?.Code?.Value ?? string.Empty,
            Title = entity.Company?.Title?.Value ?? string.Empty
        };

        var transactionDto = new CustomerTransactionDto
        {
            Code = entity.Code?.Value ?? string.Empty,
            Amount = entity.TotalAmount?.Value ?? 0,
            Customer = customerDto,
            AsOfDate = entity.AsOfDate,
            Responsible = entity.Responsible?.Value?.ToString() ?? string.Empty,
            Type = entity.Type.ToString(),
            Company = companyDto,
            TransactionProducts = []
        };

        foreach (var ctp in entity.CustomerTransactionProducts.Where(productFilter))
        {
            var productDto = new ProductDto
            {
                Code = ctp.Product?.Code?.Value ?? string.Empty,
                Title = ctp.Product?.Title?.Value ?? string.Empty,
                Purchaser = ctp.Product?.Purchaser?.Value?.ToString() ?? string.Empty,
                Supplier = ctp.Product?.Supplier?.Value?.ToString() ?? string.Empty,
                Status = ctp.Product?.Status?.Value ?? string.Empty
            };

            transactionDto.TransactionProducts.Add(new TransactionProductDto
            {
                Quantity = ctp.Quantity?.Value ?? 0,
                Amount = ctp.Amount?.Value ?? 0,
                Product = productDto
            });
        }

        return transactionDto;
    }
}
