using Calabonga.OperationResults;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities.Kpis;

public sealed class KpiFilterSalesResponsible : KpiFilter
{
    private KpiFilterSalesResponsible(Guid id, TitleValue title, bool include) : base(id, title, include)
    {
    }

    public static Operation<KpiFilterSalesResponsible, string> Create(TitleValue title, bool include)
    {
        if (title is null)
            return Operation.Error("Title is null!");

        return new KpiFilterSalesResponsible(Guid.CreateVersion7(), title, include);
    }

    public override KpiFilterType KpiFilterType => KpiFilterType.SalesResponsible;
}