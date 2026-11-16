using Mediator;
using Motivation.Contracts.QueueEvents;
using Motivation.KpiCalculationService.Application.Messaging;
using Rebus.Bus;
using Rebus.Handlers;

namespace Motivation.KpiCalculationService.Application.QueueHandlers;

public class RecalculateKpisCommandHandler : IHandleMessages<RecalculateKpisQueueCommand>
{
    private readonly IMediator _mediator;
    private readonly ILogger<RecalculateKpisCommandHandler> _logger;
    private readonly IBus _bus;

    public RecalculateKpisCommandHandler(IMediator mediator, ILogger<RecalculateKpisCommandHandler> logger, IBus bus)
    {
        _mediator = mediator;
        _logger = logger;
        _bus = bus;
    }

    public async Task Handle(RecalculateKpisQueueCommand message)
    {
        _logger.LogInformation("Message  {@RecalculateKpisQueueCommand} received!", typeof(RecalculateKpisQueueCommand).Name.ToString());
        foreach (var task in message.Models)
        {
            var result = await _mediator.Send(new CalculateKpi.Request(task));
            if (!result.Ok)
            {
                _logger.LogError(result.Error);
                return;
            }
            await _bus.Publish(new KpiValueCalculatedQueueEvent(result.Result));
        }
    }
}