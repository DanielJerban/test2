namespace BPMS.Domain.Entities;

public class RoleMapChart
{
    public RoleMapChart()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; }

    public Guid ChartId { get; set; }
    public virtual Chart Chart { get; set; }
}