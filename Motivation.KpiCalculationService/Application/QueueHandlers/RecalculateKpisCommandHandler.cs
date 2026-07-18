using Mediator;
using Motivation.Contracts.QueueEvents;
using Motivation.KpiCalculationService.Application.Messaging;
using Rebus.Handlers;

namespace Motivation.KpiCalculationService.Application.QueueHandlers;

public class RecalculateKpisCommandHandler : IHandleMessages<RecalculateKpisQueueCommand>
{
    private readonly IMediator _mediator;
    private readonly ILogger<RecalculateKpisCommandHandler> _logger;

    public RecalculateKpisCommandHandler(IMediator mediator, ILogger<RecalculateKpisCommandHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
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
        }
    }
}