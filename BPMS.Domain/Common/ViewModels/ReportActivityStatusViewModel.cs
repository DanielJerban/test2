using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class ReportActivityStatusViewModel
{
    public Guid WorkflowDetailId { get; set; }
    public Guid FlowId { get; set; }
    [Display(Name = "عنوان")]
    public string Title { get; set; }
    [Display(Name = "تعداد")]
    public int Count { get; set; }
    [Display(Name = "بیشترین زمان انجام (ساعت)")]
    public double Max { get; set; }
    [Display(Name = "کمترین زمان انجام (ساعت)")]
    public double Min { get; set; }
    [Display(Name = "میانگین زمان انجام (ساعت)")]
    public double Avg { get; set; }

    [Display(Name = "مدت زمان تعیین شده (ساعت)")]
    public double? WathingTime { get; set; }
    [Display(Name = "بیشترین زمان تاخیر (ساعت)")]
    public double MaxSediment { get; set; }

    //[Display(Name = "میانگین زمان تاخیر (ساعت)")]
    //public double AvgSediment { get; set; }
    [Display(Name = "کمترین زمان تاخیر (ساعت)")]
    public double MinSediment { get; set; }
}