using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.PositionMessages;

public class CreatePosition
{
    public record Request(PositionCreateDto Model) : IRequest<Operation<bool, string>>;

    public class Handler(PositionApiClient client) : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await client.CreateAsync(request.Model, cancellationToken);
            if (!result.Ok)
                return Operation.Error(result.Error);
            return true;
        }
    }
}
