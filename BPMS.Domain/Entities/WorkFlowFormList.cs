using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class WorkFlowFormList
{
    public WorkFlowFormList()
    {
        Id = Guid.NewGuid();
    }

    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public byte[] Content { get; set; }
}