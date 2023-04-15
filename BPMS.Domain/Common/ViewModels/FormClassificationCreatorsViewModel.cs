using System.ComponentModel.DataAnnotations;
using BPMS.Domain.Entities;

namespace BPMS.Domain.Common.ViewModels;

public class FormClassificationCreatorsViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "عنوان بیمه")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public Guid FormClassificationId { get; set; }
    public FormClassification FormClassification { get; set; }

    [Display(Name = "پرسنل")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public Guid StaffId { get; set; }
    public Staff Staff { get; set; }

    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public Guid CreatorTypeId { get; set; }
    public LookUp CreatorType { get; set; }
}