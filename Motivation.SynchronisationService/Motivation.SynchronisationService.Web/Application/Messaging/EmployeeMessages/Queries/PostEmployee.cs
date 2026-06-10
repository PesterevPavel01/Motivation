using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.SynchronisationService.Web.Application.Messaging.EmployeeMessages.Queries;

public class PostEmployee
{
    public record Request(EmployeeCreateDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request employeeRequest, CancellationToken cancellationToken)
        {
            logger.LogDebug("Creating new Employee");

            var codeValueResult = CodeValue.Create(employeeRequest.Model.Code);

            if (!codeValueResult.Ok)
            {
                return Operation.Error(codeValueResult.Error);
            }

            var firstNameValueResult = FirstNameValue.Create(employeeRequest.Model.FirstName);

            if (!firstNameValueResult.Ok)
            {
                return Operation.Error(firstNameValueResult.Error);
            }
            
            var lastNameValueResult = LastNameValue.Create(employeeRequest.Model.LastName);

            if (!lastNameValueResult.Ok)
            {
                return Operation.Error(lastNameValueResult.Error);
            }

            var fullNameValueResult = FullNameValue.Create(employeeRequest.Model.FullName);

            if (!fullNameValueResult.Ok)
            {
                return Operation.Error(fullNameValueResult.Error);
            }

            SecondNameValue? secondNameValue = null;

            if (employeeRequest.Model.SecondName is not null)
            {
                var secondNameResult = SecondNameValue.Create(employeeRequest.Model.SecondName);
                if (!secondNameResult.Ok)
                {
                    return Operation.Error(fullNameValueResult.Error);
                }
                secondNameValue = secondNameResult.Result;
            }

            var entityResult = Employee.Create(
                code: codeValueResult.Result,
                firstName: firstNameValueResult.Result,
                lastName: lastNameValueResult.Result,
                secondName: secondNameValue,
                fullName: fullNameValueResult.Result);

            if (!entityResult.Ok)
            {
                logger.LogError(entityResult.Error);
                return Operation.Error(entityResult.Error);
            }

            await unitOfWork.GetRepository<Employee>().InsertAsync(entityResult.Result, cancellationToken);
            var result = await unitOfWork.SaveChangesAsync();

            if (unitOfWork.Result.Exception is not null)
            {
                return Operation.Error(unitOfWork.Result.Exception.Message);
            }

            return true;
        }
    }
}
