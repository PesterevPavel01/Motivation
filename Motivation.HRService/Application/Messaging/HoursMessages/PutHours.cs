using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.HRService.Application.Messaging.HoursMessages;

public class PutHours
{
    public record Request(Guid Id, StandardHoursUpdateDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request hoursRequest, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<StandardHours>();
            var entity = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id == hoursRequest.Id, trackingType: TrackingType.Tracking);
            if (entity == null)
            {
                return Operation.Error("Standard hours not found!");
            }

            var standardHoursValue = HoursValue.Create(hoursRequest.Model.Value);
            if (!standardHoursValue.Ok)
            {
                return Operation.Error(standardHoursValue.Error);
            }

            var updateResult = entity.Update(standardHoursValue.Result);
            if (!updateResult.Ok)
                return Operation.Error(updateResult.Error);

            repository.Update(entity);
            var result = await unitOfWork.SaveChangesAsync();

            if (unitOfWork.Result.Exception is not null)
            {
                return Operation.Error(unitOfWork.Result.Exception.Message);
            }

            return true;
        }
    }
}
