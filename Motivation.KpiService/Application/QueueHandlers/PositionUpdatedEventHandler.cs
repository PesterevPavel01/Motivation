using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Motivation.Contracts.Dto.CalculateDto;
using Motivation.Contracts.Events;
using Motivation.Domain.Entities;
using Motivation.KpiService.Application.Commands;
using Rebus.Handlers;

namespace Motivation.KpiService.Application.QueueHandlers;

public class PositionUpdatedEventHandler : IHandleMessages<PositionUpdatedQueueEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;

    public PositionUpdatedEventHandler(IUnitOfWork unitOfWork , IPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _publisher = publisher;
    }

    public async Task Handle(PositionUpdatedQueueEvent message)
    {
        var repository = _unitOfWork.GetRepository<Employee>();
        var employees = await repository
            .GetAllAsync(
                predicate: x =>
                    x.EmployeePositions
                        .Any(p => p.Position.Id == message.Id),
                include: query => query
                    .Include(x => x.EmployeePositions)
                       .ThenInclude(ep => ep.Position)
                            .ThenInclude(p => p.PositionKpiHistory)
                                .ThenInclude(h => h.Kpi),
                trackingType: TrackingType.NoTracking);

        var models = employees
            .SelectMany(employee => employee.EmployeePositions
                .Where(ep => ep.Position.Id == message.Id)
                    .SelectMany(ep => ep.Position.PositionKpiHistory
                        .Select(kpi => new CalculateKpiDto(
                            EmployeeCode: employee.Code.Value,
                            PositionCode: ep.Position.Code.Value,
                            KpiCode: kpi.Kpi.Code.Value))));

        foreach (var model in models)
        {
            await _publisher.Publish(new RecalculateKpisCommand(model), default);
        }
    }
}
