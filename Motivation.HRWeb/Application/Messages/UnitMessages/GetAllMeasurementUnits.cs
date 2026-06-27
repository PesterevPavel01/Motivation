using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.UnitMessages;

public class GetAllMeasurementUnits
{
    public record Request() : IRequest<Operation<List<MeasurementUnitDto>?, string>>;

    public class Handler(MeasurementUnitApiClient client) : IRequestHandler<Request, Operation<List<MeasurementUnitDto>?, string>>
    {
        public async ValueTask<Operation<List<MeasurementUnitDto>?, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await client.GetAllAsync(cancellationToken);
            if (!result.Ok)
                return Operation.Error(result.Error);
            return result.Result;
        }
    }
}
