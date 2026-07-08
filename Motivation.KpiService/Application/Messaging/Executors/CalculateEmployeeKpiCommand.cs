using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Contracts.Interfaces;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.KpiService.Application.Messaging.Executors;

public class CalculateEmployeeKpiCommand
{
    public record Request(CalculateKpiDto Model) : IRequest<Operation<decimal, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger, IEnumerable<ICalculator> calculators)
        : IRequestHandler<Request, Operation<decimal, string>>
    {
        public async ValueTask<Operation<decimal, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var employeeCode = CodeValue.Create(request.Model.EmployeeCode);
            return 10;

            /*if (!employeeCode.Ok)
            {
                return Operation.Error(employeeCode.Error);
            }

            var positionCode = CodeValue.Create(request.Model.PositionCode);

            if (!positionCode.Ok)
            {
                return Operation.Error(positionCode.Error);
            }

            var repository = unitOfWork.GetRepository<Employee>();
            var employee = await repository
                .GetFirstOrDefaultAsync(
                    predicate: x => 
                        x.Code == employeeCode.Result 
                        && x.EmployeePositions
                            .Any(p => 
                                p.Position.Code == positionCode.Result),
                    include: query => query
                        .Include(x => x.EmployeePositions)
                           .ThenInclude(ep => ep.Position)
                                .ThenInclude(p => p.PositionKpiHistory)
                                    .ThenInclude(h => h.Kpi)
                                        .ThenInclude(k => k.Filters)
                        .Include(x => x.EmployeePositions)
                           .ThenInclude(ep => ep.Position)
                                .ThenInclude(p => p.PositionKpiHistory)
                                    .ThenInclude(h => h.Kpi)
                                        .ThenInclude(k => k.MeasurementUnit)
                        .Include(x => x.EmployeePositions)
                           .ThenInclude(ep => ep.DeductionHistory)
                        .Include(x => x.EmployeePositions)
                           .ThenInclude(ep => ep.ExtraPartHistory)
                        .Include(x => x.EmployeePositions)
                           .ThenInclude(ep => ep.ActualHoursHistory),
                    trackingType: TrackingType.NoTracking);

            if (employee is null)
            {
                return Operation.Error("Employee not found!");
            }

            var kpiCode = CodeValue.Create(request.Model.KpiCode);

            if (!kpiCode.Ok)
            {
                return Operation.Error(kpiCode.Error);
            }

            var calculateKpi = employee
                .EmployeePositions
                    .FirstOrDefault(x => x.Position.Code == positionCode.Result)?
                    .Position?
                    .PositionKpiHistory?
                    .FirstOrDefault(x => x.Kpi.Code == kpiCode.Result)?
                    .Kpi;

            var calculateModel = employee.MapToDto();
            if (calculateModel is null)
                return Operation.Error("Kpi not found!");

            var calculator = calculators.FirstOrDefault(x => x.KpiType == calculateKpi?.KpiType);
            if (calculator is null)
                return Operation.Error($"Calculator for not found!");

            var calculate = await calculator.CalculateKpiAsync(
                new CalculateEmployeeKpiDto(
                    calculateModel, 
                    request.Model.PositionCode,
                    request.Model.KpiCode,
                    request.Model.PeriodFrom,
                    request.Model.PeriodTo));

            if (!calculate.Ok)
                return Operation.Error(calculate.Error);

            return calculate.Result;*/
        }
    }
}
