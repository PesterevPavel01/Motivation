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
    public class GetEmployees
    {
        public record Request() : IRequest<Operation<IEnumerable<EmployeeDto>, string>>;

        public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
            : IRequestHandler<Request, Operation<IEnumerable<EmployeeDto>, string>>
        {
            public async ValueTask<Operation<IEnumerable<EmployeeDto>, string>> Handle(Request employeeRequest, CancellationToken cancellationToken)
            {
                logger.LogDebug("Get Employees");

                var employeesResult = await unitOfWork.GetRepository<Employee>()
                    .GetAllAsync(
                        predicate: x => x.Enabled == true,
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
