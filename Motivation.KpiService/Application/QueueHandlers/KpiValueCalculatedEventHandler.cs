using Mediator;
using Motivation.Contracts.QueueEvents;
using Motivation.KpiService.Application.Messaging.Calculation;
using Rebus.Handlers;

namespace Motivation.KpiService.Application.QueueHandlers;

public class KpiValueCalculatedEventHandler : IHandleMessages<KpiValueCalculatedQueueEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<KpiValueCalculatedEventHandler> _logger;

    public KpiValueCalculatedEventHandler(IMediator mediator, ILogger<KpiValueCalculatedEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(KpiValueCalculatedQueueEvent message)
    {
        _logger.LogInformation("Message  {@KpiValueCalculatedQueueEvent} received!", typeof(KpiValueCalculatedQueueEvent).Name.ToString());
        var result = await _mediator.Send(new SetKpiValue.Request(message.Result));
        if (!result.Ok)
        {
            _logger.LogError(result.Error);
            return;
        }
    }
}
