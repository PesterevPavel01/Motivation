using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.KpiMessages;

public class GetAllKpis
{
    public record Request() : IRequest<Operation<IEnumerable<KpiDto>, string>>;

    public class Handler(KpiApiClient client) : IRequestHandler<Request, Operation<IEnumerable<KpiDto>, string>>
    {
        public async ValueTask<Operation<IEnumerable<KpiDto>, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await client.GetAllAsync(cancellationToken);
            if (!result.Ok)
                return Operation.Error(result.Error);

            if (result.Result is null)
                return new List<KpiDto>();

            return result.Result.ToList();
        }
    }
}