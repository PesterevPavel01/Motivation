using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.ValueObjects;
using Motivation.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Motivation.KpiService.Application.Messaging.Kpis
{
    public class AssignKpi
    {
        public record Request(string PositionCode, IEnumerable<AssignKpiToPositionDto> Models) : IRequest<Operation<bool, string>>;

        public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger, IServiceProvider serviceProvider)
            : IRequestHandler<Request, Operation<bool, string>>
        {
            public async ValueTask<Operation<bool, string>> Handle(Request positionRequest, CancellationToken cancellationToken)
            {
                var positionCode = CodeValue.Create(positionRequest.PositionCode);
                if (!positionCode.Ok)
                {
                    return Operation.Error(positionCode.Error);
                }

                var positionRepository = unitOfWork.GetRepository<Position>();
                var position = await positionRepository.GetFirstOrDefaultAsync(
                    predicate: x => x.Code == positionCode.Result,
                    include: query => query
                        .Include(x => x.PositionKpiHistory),
                    trackingType: TrackingType.Tracking);
                if (position is null)
                {
                    return Operation.Error("Position not found!");
                }

                var clearHistory = position.ClearKpiHistory();
                if (!clearHistory.Ok)
                {
                    return Operation.Error(clearHistory.Error);
                }

                foreach (var model in positionRequest.Models) 
                {
                    var assign = await AssignKpi(position, model);
                    if (!assign.Ok)
                    {
                        return Operation.Error(assign.Error);
                    }
                }

                var dbContext = serviceProvider.GetRequiredService<MotivationDbContext>();
                var entries = dbContext.ChangeTracker.Entries();

                var result = await unitOfWork.SaveChangesAsync();
                if (unitOfWork.Result.Exception is not null)
                {
                    return Operation.Error(unitOfWork.Result.Exception.Message);
                }

                return true;
            }

            private async Task<Operation<bool, string>> AssignKpi(Position position, AssignKpiToPositionDto model)
            {
                var kpiCode = CodeValue.Create(model.KpiCode);
                if (!kpiCode.Ok)
                {
                    return Operation.Error(kpiCode.Error);
                }

                var kpiRepository = unitOfWork.GetRepository<Kpi>();
                var kpi = await kpiRepository.GetFirstOrDefaultAsync(
                    predicate: x => x.Code == kpiCode.Result,
                    trackingType: TrackingType.Tracking);
                if (kpi is null)
                {
                    return Operation.Error("Kpi not found!");
                }

                var weight = WeightValue.Create(model.Weight);
                if (!weight.Ok)
                {
                    return Operation.Error(weight.Error);
                }

                var positionKpi = PositionKpi.Create(
                    kpi,
                    model.OrderNumber,
                    weight.Result,
                    model.ValidFrom,
                    model.ValidTo);
                if (!positionKpi.Ok)
                {
                    return Operation.Error(positionKpi.Error);
                }

                var assignKpiResult = position.AssignKpi(positionKpi.Result);
                if (!assignKpiResult.Ok)
                {
                    return Operation.Error(assignKpiResult.Error);
                }

                return true;
            }
        }
    }
}
