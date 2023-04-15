using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class FormClassificationRelation
{
    [Key] 
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MainId { get; set; }
    public Guid SecondaryId { get; set; }
}