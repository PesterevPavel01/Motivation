using Motivation.Domain.Entities.Base;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.Domain.Entities;

public abstract class KpiFilter : Auditable
{
    protected KpiFilter(Guid id, TitleValue title)
    {
        Id = id;
        Title = title;
    }

    public Guid Id { get;}
    public TitleValue Title { get;}

    public virtual KpiFilterType KpiFilterType { get; private set; }
}
