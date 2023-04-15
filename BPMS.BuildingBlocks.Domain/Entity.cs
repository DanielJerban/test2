using BPMS.BuildingBlocks.Domain.BusinessRule;
using BPMS.BuildingBlocks.Domain.Events;
using BPMS.BuildingBlocks.SharedKernel;

namespace BPMS.BuildingBlocks.Domain;

public abstract class Entity
{
    public Guid StreamId { get; set; }

    private HashSet<IDomainEvent>? _domainEvents;
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents != null ? _domainEvents : Enumerable.Empty<IDomainEvent>().ToList().AsReadOnly();
    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        if (_domainEvents == null)
            _domainEvents = new HashSet<IDomainEvent>();

        _domainEvents.Add(domainEvent);
    }
    protected void CheckBusinessRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }
    public static string Info<T>() => typeof(T).ToJson();
}