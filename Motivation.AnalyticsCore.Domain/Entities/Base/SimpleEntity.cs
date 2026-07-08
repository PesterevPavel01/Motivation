using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Domain.Entities.Base;

public class SimpleEntity : Entity
{
    protected SimpleEntity(TitleValue title, CodeValue code, Guid id) : base(id, code)
    {
        Title = title;
    }

    public TitleValue Title { get; protected set; }
}
