using System.ComponentModel;

namespace BPMS.Domain.Common.ViewModels;

public class GetScheduleLogViewModel
{
    public Guid Id { get; set; }
    public Guid ScheduleId { get; set; }
    [DisplayName("عنوان")]
    public string Schedule { get; set; }
    [DisplayName("نوع کار")]
    public string TaskType { get; set; }
    [DisplayName("تاریخ اجرا")]
    public int RunDate { get; set; }
    [DisplayName("زمان اجرا")]
    public string RunTime { get; set; }
    [DisplayName("تاریخ ثبت")]
    public int RegisterDate { get; set; }
    [DisplayName("زمان ثبت")]
    public string RegisterTime { get; set; }
}