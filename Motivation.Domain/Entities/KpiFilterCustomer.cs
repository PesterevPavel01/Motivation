using Calabonga.OperationResults;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public sealed class KpiFilterCustomer : KpiFilter
{
    private KpiFilterCustomer(Guid id, TitleValue title, bool include) : base(id, title, include)
    {     
    }

    public static Operation<KpiFilterCustomer, string> Create(TitleValue title, bool include)
    {
        if (title is null)
            return Operation.Error("Title is null!");

        return new KpiFilterCustomer(Guid.CreateVersion7(),title, include);
    }

    public override KpiFilterType KpiFilterType => KpiFilterType.Customer;
}