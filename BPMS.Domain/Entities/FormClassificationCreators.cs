using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class FormClassificationCreators
{
    [Key]
    public Guid Id { get; set; }
    [Display(Name = "عنوان بیمه")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public Guid FormClassificationId { get; set; }
    public virtual FormClassification FormClassification { get; set; }
    [Display(Name = "پرسنل")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public Guid StaffId { get; set; }
    public virtual Staff Staff { get; set; }
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public Guid CreatorTypeId { get; set; }
    public virtual LookUp CreatorType { get; set; }
}