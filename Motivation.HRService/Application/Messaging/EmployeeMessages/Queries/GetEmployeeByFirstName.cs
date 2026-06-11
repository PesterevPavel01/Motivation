using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Motivation.Application.Mappings;
using Motivation.Contracts.Dto;
using Motivation.Domain.Entities;
using Motivation.Domain.ValueObjects;

namespace Motivation.HRService.Application.Messaging.EmployeeMessages.Queries
{
    public class GetEmployeeByFirstName
    {
        public record Request(string FirstName) : IRequest<Operation<IEnumerable<EmployeeDto>, string>>;

        public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
            : IRequestHandler<Request, Operation<IEnumerable<EmployeeDto>, string>>
        {
            public async ValueTask<Operation<IEnumerable<EmployeeDto>, string>> Handle(Request employeeRequest, CancellationToken cancellationToken)
            {
                logger.LogDebug("Get Employee");

                var firstNameResult = FirstNameValue.Create(employeeRequest.FirstName);

                if (!firstNameResult.Ok)
                {
                    return Operation.Error("Invalid first name!");
                }

                var employeesResult = await unitOfWork.GetRepository<Employee>()
                    .GetAllAsync(
                        predicate: x => x.FirstName == firstNameResult.Result,
                        include: query => query
                            .Include(x => x.EmployeePositions)
                                .ThenInclude(x => x.Position)
                            .Include(x => x.EmployeePositions)
                                .ThenInclude(x => x.ExtraPartHistory)
                            .Include(x => x.EmployeePositions)
                                .ThenInclude(x => x.DeductionHistory));

                return employeesResult.Select(x => x.MapToDto()).ToArray();
            }
        }
    }
}
