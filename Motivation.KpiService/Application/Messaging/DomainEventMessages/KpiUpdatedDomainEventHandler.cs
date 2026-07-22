using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.QueueEvents;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.Events.KpiDomainEvents;
using Motivation.Application.Mappings;
using Rebus.Bus;
using Microsoft.EntityFrameworkCore;

namespace Motivation.KpiService.Application.Messaging.DomainEventMessages;

public class KpiUpdatedDomainEventHandler : INotificationHandler<KpiUpdatedDomainEvent>
{
    private readonly IBus _bus;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<KpiUpdatedDomainEventHandler> _logger;

    public KpiUpdatedDomainEventHandler(IBus bus, IUnitOfWork unitOfWork, ILogger<KpiUpdatedDomainEventHandler> logger)
    {
        _bus = bus;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask Handle(KpiUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork
            .GetRepository<PositionKpi>();

        var positionKpis = await repository
            .GetAllAsync(
                predicate: x => x.Kpi.Id == notification.KpiId,
                include: query => query
                    .Include(x => x.Position)
                        .ThenInclude(x => x.MotivationPart)
                    .Include(x => x.Kpi)
                        .ThenInclude(x => x.Filters)
                    .Include(x => x.Kpi)
                        .ThenInclude(x => x.MeasurementUnit)
                    .Include(x => x.TargetHistory)
                        .ThenInclude(x => x.Employee),
                trackingType: TrackingType.NoTracking);

        if (positionKpis is null || !positionKpis.Any())
        {
            _logger.LogError($"PositionKpi with kpi id {notification.KpiId} not found!");
            return;
        }

        var targets = positionKpis
            .SelectMany(x => x
                .TargetHistory
                .Where(target => target is not null)
                .Select(t => new { Target = t, KpiPosition = x})
            )
            .ToList();

        foreach (var target in targets)
        {
            if (target.Target.Employee is null)
            {
                _logger.LogError($"Employee is null for target id {target.Target.Id}!");
                return;
            }
            if (target.KpiPosition.Position is null)
            {
                _logger.LogError($"Position is null for target id {target.Target.Id}!");
                return;
            }

            var task = target.KpiPosition.MapToCalculationTaskDto(
                    target.Target.ValidFrom,
                    target.Target.ValidTo,
                    target.Target.Employee,
                    target.KpiPosition.Position,
                    target.Target.Id);

            if (task is null)
            {
                _logger.LogError($"Failed to map PositionKpi {target.KpiPosition.Id} to CalculationTaskDto!");
                return;
            }

            await _bus.Publish(new RecalculateKpisQueueCommand([task]));

            _logger.LogInformation($"RecalculateKpisQueueCommand published for target id {target.Target.Id}");
        }
    }
}
