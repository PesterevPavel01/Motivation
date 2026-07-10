using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;
using Motivation.AnalyticsCore.Domain.Enum;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Domain.Entities;

public class CustomerTransaction : Entity
{
    private readonly List<CustomerTransactionProduct> _customerTransactionProducts = [];
    protected CustomerTransaction(Guid id, CodeValue code, ResponsibleValue responsible, CustomerTransactionType type, AmountValue? totalAmount, DateTime asOfDate) : base(id, code)
    {
        Type = type;
        TotalAmount = totalAmount;
        AsOfDate = asOfDate;
        Responsible = responsible;
    }

    public AmountValue? TotalAmount { get; protected set; }
    public ResponsibleValue Responsible { get; set; }
    public CustomerTransactionType Type { get; protected set; }
    public DateTime AsOfDate { get; private set; }

    public Company Company { get; protected set; } = null!;
    public Customer Customer { get; protected set; } = null!;

    public IReadOnlyCollection<CustomerTransactionProduct> CustomerTransactionProducts => _customerTransactionProducts.AsReadOnly();

    public static Operation<CustomerTransaction, string> Create(
        CodeValue code, 
        ResponsibleValue responsible, 
        CustomerTransactionType type, 
        AmountValue? totalAmount, 
        DateTime asOfDate, 
        Customer customer, 
        Company company,
        IEnumerable<CustomerTransactionProduct> transactionProducts)
    {
        if (code is null)
            return Operation.Error("Code is null!");
        if (responsible is null)
            return Operation.Error("Responsible is null!");

        var customerTransaction = new CustomerTransaction(Guid.CreateVersion7(), code, responsible, type, totalAmount, asOfDate);

        var setProduct = customerTransaction.SetCustomer(customer);
        if (!setProduct.Ok)
            return Operation.Error(setProduct.Error);

        var setCompany = customerTransaction.SetCompany(company);
        if (!setCompany.Ok)
            return Operation.Error(setCompany.Error);

        foreach (var transactionProduct in transactionProducts)
        {
            var addProduct = customerTransaction.AddTransactionProduct(transactionProduct);
            if (!addProduct.Ok)
                return Operation.Error(addProduct.Error);
        }

        return customerTransaction;
    }

    public Operation<bool, string> Edit(
        ResponsibleValue responsible,
        CustomerTransactionType type,
        AmountValue? totalAmount,
        DateTime asOfDate,
        Customer customer,
        Company company,
        IEnumerable<CustomerTransactionProduct> transactionProducts)
    {
        if (responsible is null)
            return Operation.Error("Responsible is null!");

        Type = type;
        TotalAmount = totalAmount;
        AsOfDate = asOfDate;
        Responsible = responsible;

        var setProduct = SetCustomer(customer);
        if (!setProduct.Ok)
            return Operation.Error(setProduct.Error);

        var setCompany = SetCompany(company);
        if (!setCompany.Ok)
            return Operation.Error(setCompany.Error);

        _customerTransactionProducts.Clear();

        foreach (var transactionProduct in transactionProducts)
        {
            var addProduct = AddTransactionProduct(transactionProduct);
            if (!addProduct.Ok)
                return Operation.Error(addProduct.Error);
        }
        return true;
    }

    protected Operation<bool, string> AddTransactionProduct(CustomerTransactionProduct transactionProduct)
    {
        if (transactionProduct is null)
            return Operation.Error("CustomerTransactionProduct is null!");

        if (_customerTransactionProducts.Any(x => x.Id == transactionProduct.Id))
            return true;
        _customerTransactionProducts.Add(transactionProduct);

        return true;
    }

    protected Operation<bool, string> SetCustomer(Customer customer)
    {
        if (customer is null)
            return Operation.Error("Customer is null!");
        Customer = customer;

        return true;
    }

    protected Operation<bool, string> SetCompany(Company company)
    {
        if (company is null)
            return Operation.Error("Company is null!");
        Company = company;

        return true;
    }
}
