using System.ComponentModel;

namespace BPMS.Domain.Common.ViewModels;

public class EmployementCertificateReportViewModel
{

    [DisplayName("شماره رهگیری")]
    public double RequestNo { get; set; }

    [DisplayName("نام کاربری")]
    public string PersonelCode { get; set; }

    [DisplayName("نام پرسنل")]
    public string FirstName { get; set; }

    [DisplayName("نام خانوادگی پرسنل")]
    public string LastName { get; set; }

    [DisplayName("تاریخ")]
    public string EmployementDate { get; set; }

    [DisplayName("جهت")]
    public string RequestIntention { get; set; }
}