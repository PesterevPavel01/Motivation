using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;

namespace Motivation.SynchronisationService.Web.Application.Messaging.KpiMessages.Queries;

public sealed class GetKpis
{
    public record Request() : IRequest<Operation<IEnumerable<KpiDto>, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<IEnumerable<KpiDto>, string>>
    {
        public async ValueTask<Operation<IEnumerable<KpiDto>, string>> Handle(Request kpiRequest, CancellationToken cancellationToken)
        {
            logger.LogDebug("Get all kpi.");

            var kpiResult = await unitOfWork.GetRepository<Kpi>()
                .GetAllAsync(
                    trackingType: TrackingType.NoTracking,
                    include: query => query
                        .Include(x => x.Filters)
                        .Include(x => x.MeasurementUnit));

            return kpiResult.Select(x => x.MapToDto()).ToArray();
        }
    }
}
