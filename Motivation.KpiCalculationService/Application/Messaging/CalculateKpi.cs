using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Contracts.Interfaces;
using Motivation.Contracts.QueueEvents;
using Rebus.Bus;

namespace Motivation.KpiCalculationService.Application.Messaging;

public class CalculateKpi
{
    public record Request(KpiCalculationTask Model) : IRequest<Operation<KpiCalculationTaskResult, string>>;

    public class Handler(
        IUnitOfWork unitOfWork, 
        ILogger<CalculateKpi> logger, 
        IEnumerable<ICalculator> calculators,
        IBus bus)
        : IRequestHandler<Request, Operation<KpiCalculationTaskResult, string>>
    {
        public async ValueTask<Operation<KpiCalculationTaskResult, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var calculator = calculators.FirstOrDefault(x => x.KpiType.ToString() == request.Model.KpiType);
            if (calculator is null)
            {
                logger.LogError($"Calculator for {request.Model.KpiType} not found!");
                return Operation.Error($"Calculator for {request.Model.KpiType} not found!");
            }

            var calculate = await calculator.CalculateKpiAsync(request.Model);

            if (!calculate.Ok)
            {
                logger.LogError(calculate.Error);
                return Operation.Error(calculate.Error);
            }

            await bus.Publish(new KpiValueCalculatedQueueEvent(calculate.Result));
            logger.LogInformation("Message  {@KpiValueCalculatedQueueEvent} sent!", typeof(KpiValueCalculatedQueueEvent).Name.ToString());

            return calculate.Result;
        }
    }
}
