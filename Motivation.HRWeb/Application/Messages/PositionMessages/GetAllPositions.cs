using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.PositionMessages;

public class GetAllPositions
{
    public record Request() : IRequest<Operation<IEnumerable<PositionDto>, string>>;

    public class Handler(PositionApiClient client) : IRequestHandler<Request, Operation<IEnumerable<PositionDto>, string>>
    {
        public async ValueTask<Operation<IEnumerable<PositionDto>, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await client.GetAllAsync(cancellationToken);
            if (!result.Ok)
                return Operation.Error(result.Error);

            if (result.Result is null)
                return new List<PositionDto>();

            return result.Result.ToList();
        }
    }
}
