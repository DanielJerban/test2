using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class RequestViewModel
{
    public Guid Id { get; set; }
    [DisplayName("تاریخ ثبت")]
    public int RegisterDate { get; set; }

    [MaxLength(4)]
    [DisplayName("زمان ثبت")]
    public string RegisterTime { get; set; }
    [DisplayName("شماره درخواست")]
    public long RequestNo { get; set; }
    [DisplayName("عنوان فرآیند")]
    public string RequestType { get; set; }
    [DisplayName("وضعیت درخواست")]
    public string RequestStatus { get; set; }
    [DisplayName("نسخه")]
    public string Version { get; set; }
    [DisplayName("درخواست دهنده")]
    public string Staff { get; set; }
    [DisplayName("پست سازمانی")]
    public string OrganizationPostTitle { get; set; }

    public string FlowId { get; set; }
}