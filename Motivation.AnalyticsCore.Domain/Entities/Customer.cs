using Calabonga.OperationResults;
using Motivation.AnalyticsCore.Domain.Entities.Base;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Domain.Entities;

public class Customer : SimpleEntity
{
    private readonly List<Receivable> _receivables = [];
    private readonly List<CustomerTransaction> _customerTransactions = [];
    private readonly List<Customer> _slaveCustomers = [];

    protected Customer(Guid id, CodeValue code, TitleValue title, FullNameValue? fullName, ManagerValue? manager) : base(title, code, id)
    {
        FullName = fullName;
        Manager = manager;
    }

    public IReadOnlyCollection<Receivable> Receivables => _receivables.AsReadOnly();
    public IReadOnlyCollection<CustomerTransaction> CustomerTransactions => _customerTransactions.AsReadOnly();
    public IReadOnlyCollection<Customer> SlaveCustomers => _slaveCustomers.AsReadOnly();

    public FullNameValue? FullName { get; set; }
    public CustomerGroup? CustomerGroup {  get; set; }
    public Customer? MasterCustomer { get; set; }
    public ManagerValue? Manager { get; set; }

    public static Operation<Customer, string> Create(TitleValue title, CodeValue code, FullNameValue? fullName, ManagerValue? manager, CustomerGroup? customerGroup)
    {
        if (title is null)
            return Operation.Error("Title is null!");
        if (code is null)
            return Operation.Error("Code is null!");

        var customer = new Customer(Guid.CreateVersion7(), code, title, fullName, manager);
        customer.SetCustomerGroup(customerGroup);

        return customer;
    }

    protected void SetCustomerGroup(CustomerGroup? customerGroup)
    {
        CustomerGroup = customerGroup;
    }
    public void SetMaster(Customer? master)
    {
        MasterCustomer = master;
    }

    public Operation<Customer, string> EditEntity(
        TitleValue title,
        FullNameValue? fullName, 
        ManagerValue? manager, 
        CustomerGroup? customerGroup)
    {
        if (title is null)
            return Operation.Error("Title is null!");

        Title = title;
        FullName = fullName;
        Manager = manager;
        SetCustomerGroup(customerGroup);

        return this;
    }

    public Operation<bool, string> DeleteReceivable(Receivable receivable)
    {
        _receivables.Remove(receivable);
        return true;
    }
}
