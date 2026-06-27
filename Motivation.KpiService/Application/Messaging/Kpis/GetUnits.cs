using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Application.Mappings;

namespace Motivation.KpiService.Application.Messaging.Kpis;

public class GetUnits
{
    public record Request() : IRequest<Operation<IEnumerable<MeasurementUnitDto>, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<IEnumerable<MeasurementUnitDto>, string>>
    {
        public async ValueTask<Operation<IEnumerable<MeasurementUnitDto>, string>> Handle(Request kpiRequest, CancellationToken cancellationToken)
        {
            var unitResult = await unitOfWork.GetRepository<MeasurementUnit>()
                .GetAllAsync(
                    trackingType: TrackingType.NoTracking);

            return unitResult.Select(x => x.MapToDto()).ToArray();
        }
    }
}
