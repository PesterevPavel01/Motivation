using Mediator;
using Motivation.Contracts.Events;
using Motivation.Domain.DomainEvents.PositionDomainEvents;
using Rebus.Bus;

namespace Motivation.HRService.Application.DomainEventHandlers;

public class PositionUpdatedDomainEventHandler : INotificationHandler<PositionUpdatedDomainEvent>
{
    private readonly IBus _bus;
    public PositionUpdatedDomainEventHandler(IBus bus)
    {
        _bus = bus;
    }

    async ValueTask INotificationHandler<PositionUpdatedDomainEvent>.Handle(PositionUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await _bus.Publish(new PositionUpdatedQueueEvent(domainEvent.Id));
    }
}
