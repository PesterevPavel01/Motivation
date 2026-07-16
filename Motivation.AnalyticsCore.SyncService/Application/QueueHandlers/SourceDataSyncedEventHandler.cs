using Motivation.Contracts.QueueEvents;
using Rebus.Handlers;

namespace Motivation.AnaliticsCore.SyncService.Application.QueueHandlers
{
    public class SourceDataSyncedEventHandler : IHandleMessages<SourceDataSyncedQueueEvent>
    {
        public async Task Handle(SourceDataSyncedQueueEvent message)
        {
            var eventMessage = "New event";
        }
    }
}
