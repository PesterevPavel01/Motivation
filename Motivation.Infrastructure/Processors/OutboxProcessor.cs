using Calabonga.UnitOfWork;
using Mediator;
using Microsoft.Extensions.Options;
using Motivation.Contracts.Interfaces;
using Motivation.Domain.Entities;
using Motivation.Domain.Interfaces;
using Motivation.Infrastructure.Options;
using System.Data;
using System.Text.Json;

namespace Motivation.Infrastructure.Processors;

public sealed class OutboxProcessor : IOutboxProcessor
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;
    private readonly TimeProvider _timerProvider;
    private readonly IEnumerable<string> _supportedEventTypes;

    public OutboxProcessor(
        IUnitOfWork unitOfWork,
        IPublisher publisher,
        TimeProvider timerProvider,
        IOptions<OutboxProcessorOptions> options)
    {
        _unitOfWork = unitOfWork;
        _publisher = publisher;
        _timerProvider = timerProvider;
        _supportedEventTypes = options.Value.SupportedEventTypes;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        var pagedList = await _unitOfWork.GetRepository<OutboxMessage>()
            .GetPagedListAsync(
                predicate: x => 
                    x.ProcessedAt == null
                    && _supportedEventTypes.Contains(x.Type),
                pageIndex: 0,
                pageSize: 20,
                orderBy: o => o.OrderBy(x => x.CreatedAt),
                trackingType: TrackingType.Tracking,
                cancellationToken: cancellationToken
            );

        foreach (var message in pagedList.Items)
        {
            var domainEvent = JsonSerializer.Deserialize<IDomainEvent>(message.Content, OutboxMessage.JsonSettings);
            
            if (domainEvent is null)
            {
                //await _telegramService.SendMessageAsync($"{"OrderService".ToUpper()} Event {message.GetType().Name}. DomainEvent not found!");
                continue;
            }

            await _publisher.Publish(domainEvent, cancellationToken);

            message.ProcessedAt = _timerProvider.GetLocalNow().DateTime;

            await _unitOfWork.SaveChangesAsync();

            if (!_unitOfWork.Result.Ok)
            {
                //await _telegramService.SendMessageAsync($"{"OrderService".ToUpper()}.{typeof(OutboxProcessor).Name}. Event: {message.GetType().Name}. {_unitOfWork.Result.Exception}");
                return;
            }
        }
    }
}