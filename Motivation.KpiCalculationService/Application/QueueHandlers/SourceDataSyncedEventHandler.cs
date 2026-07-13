using Motivation.Contracts.QueueEvents;
using Rebus.Handlers;

namespace Motivation.KpiCalculationService.Application.QueueHandlers;

public class SourceDataSyncedEventHandler : IHandleMessages<SourceDataSyncedEvent>
{
    public async Task Handle(SourceDataSyncedEvent message)
    {
        var eventMessage = "New event";
    }
}
