using Calabonga.OperationResults;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities.Kpis;

public sealed class KpiFilterCustomerResponsible : KpiFilter
{
    private KpiFilterCustomerResponsible(Guid id, TitleValue title, bool include) : base(id, title, include)
    {
    }

    public static Operation<KpiFilterCustomerResponsible, string> Create(TitleValue title, bool include)
    {
        if (title is null)
            return Operation.Error("Title is null!");

        return new KpiFilterCustomerResponsible(Guid.CreateVersion7(), title, include);
    }

    public override KpiFilterType KpiFilterType => KpiFilterType.CustomerResponsible;
}