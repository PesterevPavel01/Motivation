using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.Enum;
using Motivation.Domain.ValueObjects;

namespace Motivation.HRService.Application.Messaging.HoursMessages;

public class GetHours
{
    public record Request(string? WorkWeekType = null, int Month = 0, int Year = 0) : IRequest<Operation<IEnumerable<StandardHoursDto>, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
     : IRequestHandler<Request, Operation<IEnumerable<StandardHoursDto>, string>>
    {
        public async ValueTask<Operation<IEnumerable<StandardHoursDto>, string>> Handle(Request hoursRequest, CancellationToken cancellationToken)
        {
            logger.LogDebug("Get standard hours.");

            var repository =  unitOfWork.GetRepository<StandardHours>();

            IList<StandardHours> hours = [];

            if (hoursRequest.WorkWeekType is null)
            {
                hours = await repository
                    .GetAllAsync(
                        trackingType: TrackingType.NoTracking);

                return hours.Select(x => x.MapToDto()).ToArray();
            }

            var month = MonthValue.Create(hoursRequest.Month);
            if (!month)
                return Operation.Error(month.Error);

            var year = YearValue.Create(hoursRequest.Year);
            if (!month)
                return Operation.Error(year.Error);

            var workWeekType = WorkWeekTypeMapper.FromRussianString(hoursRequest.WorkWeekType);
            if(!workWeekType.Ok)
                return Operation.Error(workWeekType.Error);

            var hour = await repository
                .GetAllAsync(
                    predicate: x => x.Month == month.Result && x.Year == year.Result && x.WorkWeekType == workWeekType.Result,
                    trackingType: TrackingType.NoTracking);

            return hour.Select(x => x.MapToDto()).ToArray();


        }
    }
}
