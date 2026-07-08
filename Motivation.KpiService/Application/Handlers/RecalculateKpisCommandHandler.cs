using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Contracts.Interfaces;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;
using Motivation.KpiService.Application.Commands;

namespace Motivation.HRService.Application.DomainEventHandlers;

public class RecalculateKpisCommandHandler : INotificationHandler<RecalculateKpisCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnumerable<ICalculator> _calculators;
    public RecalculateKpisCommandHandler(IUnitOfWork unitOfWork, IEnumerable<ICalculator> calculators)
    {
        _unitOfWork = unitOfWork;
        _calculators = calculators;
    }

    async ValueTask INotificationHandler<RecalculateKpisCommand>.Handle(RecalculateKpisCommand command, CancellationToken cancellationToken)
    {
    //TODO: Дописать логику обновления покателя!
        var result = command.Model;

        var employeeCode = CodeValue.Create(command.Model.EmployeeCode);
        if (!employeeCode.Ok)
        {
            return;
        }

        var positionCode = CodeValue.Create(command.Model.PositionCode);

        if (!positionCode.Ok)
        {
            return;
        }

        var repository = _unitOfWork.GetRepository<Employee>();
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
            return;
        }

        var kpiCode = CodeValue.Create(command.Model.KpiCode);

        if (!kpiCode.Ok)
        {
            return;
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
            return;

        var calculator = _calculators.FirstOrDefault(x => x.KpiType == calculateKpi?.KpiType);
        if (calculator is null)
            return;

        var calculate = await calculator.CalculateKpiAsync(
            new CalculateEmployeeKpiDto(
                calculateModel,
                command.Model.PositionCode,
                command.Model.KpiCode,
                DateTime.UtcNow,
                DateTime.UtcNow));
    }
}
