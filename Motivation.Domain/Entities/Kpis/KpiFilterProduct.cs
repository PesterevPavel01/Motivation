using Calabonga.OperationResults;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities.Kpis;

public class KpiFilterProduct : KpiFilter
{
    private KpiFilterProduct(Guid id, TitleValue title, bool include) : base(id, title, include)
    {
    }

    public static Operation<KpiFilterProduct, string> Create(TitleValue title, bool include)
    {
        if (title is null)
            return Operation.Error("Title is null!");

        return new KpiFilterProduct(Guid.CreateVersion7(), title, include);
    }

    public override KpiFilterType KpiFilterType => KpiFilterType.Product;
}
