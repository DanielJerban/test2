using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class WorkFlowIndicator
{
    public WorkFlowIndicator()
    {
        Id = Guid.NewGuid();
    }
    [Key]
    public Guid Id { get; set; }
    public Guid? WidgetTypeId { get; set; }
    public Guid RequestTypeId { get; set; }
    public Guid ActivityId { get; set; }
    public Guid DurationId { get; set; }
    public Guid FlowstatusId { get; set; }
    public Guid CalcCriterionId { get; set; }
    public int Warning { get; set; }
    public int Crisis { get; set; }
    public int RegisterDate { get; set; }
    [MaxLength(4)]
    public string? RegisterTime { get; set; }

    //Navigation Property
    public virtual LookUp RequestType { get; set; }
    public virtual LookUp Duration { get; set; }
    public virtual LookUp Flowstatus { get; set; }
    public virtual LookUp CalcCriterion { get; set; }
    public virtual LookUp? WidgetType { get; set; }
    public virtual WorkFlowDetail WorkFlowDetail { get; set; }
}