using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class Report
{
    public Report()
    {
        Id = Guid.NewGuid();
        DynamicCharts = new HashSet<DynamicChart>();
    }

    [Key]
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public Guid CreatorId { get; set; }
    public Guid? WorkflowId { get; set; }
    public byte[]? Expersion { get; set; }
    public int RegisterDate { get; set; }
    public bool IsActive { get; set; }
    public string? PrintFileName { get; set; }
        
        
    //Navigation Property
    public virtual Staff Creator { get; set; }
    public virtual Workflow? Workflow { get; set; }
    public virtual ICollection<DynamicChart> DynamicCharts { get; set; }
}