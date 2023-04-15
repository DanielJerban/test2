using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.Dtos;

public class PatternDto
{
    public List<PatternItemDto> Items { get; set; }
    [Display(Name = "نام الگو")]
    public string Title { get; set; }
    public Guid PatternId { get; set; }
}