namespace BPMS.Domain.Common.ViewModels;

public class WorkFlowDetailViewModel
{
    public Guid Id { get; set; }
    public Guid FlowId { get; set; }
    public Guid RequestId { get; set; }
    public string Title { get; set; }
    public int ReceiveDate { get; set; }
    public int? ResponseDate { get; set; }
    public string ResponseTime { get; set; }
    public string FullName { get; set; }
    public long RequestNo { get; set; }
    public string FlowStatus { get; set; }
    public string Building { get; set; }
    public string Phone { get; set; }


    public int Answer { get; set; }
    public int Sediment { get; set; }
    public int? WaitingTime { get; set; }


    public string RequestType { get; set; }
}