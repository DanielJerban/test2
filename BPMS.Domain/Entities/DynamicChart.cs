namespace BPMS.Domain.Entities;

public class DynamicChart
{
    public DynamicChart()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public Guid WidgetTypeId { get; set; }
    public Guid ReportId { get; set; }
    public Guid CreatorId { get; set; }
    public bool IsActive { get; set; }
    public string? DataSetting { get; set; }

    public virtual LookUp WidgetType { get; set; }
    public virtual Report Report { get; set; }
    public virtual Staff Creator { get; set; }
}