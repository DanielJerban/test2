using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPMS.Domain.Entities;

public class Exceptions
{
    [Key]
    public Guid Id { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Display(Name = "شماره خطا")]
    public long Number { get; set; }

    [Display(Name = "تاریخ رویداد خطا")]
    public DateTime CreateDate { get; set; }

    public string? Content { get; set; }

    [Display(Name = "نام کاربری")]
    public string? UserName { get; set; }

    [Display(Name = "رویت شده؟")]
    public bool IsRead { get; set; }

    [Display(Name = "IP")]
    public string? IpAddress { get; set; }
}