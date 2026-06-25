using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.TimeStandardHours;

public class UpdateStandardHours
{
    public record Request(Guid Id, StandardHoursUpdateDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(StandardHoursApiClient client) : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await client.UpdateAsync(request.Id, request.Model, cancellationToken);
            if (!result.Ok)
                return Operation.Error(result.Error);
            return result.Result;
        }
    }
}