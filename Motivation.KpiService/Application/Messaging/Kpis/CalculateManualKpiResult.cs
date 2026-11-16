using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Contracts.Models;
using Motivation.Contracts.QueueEvents;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.Enum;
using Rebus;
using Rebus.Bus;

namespace Motivation.KpiService.Application.Messaging.Kpis;

public class CalculateManualKpiResult
{
    public record Request(SetEmployeeKpiResult Model) : IRequest<Operation<KpiCalculationTaskResult, string>>;

    public class Handler(IMediator mediator, IBus bus, IUnitOfWork unitOfWork, ILogger<CalculateManualKpiResult> logger)
        : IRequestHandler<Request, Operation<KpiCalculationTaskResult, string>>
    {
        public async ValueTask<Operation<KpiCalculationTaskResult, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork
            .GetRepository<PositionKpi>();

            var positionKpi = await repository
                .GetFirstOrDefaultAsync(
                    predicate: x => x.TargetHistory.Any(t => t.Id == request.Model.Id),
                    include: query => query
                        .Include(x => x.Position)
                            .ThenInclude(x => x.MotivationPart)
                        .Include(x => x.Kpi)
                            .ThenInclude(x => x.Filters)
                        .Include(x => x.Kpi)
                            .ThenInclude(x => x.MeasurementUnit)
                        .Include(x => x.TargetHistory)
                            .ThenInclude(x => x.Employee)
                        .Include(x => x.TargetHistory)
                            .ThenInclude(x => x.KpiFact!),
                    trackingType: TrackingType.NoTracking);

            if (positionKpi is null)
            {
                logger.LogError($"PositionKpi with target id {request.Model.Id} not found!");
                return Operation.Error($"PositionKpi with target id {request.Model.Id} not found!");
            }

            var target = positionKpi.TargetHistory
                .FirstOrDefault(x => x.Id == request.Model.Id);

            if (target is null)
            {
                logger.LogError($"Target with id {request.Model.Id} not found in PositionKpi history!");
                return Operation.Error($"Target with id {request.Model.Id} not found in PositionKpi history!");
            }
            if (target.Employee is null)
            {
                logger.LogError($"Employee is null for target id {request.Model.Id}!");
                return Operation.Error($"Employee is null for target id {request.Model.Id}!");
            }
            if (positionKpi.Position is null)
            {
                logger.LogError($"Position is null for target id {request.Model.Id}!");
                return Operation.Error($"Position is null for target id {request.Model.Id}!");
            }

            if (positionKpi.Kpi?.KpiType != KpiType.Manual)
            {
                logger.LogError($"KPI has type not ManualKpi!");
                return Operation.Error($"KPI has type not ManualKpi!");
            }

            var task = positionKpi.MapToCalculationTaskDto(
                    target!.ValidFrom,
                    target!.ValidTo,
                    target.Employee,
                    positionKpi.Position,
                    request.Model.Id);

            if (task is null)
            {
                logger.LogError($"Failed to map PositionKpi {positionKpi.Id} to CalculationTaskDto!");
                return Operation.Error($"Failed to map PositionKpi {positionKpi.Id} to CalculationTaskDto!");
            }

            task.ManualResult = new ManualResultDto(request.Model.Result);

            var recalculateKpiRequest = new RecalculateKpiRequest(task);

            var response = await bus.SendRequest<RecalculateKpiResponse>(
                new RecalculateKpiRequest(task), 
                timeout: TimeSpan.FromSeconds(15), 
                externalCancellationToken: cancellationToken);

            if (response.Result is null)
            {
                logger.LogInformation(response.Error);
                return Operation.Error(response.Error ?? "Error from RecalculateKpiRequestHandler");
            }
            
            return response.Result;
        }
    }
}
