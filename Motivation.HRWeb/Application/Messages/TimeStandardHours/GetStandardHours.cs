using Calabonga.OperationResults;
using Mediator;
using Motivation.Contracts.Dto;
using Motivation.HRWeb.Application.Clients;

namespace Motivation.HRWeb.Application.Messages.TimeStandardHours;

public class GetStandardHours
{
    public record Request(int Month, int Year, string WorkWeekType) : IRequest<Operation<StandardHoursDto, string>>;

    public class Handler(StandardHoursApiClient client) : IRequestHandler<Request, Operation<StandardHoursDto, string>>
    {
        public async ValueTask<Operation<StandardHoursDto, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await client.GetByMonthAsync(request.WorkWeekType, request.Month, request.Year, cancellationToken);
            if (!result.Ok)
                return Operation.Error(result.Error);
            if (result.Result is null)
                return Operation.Error("Standard hours not found!");
            return result.Result;
        }
    }
}