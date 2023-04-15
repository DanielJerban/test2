using BPMS.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class ProviderViewModel
{
    public Guid Id { get; set; }
    public SmsSenderType SmsSenderType { get; set; }
    [Display(Name = "فعال")]
    [Required(ErrorMessage = "این فیلد اجباری است.")]
    public bool IsActive { get; set; } = true;

}