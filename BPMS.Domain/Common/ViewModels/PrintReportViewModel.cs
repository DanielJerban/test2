namespace BPMS.Domain.Common.ViewModels;

public class PrintReportViewModel
{
    public string Data { get; set; }
    public Guid ReportId { get; set; }
    public Entities.Report Report { get; set; }

    //for bpmn:
    public string FileName { get; set; }
}