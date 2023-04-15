using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class UserLoginOut
{
    public UserLoginOut()
    {
        Id = Guid.NewGuid();
    }

    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid LoginOutTypeId { get; set; }

    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0: ####/##/##}")]
    public int Date { get; set; }

    [MaxLength(4)]
    public string? Time { get; set; }
       
    public string? Ip { get; set; }
    public string? MachineName { get; set; }
    public string? BrowserName { get; set; }
        
      
    public virtual User User { get; set; }
        
    public virtual LookUp UserLoginoutType { get; set; }
}