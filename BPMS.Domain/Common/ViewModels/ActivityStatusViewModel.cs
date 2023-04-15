﻿using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class ActivityStatusViewModel
{
    public Guid WorkflowDetailId { get; set; }
    public Guid FlowId { get; set; }
    [Display(Name = "عنوان")]
    public string Title { get; set; }
    [Display(Name = "تعداد")]
    public int Count { get; set; }
    [Display(Name = "بیشترین زمان انجام (ساعت)")]
    public int Max { get; set; }
    [Display(Name = "کمترین زمان انجام (ساعت)")]
    public int Min { get; set; }
    [Display(Name = "میانگین زمان انجام (ساعت)")]
    public int Avg { get; set; }

    [Display(Name = "مدت زمان تعیین شده (ساعت)")]
    public int? WathingTime { get; set; }
    [Display(Name = "بیشترین زمان تاخیر (ساعت)")]
    public int MaxSediment { get; set; }

    [Display(Name = "میانگین زمان تاخیر (ساعت)")]
    public int AvgSediment { get; set; }
    [Display(Name = "کمترین زمان تاخیر (ساعت)")]
    public int MinSediment { get; set; }
}