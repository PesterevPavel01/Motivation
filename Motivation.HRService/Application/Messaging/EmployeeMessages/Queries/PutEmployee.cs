using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.HRService.Application.Messaging.EmployeeMessages.Queries;

public class PutEmployee
{
    public record Request(string Code, EmployeeUpdateDto Model) : IRequest<Operation<bool, string>>;

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
            var entity = await repository.GetFirstOrDefaultAsync(predicate: x => x.Code == code.Result, trackingType: TrackingType.Tracking);
            if (entity == null)
            {
                return Operation.Error("Employee not found!");
            }

            var newCode = CodeValue.Create(employeeRequest.Model.Code);

            if (!newCode.Ok)
            {
                return Operation.Error(newCode.Error);
            }

            var firstName = FirstNameValue.Create(employeeRequest.Model.FirstName);

            if (!firstName.Ok)
            {
                return Operation.Error(firstName.Error);
            }

            var lastName = LastNameValue.Create(employeeRequest.Model.LastName);

            if (!lastName.Ok)
            {
                return Operation.Error(lastName.Error);
            }

            var fullName = FullNameValue.Create(employeeRequest.Model.FullName);

            if (!fullName.Ok)
            {
                return Operation.Error(fullName.Error);
            }

            SecondNameValue? secondNameValue = null;

            if (employeeRequest.Model.SecondName is not null)
            {
                var secondNameResult = SecondNameValue.Create(employeeRequest.Model.SecondName);
                if (!secondNameResult.Ok)
                {
                    return Operation.Error(fullName.Error);
                }
                secondNameValue = secondNameResult.Result;
            }

            var entityResult = entity.Update(
                code: newCode.Result,
                firstName: firstName.Result,
                lastName: lastName.Result,
                secondName: secondNameValue,
                fullName: fullName.Result);

            if (!entityResult.Ok)
            {
                logger.LogError(entityResult.Error);
                return Operation.Error(entityResult.Error);
            }

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
