using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;

namespace Motivation.HRService.Application.Messaging.PositionMessages.Queries
{
    public sealed class GetPositions
    {
        public record Request() : IRequest<Operation<IEnumerable<PositionDto>, string>>;

        public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
            : IRequestHandler<Request, Operation<IEnumerable<PositionDto>, string>>
        {
            public async ValueTask<Operation<IEnumerable<PositionDto>, string>> Handle(Request positionRequest, CancellationToken cancellationToken)
            {
                logger.LogDebug("Get all positions.");

                var positionResult = await unitOfWork.GetRepository<Position>()
                    .GetAllAsync(
                        trackingType: TrackingType.NoTracking,
                        include: query => query
                            .Include(x=> x.PositionKpiHistory)
                                .ThenInclude(x => x.Kpi));

                return positionResult.Select(x => x.MapToDto()).ToArray();
            }
        }
    }
}
