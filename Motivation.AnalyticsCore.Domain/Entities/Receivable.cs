using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Domain.Entities;

public class Receivable : Auditable
{
    protected Receivable(Guid id, AmountValue amount, DateTime asOfDate)
    {
        Id = id;
        Amount = amount;
        AsOfDate = asOfDate;
    }
    public Guid Id { get; protected set; }
    public AmountValue Amount { get; protected set; }
    public Customer Customer { get; protected set; } = null!;
    public Company Company { get; protected set; } = null!;
    public DateTime AsOfDate { get; private set; }

    public static Operation<Receivable, string> Create(AmountValue amount, Customer customer, Company company, DateTime asOfDate)
    {
        if (amount is null)
            return Operation.Error("Amount is null!");

        var receivable = new Receivable(Guid.CreateVersion7(), amount, asOfDate);

        var setCustomer = receivable.SetCustomer(customer);
        if (!setCustomer.Ok)
            return Operation.Error(setCustomer.Error);
        var setCompany = receivable.SetCompany(company);
        if (!setCompany.Ok)
            return Operation.Error(setCustomer.Error);

        return receivable;
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
