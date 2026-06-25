using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.TimeSheetMessages;

public class RemoveActualHours
{
    public record Request(string EmployeeCode, RemoveActualHoursDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(EmployeeApiClient client) : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await client.RemoveActualHoursAsync(request.EmployeeCode, request.Model, cancellationToken);
            if (!result.Ok)
                return Operation.Error(result.Error);
            return true;
        }
    }
}
