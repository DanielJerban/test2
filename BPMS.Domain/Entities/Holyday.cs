namespace BPMS.Domain.Entities;

public class Holyday
{
    public Holyday()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public int Date { get; set; }
    public string? Dsr { get; set; }
    public Guid HolydayTypeId { get; set; }

    public virtual LookUp HolydayType { get; set; }
}