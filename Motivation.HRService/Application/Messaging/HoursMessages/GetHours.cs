using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;

namespace Motivation.HRService.Application.Messaging.HoursMessages;

public class GetHours
{
    public record Request() : IRequest<Operation<IEnumerable<StandardHoursDto>, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
     : IRequestHandler<Request, Operation<IEnumerable<StandardHoursDto>, string>>
    {
        public async ValueTask<Operation<IEnumerable<StandardHoursDto>, string>> Handle(Request hoursRequest, CancellationToken cancellationToken)
        {
            logger.LogDebug("Get all standard hours.");

            var standardHours = await unitOfWork.GetRepository<StandardHours>()
                .GetAllAsync(trackingType: TrackingType.NoTracking);

            return standardHours.Select(x => x.MapToDto()).ToArray();
        }
    }
}
