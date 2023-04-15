namespace BPMS.BuildingBlocks.Domain.Events;

public class DomainEventBase : IDomainEvent
{
    public Guid Id { get; }

    public DateTime OccurredOn { get; }

    public CorrelationId CorrelationId { get; protected set; } = CorrelationId.New();

    public DomainEventBase()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }

    public void SetCorrelationId(CorrelationId correlationId) => CorrelationId = correlationId;
}

public class DomainEventBase<TAggregateRootId> : DomainEventBase
{
    public TAggregateRootId AggregateRootId { get; }

    public DomainEventBase(TAggregateRootId aggregateRootId) : base() => AggregateRootId = aggregateRootId;
}