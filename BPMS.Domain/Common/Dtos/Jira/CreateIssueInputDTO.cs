using System.Collections.Specialized;

namespace BPMS.Domain.Common.Dtos.Jira;

public class CreateIssueInputDTO
{
    public NameValueCollection forms { get; set; }
    public string userName { get; set; }
}