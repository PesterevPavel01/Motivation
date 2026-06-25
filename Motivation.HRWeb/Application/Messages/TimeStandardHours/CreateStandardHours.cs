using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.TimeStandardHours;

public class CreateStandardHours
{
    public record Request(StandardHoursCreateDto Model) : IRequest<Operation<StandardHoursDto, string>>;

    public class Handler(StandardHoursApiClient client) : IRequestHandler<Request, Operation<StandardHoursDto, string>>
    {
        public async ValueTask<Operation<StandardHoursDto, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await client.CreateAsync(request.Model, cancellationToken);
            if (!result.Ok)
                return Operation.Error(result.Error);
            return result.Result!;
        }
    }
}
