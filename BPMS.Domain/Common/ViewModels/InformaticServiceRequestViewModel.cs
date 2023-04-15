using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class InformaticServiceRequestViewModel
{
    [Display(Name = "موضوع :")]
    public List<SelectListItem> DropDownItems { get; set; }

    [Display(Name = "پیام :")]
    public string Message { get; set; }
    public Guid uniqeid { get; set; }
    public bool CanPrint { get; set; }
    public string SelectedService { get; set; }

}