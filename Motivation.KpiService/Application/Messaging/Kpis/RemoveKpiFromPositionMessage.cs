using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Domain.Entities;
using Motivation.Domain.Entities.Kpis;
using Motivation.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Motivation.KpiService.Application.Messaging.Kpis;

public class RemoveKpiFromPositionMessage
{ 
    public record Request(string PositionCode, string KpiCode) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request request, CancellationToken cancellationToken)
        {

            if (String.IsNullOrWhiteSpace(request.KpiCode))
            {
                return Operation.Error("Kpi code is null or white space!");
            }

            if (String.IsNullOrWhiteSpace(request.PositionCode))
            {
                return Operation.Error("Position code is null or white space!");
            }

            var kpiCode = CodeValue.Create(request.KpiCode);
            if (!kpiCode.Ok)
            {
                return Operation.Error(kpiCode.Error);
            }

            var positionCode = CodeValue.Create(request.PositionCode);
            if (!positionCode.Ok)
            {
                return Operation.Error(positionCode.Error);
            }

            var position = await unitOfWork.GetRepository<Position>()
                .GetFirstOrDefaultAsync(
                predicate: x => x.Code == positionCode.Result,
                include: query => query
                    .Include(x => x.PositionKpiHistory)
                        .ThenInclude(x => x.Position)
                    .Include(x => x.PositionKpiHistory)
                        .ThenInclude(x => x.Kpi),
                trackingType: TrackingType.Tracking);
            if (position is null)
            {
                return Operation.Error("Position is not found!");
            }

            var removeKpiPosition = position.PositionKpiHistory
                .FirstOrDefault(x => x.Kpi.Code == kpiCode.Result);

            if (removeKpiPosition is null)
            {
                return Operation.Error("KpiPosition not found!");
            }

            var removeResult = position.RemoveKpi(removeKpiPosition);
            if (!removeResult.Ok)
            {
                return Operation.Error(removeResult.Error);
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