using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class InformaticRequestsReportPerSupportExpert
{
    public IEnumerable<InformaticRequestsReportViewModel> InformaticRequests { get; set; }
    public string Title { get; set; }
}

public class InformaticRequestsReportViewModel
{
    [Display(Name = "شماره درخواست")]
    public long RequestNo { get; set; }

    [Display(Name = "نام و نام خانوادگی")]
    public string RequestorFullName { get; set; }

    [Display(Name = "وضعیت فرآیند")]
    public string RequestStatus { get; set; }
    [Display(Name = "وضعیت مرحله")]
    public string FlowtStatus { get; set; }

    [Display(Name = "نام کارشناس")]
    public string ResponsiblePerson { get; set; }

    [Display(Name = "نام واحد")]
    public string ChartTitle { get; set; }

    [Display(Name = "تاریخ درخواست")]
    public string LastEditDate { get; set; }

    [Display(Name = "زمان درخواست")]
    public string RequestTime { get; set; }

    [Display(Name = "عنوان درخواست")]
    public string ServiceTitles { get; set; }

    [Display(Name = "عنوان مرحله")]
    public string StepTitle { get; set; }
}