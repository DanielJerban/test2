using BPMS.BuildingBlocks.Domain.Events;
using MediatR;

namespace BPMS.BuildingBlocks.Application.Configuration.DomainEvents;

public interface IDomainEventHandler
{
}

public interface IDomainEventHandler<TDomainEvent> : IDomainEventHandler, INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
}