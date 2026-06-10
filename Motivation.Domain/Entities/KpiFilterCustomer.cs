using Calabonga.OperationResults;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public sealed class KpiFilterCustomer : KpiFilter
{
    private KpiFilterCustomer(Guid id, TitleValue title) : base(id, title)
    {     
    }

    public static Operation<KpiFilterCustomer, string> Create(TitleValue title)
    {
        if (title is null)
            return Operation.Error("Title is null!");

        return new KpiFilterCustomer(Guid.CreateVersion7(),title);
    }

    public override KpiFilterType KpiFilterType => KpiFilterType.Customer;
}