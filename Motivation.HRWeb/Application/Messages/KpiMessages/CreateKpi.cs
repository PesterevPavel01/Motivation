using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.KpiMessages;

public class CreateKpi
{
    public record Request(KpiCreateDto Model) : IRequest<Operation<KpiDto, string>>;

    public class Handler(KpiApiClient client) : IRequestHandler<Request, Operation<KpiDto, string>>
    {
        public async ValueTask<Operation<KpiDto, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await client.CreateAsync(request.Model, cancellationToken);
            if (!result.Ok)
                return Operation.Error(result.Error);
            if (result.Result is null)
                return Operation.Error("Creation result is null!");
            return result.Result;
        }
    }
}
