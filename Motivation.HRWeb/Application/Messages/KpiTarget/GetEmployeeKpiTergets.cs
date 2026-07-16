using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.KpiTarget;

public class GetEmployeeKpiTergets
{
    public record Request(string EmployeeCode) : IRequest<Operation<EmployeeDto, string>>;

    public class Handler(KpiApiClient client) : IRequestHandler<Request, Operation<EmployeeDto, string>>
    {
        public async ValueTask<Operation<EmployeeDto, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await client.GetEmployeeAsync(request.EmployeeCode, cancellationToken);
            if (!result.Ok)
                return Operation.Error(result.Error);

            if (result.Result is null)
                return Operation.Error("Employee not found!");

            return result.Result;
        }
    }
}
