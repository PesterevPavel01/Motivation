using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;
using Motivation.AnalyticsCore.Domain.Enum;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Domain.Entities;

public class CustomerTransaction : Entity
{
    private readonly List<CustomerTransactionProduct> _CustomerTransactionProducts = [];
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

    public IReadOnlyCollection<CustomerTransactionProduct> CustomerTransactionProducts => _CustomerTransactionProducts.AsReadOnly();

    public static Operation<CustomerTransaction, string> Create(CodeValue code, ResponsibleValue responsible, CustomerTransactionType type, AmountValue? totalAmount, DateTime asOfDate, Customer customer, Company company)
    {
        if (code is null)
            return Operation.Error("Code is null!");
        if (responsible is null)
            return Operation.Error("Responsible is null!");

        var warehouseTransaction = new CustomerTransaction(Guid.CreateVersion7(), code, responsible, type, totalAmount, asOfDate);

        var setProduct = warehouseTransaction.SetCustomer(customer);
        if (!setProduct.Ok)
            return Operation.Error(setProduct.Error);

        var setCompany = warehouseTransaction.SetCompany(company);
        if (!setCompany.Ok)
            return Operation.Error(setCompany.Error);

        return warehouseTransaction;
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
