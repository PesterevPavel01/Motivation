using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.HRService.Application.Messaging.EmployeeMessages.Queries;

public class DeleteEmployee
{
    public record Request(string Code) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request employeeRequest, CancellationToken cancellationToken)
        {
            var code = CodeValue.Create(employeeRequest.Code);

            if (!code.Ok)
            {
                return Operation.Error(code.Error);
            }

            var repository = unitOfWork.GetRepository<Employee>();
            var entity = await repository
                .GetFirstOrDefaultAsync(
                    predicate: x => x.Code == code.Result,
                    trackingType: TrackingType.Tracking);
            
            if (entity is null)
            {
                return Operation.Error("Employee not found!");
            }

            entity.Delete();

            repository.Delete(entity);
            var result = await unitOfWork.SaveChangesAsync();

            if (unitOfWork.Result.Exception is not null)
            {
                return Operation.Error(unitOfWork.Result.Exception.Message);
            }

            return true;
        }
    }
}
