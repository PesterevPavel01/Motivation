using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Domain.Entities;

public class Company : AggregateRoot
{
    private readonly List<Receivable> _receivables = [];
    private readonly List<CustomerTransaction> _customerTransactions = [];

    protected Company(Guid id, CodeValue code, TitleValue title) : base(title, code, id)
    {
    }

    public IReadOnlyCollection<Receivable> Receivables => _receivables.AsReadOnly();
    public IReadOnlyCollection<CustomerTransaction> CustomerTransactions => _customerTransactions.AsReadOnly();

    public static Operation<Company, string> Create(TitleValue title, CodeValue code)
    {
        if (title is null)
            return Operation.Error("Title is null!");
        if (code is null)
            return Operation.Error("Code is null!");

        return new Company(Guid.CreateVersion7(), code, title);
    }

    public Operation<Company, string> EditEntity(TitleValue title)
    {
        if (title is null)
            return Operation.Error("Title is null!");
        Title = title;
        return this;
    }
    public Operation<bool, string> AddReceivable(Receivable receivable)
    {
        if (receivable is null)
            return Operation.Error("Receivable is null!");
        if (_receivables.Any(x => x.AsOfDate == receivable.AsOfDate && x.Customer.Code == receivable.Customer.Code))
            return Operation.Error("Receivable for this customer and date already exists!");
        _receivables.Add(receivable);
        return true;
    }

    public Operation<bool, string> PerformTransaction(CustomerTransaction customerTransaction)
    {
        if (customerTransaction is null)
            return Operation.Error("CustomerTransaction is null!");
        if (_customerTransactions.Any(x => x.Code == customerTransaction.Code))
            return Operation.Error($"Transaction with code '{customerTransaction.Code}' already exists!");
        _customerTransactions.Add(customerTransaction);
        return true;
    }
    public Operation<bool, string> DeleteReceivable(Receivable receivable)
    {
        _receivables.Remove(receivable);
        return true;
    }
}
