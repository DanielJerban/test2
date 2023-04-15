using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class Schedule
{
    public Schedule()
    {
        Id = Guid.NewGuid();
    }
    [Key]
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public Guid TaskTypeId { get; set; }
    public bool IsActive { get; set; }
    public int StartDate { get; set; }
    public int EndDate { get; set; }
    [MaxLength(4)]
    public string? RunTime { get; set; }
    public bool IsDaily { get; set; }
    public bool SaturDay { get; set; }
    public bool SunDay { get; set; }
    public bool MonDay { get; set; }
    public bool TuesDay { get; set; }
    public bool WednesDay { get; set; }
    public bool ThursDay { get; set; }
    public bool Friday { get; set; }
    public bool IsRunExpireTrigger { get; set; }
    public byte DailyInterval { get; set; }
    [MaxLength(4)]
    public string? HourlyInterval { get; set; }
    public int RegisterDate { get; set; }
    public byte[]? Content { get; set; }

    //Navigation Property
    public virtual LookUp TaskType { get; set; }
}