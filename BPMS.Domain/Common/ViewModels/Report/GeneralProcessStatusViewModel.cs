namespace BPMS.Domain.Common.ViewModels.Report;

public class GeneralProcessStatusViewModel
{
    public string ProcessName { get; set; }
    public int? AvgPredictedTime { get; set; }
    public int? AvgTimeToDo { get; set; }
    public int? AvgTimeDone { get; set; }
    public int? Min { get; set; }
    public int? Max { get; set; }
    public int Count { get; set; }
    public Guid Id { get; set; }
}