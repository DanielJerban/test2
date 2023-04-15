using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class FormClassification
{
    [Key]
    [Display(Name = "شناسه")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Display(Name = "عنوان مدرک")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public string Title { get; set; }

    [MaxLength(30, ErrorMessage = "مقدار {0} باید کمتر از {1} کاراکتر باشد")]
    [Display(Name = "شناسه مدرک")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public string FormNo { get; set; }

    [Display(Name = "نوع مدرک")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public Guid FormTypeId { get; set; }
    public virtual LookUp FormType { get; set; }

    [Display(Name = "وضعیت")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public Guid FormStatusId { get; set; }
    public virtual LookUp FormStatus { get; set; }

    [Display(Name = "تاریخ ویرایش")]
    public int? EditDate { get; set; }

    [Display(Name = "فعال")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "نوع استاندارد")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public Guid StandardTypeId { get; set; }
    public virtual LookUp StandardType { get; set; }

    [MaxLength(10, ErrorMessage = "مقدار {0} باید کمتر از {1} کاراکتر باشد")]
    [Display(Name = "شماره ویرایش")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public string EditNo { get; set; }

    [Display(Name = "نوع درسترسی")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public Guid AccessTypeId { get; set; }
    public virtual LookUp AccessType { get; set; }

    [Display(Name = "نام فرآیند")]
    public Guid? WorkFlowLookupId { get; set; }
    public virtual LookUp? WorkFlowLookup { get; set; }

    [Display(Name = "سطح محرمانگی")]
    public Guid? ConfidentialLevelId { get; set; }
    public LookUp? ConfidentialLevel { get; set; }

    [Display(Name = "تاریخ ویرایش رکورد")]
    public int? RecordEditDate { get; set; }

    [Display(Name = "تاریخ ثبت")]
    public int RegisterDate { get; set; }

    [Display(Name = "تاریخ ثبت")]
    public int? CreatedDate { get; set; }

    [Display(Name = "خلاصه تغییرات")]
    public string? Dsr { get; set; }

    [Display(Name = "تعداد بازدید")]
    [Required(ErrorMessage = "مقدار {0} الزامی میباشد")]
    public int Counter { get; set; }

    [Display(Name = "کلمات کلیدی")]
    public string? Tags { get; set; }
    public Guid? Parent { get; set; }

    public bool IsImplementedInBpms { get; set; }
    public virtual ICollection<FormClassificationCreators> FormClassificationCreators { get; set; }
    public virtual ICollection<FormClassificationAccess> FormClassificationAccess { get; set; }
        
}