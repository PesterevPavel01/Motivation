using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Domain.Entities;
using Motivation.IdentityServer.Domain.Base;

namespace Motivation.HRService.Application.Messaging.HoursMessages;

public class DeleteHours
{
    public record Request(Guid Id) : IRequest<Operation<bool, string>>;

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

            repository.Delete(entity);
            await unitOfWork.SaveChangesAsync();
            if (!unitOfWork.Result.Ok)
            {
                return Operation.Error(unitOfWork.Result.Exception?.Message ?? AppData.Exceptions.SomethingWrong);
            }

            return true;
        }
    }
}
