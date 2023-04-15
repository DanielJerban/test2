namespace BPMS.Domain.Common.Dtos.Jira;

public class SearchDTO
{
    public int total { get; set; }
    public List<IssuesDTO> issues { get; set; }
}