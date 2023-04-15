namespace BPMS.BuildingBlocks.Domain;

public class CorrelationId : ValueObject
{
    private readonly Guid value;
    private CorrelationId()
    {
        value = Guid.NewGuid();
    }
    private CorrelationId(Guid value)
    {
        this.value = value;
    }
    public Guid Value => value;
    public static CorrelationId New() => new CorrelationId();

    public static implicit operator CorrelationId(Guid value) => new CorrelationId(value);
}