using BPMS.BuildingBlocks.Application.Logging.Enrichers;
using BPMS.BuildingBlocks.Domain.Events;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace BPMS.BuildingBlocks.Application.Configuration.DomainEvents;

public abstract class DomainEventHandlerBase<TDomainEvent> : IDomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    protected ILogger Logger { get; }

    public DomainEventHandlerBase(ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger(GetType());
    }
    public async Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        using (LogContext.Push(new DomainEventLogEnricher(domainEvent)))
        {
            try
            {
                await Batch(domainEvent, cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"DomainEventHandler({GetType()}): Unexpected error: {exception.Message}");
                throw;
            }
        }
    }
    protected abstract Task Batch(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}