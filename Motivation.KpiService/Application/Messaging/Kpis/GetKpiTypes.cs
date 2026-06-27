using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using Mediator;
using Motivation.Application.Mappings;
using Motivation.Domain.Enum;

namespace Motivation.KpiService.Application.Messaging.Kpis;

public sealed class GetKpiTypes
{
    public record Request() : IRequest<Operation<IEnumerable<string>, string>>;

    public class Handler(IUnitOfWork unitOfWork, ILogger<Handler> logger)
        : IRequestHandler<Request, Operation<IEnumerable<string>, string>>
    {
        public async ValueTask<Operation<IEnumerable<string>, string>> Handle(Request kpiRequest, CancellationToken cancellationToken)
        {
            var kpiTypes = Enum.GetValues<KpiType>()
                .Select(x => x.ToRussianString());

            return kpiTypes.ToArray();
        }
    }
}
