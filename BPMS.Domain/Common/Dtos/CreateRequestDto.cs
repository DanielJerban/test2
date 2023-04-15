using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.Dtos;

public class CreateRequestDto
{
    [Required]
    public Guid WorkflowExternalId { get; set; }
    public string FormData { get; set; }
    public string PersonalCode { get; set; }
    public string GridList { get; set; }
}