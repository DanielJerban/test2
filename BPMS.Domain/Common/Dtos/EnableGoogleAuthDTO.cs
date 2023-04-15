using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.Dtos;

public class EnableGoogleAuthDTO
{
    [Required]
    public string Email { get; set; }
    public string QrImage { get; set; }
    public string QrKey { get; set; }
}