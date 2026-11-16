using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.KpiTarget;

public class SetKpiResultMessage
{
    public record Request(SetEmployeeKpiResult Model) : IRequest<Operation<bool, string>>;

    public class Handler(KpiApiClient client) : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            return await client.SetKpiResultAsync(request.Model, cancellationToken);
        }
    }
}
