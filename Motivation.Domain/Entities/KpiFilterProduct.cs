using Calabonga.OperationResults;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public class KpiFilterProduct : KpiFilter
{
    private KpiFilterProduct(Guid id, TitleValue title) : base(id, title)
    {
    }

    public static Operation<KpiFilterProduct, string> Create(TitleValue title)
    {
        if (title is null)
            return Operation.Error("Title is null!");

        return new KpiFilterProduct(Guid.CreateVersion7(), title);
    }

    public override KpiFilterType KpiFilterType => KpiFilterType.Product;
}
