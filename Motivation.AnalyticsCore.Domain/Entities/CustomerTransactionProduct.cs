using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Domain.Entities;

public class CustomerTransactionProduct
{
    protected CustomerTransactionProduct(Guid id, QuantityValue quantity, AmountValue amount)
    {
        Id = id;
        Quantity = quantity;
        Amount = amount;
    }
    public Guid Id { get; private set; }
    public QuantityValue Quantity { get; private set; }
    public AmountValue Amount { get; private set; }

    public CustomerTransaction CustomerTransaction { get; private set; } = null!;
    public Product Product {  get; private set; } = null!;

    public static Operation<CustomerTransactionProduct, string> Create(Product product, QuantityValue quantity, AmountValue amount)
    { 
        if (quantity is null)
            return Operation.Error("Quantity is null!");
        if (amount is null)
            return Operation.Error("Amount is null!");

        var customerTransactionProduct = new CustomerTransactionProduct(Guid.CreateVersion7(), quantity, amount);

        var setProduct = customerTransactionProduct.SetProduct(product);
        if (!setProduct.Ok)
            return Operation.Error(setProduct.Error);

        return customerTransactionProduct;
    }
    protected Operation<bool, string> SetProduct(Product product)
    {
        if (product is null)
            return Operation.Error("Product is null!");
        Product = product;

        return true;
    }
}
