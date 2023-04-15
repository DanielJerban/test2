namespace BPMS.Domain.Common.ViewModels;

public class WorkFlowConfermentAuthorityViewModel
{
    public Guid Id { get; set; }
    public Guid RequestTypeId { get; set; }
    public string RequestTypeTitle { get; set; }
    public bool IsDisplay { get; set; }
    public int RegisterDate { get; set; }
    public Guid ConfermAuthorityId { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public string FullName { get; set; }
}