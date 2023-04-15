namespace BPMS.Domain.Common.Dtos;

public class WorkFlowRequestsDetailByRequestTypeDto
{
    public string FirstRequestDateTime { get; set; }
    public string LastRequestDateTime { get; set; }
    public int FinishedRejectedRequestsCount { get; set; }
    public int FinishedAcceptedRequestsCount { get; set; }
    public int InProgressRequestsCount { get; set; }
    public int NotStartedRequestsCount { get; set; }
    public int TotalRequestsCount { get; set; }
}