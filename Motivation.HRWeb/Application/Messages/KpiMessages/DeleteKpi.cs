using Calabonga.OperationResults;
using Mediator;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.KpiMessages;

public class DeleteKpi
{
    public record Request(string Code) : IRequest<Operation<bool, string>>;

    public class Handler(KpiApiClient client) : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await client.DeleteAsync(request.Code, cancellationToken);
            if (!result.Ok)
                return Operation.Error(result.Error);
            return true;
        }
    }
}
