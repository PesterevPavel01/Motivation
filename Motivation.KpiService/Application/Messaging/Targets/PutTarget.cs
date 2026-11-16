using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.ValueObjects;

namespace Motivation.KpiService.Application.Messaging.Targets;

public class PutTarget
{
    public record Request(SetEmployeeKpiTarget model) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger, IServiceProvider serviceProvider)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var employeeCode = CodeValue.Create(request.model.EmployeeCode);
            if (!employeeCode.Ok)
            {
                return Operation.Error(employeeCode.Error);
            }

            var kpiCode = CodeValue.Create(request.model.KpiCode);
            if (!kpiCode.Ok)
            {
                return Operation.Error(kpiCode.Error);
            }

            var employeeRepository = unitOfWork.GetRepository<Employee>();
            
            var employee = await employeeRepository
                .GetFirstOrDefaultAsync(
                    predicate: x => 
                        x.Code == employeeCode.Result,
                    include: query => query
                        .Include(x => x.TargetHistory)
                            .ThenInclude(x => x.PositionKpi)
                                .ThenInclude(x => x.Kpi)
                        .Include(x => x.EmployeePositions)
                            .ThenInclude(x => x.Position)
                                .ThenInclude(x => x.PositionKpiHistory)
                                    .ThenInclude(x => x.Kpi),
                    trackingType: TrackingType.Tracking);
            
            if (employee is null)
            {
                return Operation.Error("Employee not found!");
            }
            
            var hasOverlap = employee.TargetHistory
                .Where(t => t.Employee.Code == employeeCode.Result)
                .Any(t =>
                    ((request.model.ValidFrom >= t.ValidFrom
                        && request.model.ValidFrom <= t.ValidTo)
                        || (request.model.ValidTo >= t.ValidFrom
                        && request.model.ValidTo <= t.ValidTo)
                        || (request.model.ValidFrom <= t.ValidFrom
                        && request.model.ValidTo >= t.ValidTo))
                    && !(request.model.ValidFrom == t.ValidFrom
                        && request.model.ValidTo == t.ValidTo));

            if (hasOverlap)
                return Operation.Error($"Target already exists for employee {request.model.EmployeeCode} in period {request.model.ValidFrom:yyyy-MM-dd} - {request.model.ValidTo:yyyy-MM-dd}");

            var positionKpi = employee.EmployeePositions
                .SelectMany(ep => ep.Position.PositionKpiHistory)
                .FirstOrDefault(pk => pk.Kpi.Code == kpiCode.Result);

            if (positionKpi is null)
            {
                return Operation.Error($"KPI with code '{request.model.KpiCode}' not found for employee '{request.model.EmployeeCode}'!");
            }

            var targetValue = TargetValue.Create(request.model.TargetValue);
            if (!targetValue.Ok)
            {
                return Operation.Error(targetValue.Error);
            }

            var existingTarget = positionKpi.TargetHistory
                .FirstOrDefault(t => 
                t.Employee.Code == employeeCode.Result
                && (request.model.ValidFrom == t.ValidFrom
                        && request.model.ValidTo == t.ValidTo));

            if (existingTarget is null)
            {
                var newTarget = KpiTarget.Create(
                employee,
                targetValue.Result,
                request.model.ValidFrom,
                request.model.ValidTo);

                if (!newTarget.Ok)
                {
                    return Operation.Error(newTarget.Error);
                }

                var addResult = employee.AddTarget(positionKpi, newTarget.Result);
                if (!addResult.Ok)
                {
                    return Operation.Error(addResult.Error);
                }
            }
            else 
            {
                var editResult = employee.EditTarget(existingTarget, targetValue.Result);
                if (!editResult.Ok)
                {
                    return Operation.Error(editResult.Error);
                }
            }

            employeeRepository.Update(employee);

            var saveResult = await unitOfWork.SaveChangesAsync();

            if (unitOfWork.Result.Exception is not null)
            {
                return Operation.Error(unitOfWork.Result.Exception.Message);
            }

            return true;
        }
    }
}
