using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class FormLookUp2N
{
    public FormLookUp2N()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Type1 { get; set; }
    [Required]
    public string Type2 { get; set; }
    [Required]
    public string Title1 { get; set; }
    [Required]
    public string Title2 { get; set; }
}