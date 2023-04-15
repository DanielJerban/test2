using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class UsefulLinksViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "عنوان لینک")]
    [Required(ErrorMessage = "این فیلد اجباری است.")]
    public string Name { get; set; }

    [Display(Name = "توضیحات")]
    public string Description { get; set; }

    [Display(Name = "Url")]
    [Required(ErrorMessage = "این فیلد اجباری است.")]
    public string Url { get; set; }

    [Display(Name = "لینک external است؟")]
    [Required(ErrorMessage = "این فیلد اجباری است.")]
    public bool IsExternalLink { get; set; }
}