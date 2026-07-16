using Calabonga.OperationResults;
using Mediator;
using Motivation.AnaliticsCore.SyncService.Application.Interfaces;
using Motivation.AnalyticsCore.Contracts.Models;

namespace Motivation.AnaliticsCore.SyncService.Application.Messaging
{
    public class SyncSourceData
    {
        public record Request(SyncDto SyncModel) : IRequest<Operation<bool, string>>;

        public class Handler(
            IExecutor<SyncDto> syncExecutor)
            : IRequestHandler<Request, Operation<bool, string>>
        {
            public async ValueTask<Operation<bool, string>> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    return await syncExecutor.ExecuteAsync(request.SyncModel, default);
                }
                catch (Exception exception)
                {
                    return Operation.Error(exception.Message);
                }
            }
        }
    }
}
