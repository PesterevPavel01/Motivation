using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Domain.Entities;

public class CustomerGroup : SimpleEntity
{
    private readonly List<Customer> _customers = [];
    private readonly List<CustomerGroup> _childGroups = [];

    protected CustomerGroup(TitleValue title, CodeValue code, Guid id) : base(title, code, id)
    {
    }

    public CustomerGroup? ParentGroup { get; set; }

    public IReadOnlyCollection<CustomerGroup> ChildGroups => _childGroups.AsReadOnly();
    public IReadOnlyCollection<Customer> Customers => _customers.AsReadOnly();

    public static Operation<CustomerGroup, string> Create(TitleValue title, CodeValue code, CustomerGroup? parentGroup)
    {
        if (title is null)
            return Operation.Error("Title is null!");
        if (code is null)
            return Operation.Error("Code is null!");

        var customerGroup = new CustomerGroup(title, code, Guid.CreateVersion7());

        if (parentGroup is not null)
            customerGroup.SetParent(parentGroup);

        return customerGroup;
    }

    public Operation<bool, string> Edit(TitleValue title, CustomerGroup? parentGroup)
    {
        if (title is null)
            return Operation.Error("Title is null!");

        Title = title;

        if (parentGroup is not null)
            SetParent(parentGroup);

        return true;
    }

    public void SetParent(CustomerGroup? parentGroup)
    {
        ParentGroup = parentGroup;
    }

    public void AddChildGroup(CustomerGroup childGroup)
    {
        childGroup.SetParent(this);
        if (_childGroups.Any(x => x.Code == childGroup.Code))
            return;

        _childGroups.Add(childGroup);
    }
}