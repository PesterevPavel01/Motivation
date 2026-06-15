using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.HRService.Application.Messaging.HoursMessages;

public class PostHours
{
    public record Request(StandardHoursCreateDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request hoursRequest, CancellationToken cancellationToken)
        {
            logger.LogDebug($"Create standard hours.");

            if (hoursRequest.Model is null)
                return Operation.Error("Standard hours model is null!");

            if (String.IsNullOrWhiteSpace(hoursRequest.Model.WorkWeekType))
            {
                return Operation.Error("Work week type code is null or white space!");
            }

            var workWeekType = WorkWeekTypeMapper.FromRussianString(hoursRequest.Model.WorkWeekType);
            if (!workWeekType.Ok)
            {
                return Operation.Error(workWeekType.Error);
            }

            var standardHoursValue = StandardHoursValue.Create(hoursRequest.Model.Value);
            if (!standardHoursValue.Ok)
            {
                return Operation.Error(standardHoursValue.Error);
            }

            var month = MonthValue.Create(hoursRequest.Model.Month);
            if (!month.Ok)
            {
                return Operation.Error(month.Error);
            }

            var year = YearValue.Create(hoursRequest.Model.Year);
            if (!year.Ok)
            {
                return Operation.Error(year.Error);
            }

            var standardHours = StandardHours.Create(standardHoursValue.Result, workWeekType.Result, month.Result, year.Result);
            if (!standardHours.Ok)
            {
                return Operation.Error(standardHours.Error);
            }

            await unitOfWork.GetRepository<StandardHours>().InsertAsync(standardHours.Result, cancellationToken);
            var result = await unitOfWork.SaveChangesAsync();

            if (unitOfWork.Result.Exception is not null)
            {
                return Operation.Error(unitOfWork.Result.Exception.Message);
            }

            return true;
        }
    }
}