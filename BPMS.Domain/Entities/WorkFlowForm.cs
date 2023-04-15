using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class WorkFlowForm
{
    public WorkFlowForm()
    {
        Id = Guid.NewGuid();
        WorkFlowDetails = new HashSet<WorkFlowDetail>();
    }
    [Key]
    public Guid Id { get; set; }
    public string? PName { get; set; }
    public byte[]? Content { get; set; }
    public byte[]? Jquery { get; set; }
    public byte[]? AdditionalCssStyleCode { get; set; }
    public Guid StaffId { get; set; }
    public int RegisterDate { get; set; }
    [MaxLength(4)]
    public string? RegisterTime { get; set; }
    public Guid? ModifiedId { get; set; }
    public int? ModifideDate { get; set; }
    [MaxLength(4)]
    public string? ModifideTime { get; set; }
    public string? DocumentCode { get; set; }
       
    [Display(Name = "نسخه اصلی")]
    [Required(ErrorMessage = "{0} را وارد نمایید.")]
    public int OrginalVersion { get; set; }
    [Display(Name = "نسخه فرعی")]
    [Required(ErrorMessage = "{0} را وارد نمایید.")]
    public int SecondaryVersion { get; set; }

    public ICollection<WorkFlowDetail> WorkFlowDetails { get; set; }
    public virtual Staff Staff { get; set; }
    public virtual Staff? Modifier { get; set; }
}