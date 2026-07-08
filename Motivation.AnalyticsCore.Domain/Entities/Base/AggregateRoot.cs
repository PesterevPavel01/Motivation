using Motivation.AnalyticsCore.Domain.Interfaces;
using Motivation.AnalyticsCore.Domain.ValueObjects;

namespace Motivation.AnalyticsCore.Domain.Entities.Base;

public abstract class AggregateRoot : SimpleEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(TitleValue title, CodeValue code, Guid id) : base(title, code, id) {}

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents()
    {
        return [.. _domainEvents];
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
