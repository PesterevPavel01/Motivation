using Calabonga.UnitOfWork;
using Microsoft.Extensions.Options;
using Motivation.Contracts.Interfaces;
using Motivation.Domain.Entities;
using Motivation.Infrastructure.Options;

namespace Motivation.Infrastructure.Processors;

public sealed class OutboxCleanerProcessor : IOutboxCleanerProcessor
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timerProvider;
    private readonly IEnumerable<string> _supportedEventTypes;

    public OutboxCleanerProcessor(
        IUnitOfWork unitOfWork, 
        TimeProvider timerProvider,
        IOptions<OutboxProcessorOptions> options)
    {
        _unitOfWork = unitOfWork;
        _timerProvider = timerProvider;
        _supportedEventTypes = options.Value.SupportedEventTypes;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        var outboxMessageRepository = _unitOfWork
            .GetRepository<OutboxMessage>();

        var removeOutboxMessages = await outboxMessageRepository
            .GetAllAsync(
                predicate: x => 
                    x.ProcessedAt < _timerProvider.GetUtcNow().AddDays(-10)
                    && _supportedEventTypes.Contains(x.Type),
                trackingType: TrackingType.Tracking);

        outboxMessageRepository.Delete(removeOutboxMessages);

        await _unitOfWork.SaveChangesAsync();

        if (!_unitOfWork.Result.Ok)
        {
            //await _telegramService.SendMessageAsync($"{"OrderService".ToUpper()}.{typeof(OutboxCleanerProcessor).Name}. {_unitOfWork.Result.Exception}");
            return;
        }
    }
}
