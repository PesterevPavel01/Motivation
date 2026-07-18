using Mediator;
using Motivation.Contracts.QueueEvents;
using Motivation.KpiService.Application.Messaging.Calculation;
using Rebus.Bus;
using Rebus.Handlers;

namespace Motivation.KpiService.Application.QueueHandlers;

public class SourceDataSyncedEventHandler : IHandleMessages<SourceDataSyncedQueueEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<SourceDataSyncedEventHandler> _logger;
    private readonly IBus _bus;

    public SourceDataSyncedEventHandler(IMediator mediator, ILogger<SourceDataSyncedEventHandler> logger, IBus bus)
    {
        _mediator = mediator;
        _logger = logger;
        _bus = bus;
    }

    public async Task Handle(SourceDataSyncedQueueEvent message)
    {

        var result = await _mediator.Send(new GetKpiCalculationPlanQuery.Request(message.DateStart, message.DateEnd));
        if (!result.Ok)
        {
            _logger.LogError(result.Error);
            return;
        }
        await _bus.Publish(new RecalculateKpisQueueCommand(result.Result.ToList()));
    }
}