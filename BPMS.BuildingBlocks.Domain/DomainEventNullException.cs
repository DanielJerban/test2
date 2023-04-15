using BPMS.BuildingBlocks.Domain.Events;

namespace BPMS.BuildingBlocks.Domain;

public interface IDomainEventNullException
{
}

public class DomainEventNullException<TDomainEvent> : Exception, IDomainEventNullException where TDomainEvent : IDomainEvent
{
    public DomainEventNullException() : this(string.Empty)
    {
    }

    public DomainEventNullException(string message)
        : base($"DomainEvent with type {typeof(TDomainEvent).FullName} has passed null{(!string.IsNullOrEmpty(message) ? $" with message: {message}" : string.Empty)}")
    {
    }
}