using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Enums;

namespace BPMS.Domain.Entities;

public class ExternalApi
{
    public ExternalApi()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Url { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Token { get; set; }
    public string? Content { get; set; } // form data or json goes here
    public string? ResponseStructute { get; set; }
    public string? Headers { get; set; }
    public bool UseInGrid { get; set; }
    public ContentTypeEnum ContentType { get; set; }
    public ActionType ActionType { get; set; }
    public AuthorizationType AuthorizationType { get; set; }
    public ICollection<WorkFlowDetail> WorkFlowDetails { get; set; }
}