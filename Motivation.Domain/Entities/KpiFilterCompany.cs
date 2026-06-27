using Calabonga.OperationResults;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public sealed class KpiFilterCompany : KpiFilter
{
    private KpiFilterCompany(Guid id, TitleValue title, bool include) : base(id, title, include)
    {
    }

    public static Operation<KpiFilterCompany, string> Create(TitleValue title, bool include)
    {
        if (title is null)
            return Operation.Error("Title is null!");

        return new KpiFilterCompany(Guid.CreateVersion7(), title, include);
    }

    public override KpiFilterType KpiFilterType => KpiFilterType.Company;
}