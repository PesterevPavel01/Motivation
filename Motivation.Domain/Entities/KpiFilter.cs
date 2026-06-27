using Motivation.Domain.Entities.Base;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public abstract class KpiFilter : Auditable
{
    protected KpiFilter(Guid id, TitleValue title, bool include)
    {
        Id = id;
        Title = title;
        Include = include;
    }
    public bool Include { get; private set; } = true;
    public Guid Id { get;}
    public TitleValue Title { get;}

    public virtual KpiFilterType KpiFilterType { get; private set; }
}
