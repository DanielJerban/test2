using System.ComponentModel;

namespace BPMS.Domain.Common.ViewModels;

public class RequestDelayExcelViewModel
{
    [DisplayName("اقدام کننده")]
    public string PersonalName { get; set; }

    [DisplayName("شماره درخواست")]
    public string RequestNumber { get; set; }
        
    [DisplayName("تاریخ درخواست")]
    public string RequestDateTime { get; set; }
        
    [DisplayName("عنوان فرآیند")]
    public string FlowNameAndVersion { get; set; }
        
    [DisplayName("عنوان زیر فرآیند")]
    public string SubprocessName { get; set; }
        
    [DisplayName("عنوان مرحله")]
    public string FlowLevelName { get; set; }
        
    [DisplayName("درخواست دهنده")]
    public string ApplicantName { get; set; }
        
    [DisplayName("مهلت اقدام")]
    public string TimeToDo { get; set; }
        
    [DisplayName("میزان تاخیر")]
    public double? DelayHour { get; set; }
}