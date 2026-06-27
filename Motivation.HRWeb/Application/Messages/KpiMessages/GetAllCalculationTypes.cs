using Calabonga.OperationResults;
using Mediator;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.KpiMessages;

public class GetAllCalculationTypes
{
    public record Request() : IRequest<Operation<IEnumerable<string>, string>>;

    public class Handler(KpiApiClient client) : IRequestHandler<Request, Operation<IEnumerable<string>, string>>
    {
        public async ValueTask<Operation<IEnumerable<string>, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await client.GetAllCalculationTypesAsync(cancellationToken);
            if (!result.Ok)
                return Operation.Error(result.Error);

            if (result.Result is null)
                return new List<string>();

            return result.Result.ToList();
        }
    }
}
