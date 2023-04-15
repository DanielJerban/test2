using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class ReportViewModel
{
    public Guid Id { get; set; }
    [DisplayName("عنوان")]
    [Required(ErrorMessage = "{0} وارد نشده است")]
    public string Title { get; set; }
    public Guid CreatorId { get; set; }
    [DisplayName("نام ایجاد کننده")]
    public string Creator { get; set; }
    public byte[] Expersion { get; set; }
    [DisplayName("تاریخ ثبت")]
    public int RegisterDate { get; set; }
    [DisplayName("فعال")]
    public bool IsActive { get; set; }
    public string Content { get; set; }
    public string PrintFileName { get; set; }
    [DisplayName("نام فرآیند")]
    public string WorkflowName { get; set; }
    public Guid? WorkflowId { get; set; }
    public List<SelectListItem> WorkflowItems { get; set; }
}