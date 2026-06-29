using Calabonga.OperationResults;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities.Kpis;

public class KpiFilterProductResponsible : KpiFilter
{
    private KpiFilterProductResponsible(Guid id, TitleValue title, bool include) : base(id, title, include)
    {
    }

    public static Operation<KpiFilterProductResponsible, string> Create(TitleValue title, bool include)
    {
        if (title is null)
            return Operation.Error("Title is null!");

        return new KpiFilterProductResponsible(Guid.CreateVersion7(), title, include);
    }

    public override KpiFilterType KpiFilterType => KpiFilterType.ProductResponsible;
}
