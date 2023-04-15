using BPMS.BuildingBlocks.Domain.Events;
using Serilog.Core;
using Serilog.Events;

namespace BPMS.BuildingBlocks.Application.Logging.Enrichers;

public class DomainEventLogEnricher : ILogEventEnricher
{
    private readonly IDomainEvent _domainEvent;

    public DomainEventLogEnricher(IDomainEvent domainEvent)
    {
        _domainEvent = domainEvent;
    }
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddOrUpdateProperty(new LogEventProperty("Context", new ScalarValue($"DomainEvent-{_domainEvent.GetType()}")));
        logEvent.AddOrUpdateProperty(new LogEventProperty("DomainEventId", new ScalarValue(_domainEvent.Id)));
        logEvent.AddOrUpdateProperty(new LogEventProperty(nameof(IDomainEvent.CorrelationId), new ScalarValue(_domainEvent.CorrelationId)));
    }
}