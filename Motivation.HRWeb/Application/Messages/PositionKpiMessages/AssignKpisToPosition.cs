using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.PositionKpiMessages;

public class AssignKpisToPosition
{
    public record Request(String PositionCode, IEnumerable<AssignKpiToPositionDto> Models) : IRequest<Operation<bool, string>>;

    public class Handler(KpiApiClient client) : IRequestHandler<Request, Operation<bool, string>>
    {
        public async ValueTask<Operation<bool, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await client.CreatePositionKpiAsync(request.PositionCode, request.Models, cancellationToken);
            if (!result.Ok)
                return Operation.Error(result.Error);
            return result.Result;
        }
    }
}
