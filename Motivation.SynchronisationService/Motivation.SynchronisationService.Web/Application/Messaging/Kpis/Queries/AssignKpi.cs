using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.SynchronisationService.Web.Application.Messaging.Kpis.Queries;

public class AssignKpi
{
    public record Request(AssignKpiToPositionDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request positionRequest, CancellationToken cancellationToken)
        {
            var positionCode = CodeValue.Create(positionRequest.Model.PositionCode);
            if (!positionCode.Ok)
            {
                return Operation.Error(positionCode.Error);
            }

            var kpiCode = CodeValue.Create(positionRequest.Model.KpiCode);
            if (!kpiCode.Ok)
            {
                return Operation.Error(kpiCode.Error);
            }

            var positionRepository = unitOfWork.GetRepository<Position>();
            var position = await positionRepository.GetFirstOrDefaultAsync(
                predicate: x => x.Code == positionCode.Result,
                trackingType: TrackingType.Tracking);
            if (position is null)
            { 
                return Operation.Error("Position not found!");
            }

            var kpiRepository = unitOfWork.GetRepository<Kpi>();
            var kpi = await kpiRepository.GetFirstOrDefaultAsync(
                predicate: x => x.Code == kpiCode.Result,
                trackingType: TrackingType.Tracking);
            if (kpi is null)
            {
                return Operation.Error("Kpi not found!");
            }

            var weight = WeightValue.Create(positionRequest.Model.Weight);
            if (!weight.Ok)
            {
                return Operation.Error(weight.Error);
            }

            var positionKpi = PositionKpi.Create(
                kpi,
                1,
                weight.Result,
                positionRequest.Model.ValidFrom,
                positionRequest.Model.ValidTo);
            if (!positionKpi.Ok)
            {
                return Operation.Error(positionKpi.Error);
            }

            var assignKpiResult = position.AssignKpi(positionKpi.Result);
            if (!assignKpiResult.Ok)
            {
                return Operation.Error(assignKpiResult.Error);
            }

            var result = await unitOfWork.SaveChangesAsync();
            if (unitOfWork.Result.Exception is not null)
            {
                return Operation.Error(unitOfWork.Result.Exception.Message);
            }

            return true;
        }
    }
}
