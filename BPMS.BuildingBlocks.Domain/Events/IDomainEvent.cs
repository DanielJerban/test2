using MediatR;

namespace BPMS.BuildingBlocks.Domain.Events;

public interface IDomainEvent: INotification
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
    CorrelationId CorrelationId { get; }
    void SetCorrelationId(CorrelationId correlationId);
}