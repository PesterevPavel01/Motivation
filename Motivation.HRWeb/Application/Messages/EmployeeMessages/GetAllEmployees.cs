using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.EmployeeMessages;

public class GetAllEmployees
{
    public record Request() : IRequest<Operation<IEnumerable<EmployeeDto>, string>>;

    public class Handler(EmployeeApiClient client) : IRequestHandler<Request, Operation<IEnumerable<EmployeeDto>, string>>
    {
        public async ValueTask<Operation<IEnumerable<EmployeeDto>, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await client.GetAllAsync(cancellationToken);
            if (!result.Ok)
                return Operation.Error(result.Error);
            
            if (result.Result is null)
                return new List<EmployeeDto>();

            return result.Result.ToList();
        }
    }
}