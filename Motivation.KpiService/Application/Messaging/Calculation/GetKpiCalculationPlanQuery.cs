using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Domain.Entities;
using Motivation.Application.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Motivation.KpiService.Application.Messaging.Calculation;

public class GetKpiCalculationPlanQuery
{
    public record Request(DateTime PeriodFrom, DateTime PeriodTo) : IRequest<Operation<IEnumerable<KpiCalculationTask>, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<IEnumerable<KpiCalculationTask>, string>>
    {
        public async ValueTask<Operation<IEnumerable<KpiCalculationTask>, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            if (request.PeriodFrom > request.PeriodTo)
            {
                return Operation.Error("DateStart cannot be greater than DateEnd.");
            }

            var repository = unitOfWork
                .GetRepository<EmployeePosition>();

            var employeePositions = await repository
                .GetAllAsync(
                    predicate: x => x.AssignmentDate <= request.PeriodTo,
                    include: query => query
                        .Include(x => x.Employee)
                        .Include(x => x.Position)
                            .ThenInclude(x => x.PositionKpiHistory)
                                .ThenInclude(x => x.Kpi)
                                    .ThenInclude(x => x.Filters)
                        .Include(x => x.Position)
                            .ThenInclude(x => x.PositionKpiHistory)
                                .ThenInclude(x => x.Kpi)
                                    .ThenInclude(x => x.MeasurementUnit)
                        .Include(x => x.Position)
                            .ThenInclude(x => x.PositionKpiHistory)
                                .ThenInclude(x => x.TargetHistory),
                    trackingType: TrackingType.NoTracking);

            var tasks = employeePositions
                .Where (x => x is not null)
                .Select(x => x.Position.PositionKpiHistory
                .Where(pk =>
                    pk.ValidFrom <= request.PeriodTo
                    && pk.ValidTo >= request.PeriodFrom
                    && pk.TargetHistory
                        .Any(t => 
                            t.ValidFrom >= request.PeriodFrom 
                            && t.ValidTo <= request.PeriodTo)
                    && pk.Kpi is not null)
                .Select(pk => pk.MapToCalculationTaskDto(
                    request.PeriodFrom,
                    request.PeriodTo,
                    x.Employee,
                    x.Position))
                .Where(x => x is not null)
                .Select(x => x!))
                .SelectMany(x => x)
                .ToArray();

            if (tasks is null)
            {
                return new List<KpiCalculationTask>();
            }
            else
            {
                return tasks.ToArray();
            }
        }
    }
}