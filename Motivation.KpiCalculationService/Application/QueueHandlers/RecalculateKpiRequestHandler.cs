using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.QueueEvents;
using Motivation.KpiCalculationService.Application.Messaging;
using Rebus.Bus;
using Rebus.Handlers;

namespace Motivation.KpiCalculationService.Application.QueueHandlers;

public class RecalculateKpiRequestHandler : IHandleMessages<RecalculateKpiRequest>
{
    private readonly IMediator _mediator;
    private readonly ILogger<RecalculateKpiRequestHandler> _logger;
    private readonly IBus _bus;

    public RecalculateKpiRequestHandler(IMediator mediator, ILogger<RecalculateKpiRequestHandler> logger, IBus bus)
    {
        _mediator = mediator;
        _logger = logger;
        _bus = bus;
    }
    public async Task Handle(RecalculateKpiRequest message)
    {
        _logger.LogInformation("Message  {@RecalculateKpiRequest} received!", typeof(RecalculateKpiRequest).Name.ToString());

        var result = await _mediator.Send(new CalculateKpi.Request(message.Model));
        if (!result.Ok)
        {
            _logger.LogError(result.Error);
            await _bus.Reply(new RecalculateKpiResponse(Result: null, Error: result.Error));
            return;
        }

        var response = new RecalculateKpiResponse(Result: result.Result, null);

        await _bus.Reply(response);
    }
}
