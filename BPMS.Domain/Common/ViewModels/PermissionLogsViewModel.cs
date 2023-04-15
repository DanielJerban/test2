using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Enums;

namespace BPMS.Domain.Common.ViewModels;

public class PermissionLogsViewModel
{
    [Display(Name = "ID")]
    public Guid Id { get; set; }

    [Display(Name = "نوع دسترسی")]
    public string ClaimType { get; set; }

    public string ClaimValue { get; set; }

    [Display(Name = "دسترسی به")]
    public string ClaimDesc { get; set; }

    [Display(Name = "نام گروه دسترسی گیرنده")]
    public string RoleName { get; set; }

    [Display(Name = "کاربر دسترسی دهنده")]
    public string CreatorUser { get; set; }

    [Display(Name = "نوع عملیات")]
    public PermissionActionType ActionType { get; set; }

    [Display(Name = "تاریخ و ساعت")]
    public string CreateDateTime { get; set; }

    [Display(Name = "نام کاربر دسترسی گیرنده")]
    public string FullName { get; set; }
}