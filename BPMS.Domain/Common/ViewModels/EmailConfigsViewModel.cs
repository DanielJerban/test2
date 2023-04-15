using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class EmailConfigsViewModel
{
    public Guid? Id { get; set; }

    [Display(Name = "آدرس SMTP")]
    [Required(ErrorMessage = "این فیلد اجباری است.")]
    public string SmtpServerUrl { get; set; }

    [Display(Name = "پورت")]
    [Required(ErrorMessage = "این فیلد اجباری است.")]
    public int PortNumber { get; set; }

    [Display(Name = "ایمیل")]
    [Required(ErrorMessage = "این فیلد اجباری است.")]
    public string Username { get; set; }

    [Display(Name = "کلمه عبور")]
    [Required(ErrorMessage = "این فیلد اجباری است.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "SSL")]
    public bool SslRequired { get; set; }

    [Display(Name = "فعال")]
    public bool IsActive { get; set; }
}