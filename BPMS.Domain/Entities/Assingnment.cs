namespace BPMS.Domain.Entities;

public class Assingnment
{
    public Assingnment()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public Guid ResponseTypeGroupId { get; set; }

    public virtual Staff Staff { get; set; }
    public virtual LookUp ResponseTypeGroup { get; set; }
}