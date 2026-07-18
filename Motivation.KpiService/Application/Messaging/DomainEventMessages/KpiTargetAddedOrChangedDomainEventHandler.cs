using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Motivation.Application.Mappings;
using Motivation.Contracts.QueueEvents;
using Motivation.Domain.DomainEvents.EmployeeDomainEvents;
using Motivation.Domain.Entities.Kpis;
using Rebus.Bus;

namespace Motivation.KpiService.Application.Messaging.DomainEventMessages;

public class KpiTargetAddedOrChangedDomainEventHandler : INotificationHandler<KpiTargetAddedOrChangedDomainEvent>
{
    private readonly IBus _bus;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<KpiTargetAddedOrChangedDomainEventHandler> _logger;

    public KpiTargetAddedOrChangedDomainEventHandler(IBus bus, IUnitOfWork unitOfWork, ILogger<KpiTargetAddedOrChangedDomainEventHandler> logger)
    {
        _bus = bus;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask Handle(KpiTargetAddedOrChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork
            .GetRepository<PositionKpi>();

        var positionKpi = await repository
            .GetFirstOrDefaultAsync(
                predicate: x => x.TargetHistory.Any(t => t.Id == notification.TargetId),
                include: query => query
                    .Include(x => x.Position)
                    .Include(x => x.Kpi)
                        .ThenInclude(x => x.Filters)
                    .Include(x => x.Kpi)
                        .ThenInclude(x => x.MeasurementUnit)
                    .Include(x => x.TargetHistory)
                        .ThenInclude(x => x.Employee),
                trackingType: TrackingType.NoTracking);

        if (positionKpi is null)
        {
            _logger.LogError($"PositionKpi with target id {notification.TargetId} not found!");
            return;
        }

        var target = positionKpi.TargetHistory
            .FirstOrDefault(x => x.Id == notification.TargetId);

        if (target is null)
        {
            _logger.LogError($"Target with id {notification.TargetId} not found in PositionKpi history!");
            return;
        }
        if (target.Employee is null)
        {
            _logger.LogError($"Employee is null for target id {notification.TargetId}!");
            return;
        }
        if (positionKpi.Position is null)
        {
            _logger.LogError($"Position is null for target id {notification.TargetId}!");
            return;
        }

        var task = positionKpi.MapToCalculationTaskDto(
                target!.ValidFrom,
                target!.ValidTo,
                target.Employee,
                positionKpi.Position);

        if (task is null)
        {
            _logger.LogError($"Failed to map PositionKpi {positionKpi.Id} to CalculationTaskDto!");
            return;
        }

        await _bus.Publish(new RecalculateKpisQueueCommand([task]));
        _logger.LogInformation($"RecalculateKpisQueueCommand published for target {notification.TargetId}");
    }
}
