using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class Client
{
    public Client()
    {
        Id = Guid.NewGuid();
    }
    [Key]
    public Guid Id { get; set; }
    [MaxLength(30)]
    [Required]
    public string FName { get; set; }
    [MaxLength(40)]
    [Required]
    public string LName { get; set; }
    [MaxLength(50)]
    public string? NationalNo { get; set; }
    [MaxLength(100)]
    public string? FromDsr { get; set; }
    public string? Address { get; set; }
    [MaxLength(20)]
    [Required]
    public string CellPhone { get; set; }
    public string? Dsr { get; set; }
    public bool Avtive { get; set; }
    [MaxLength(200)]
    public string? Email { get; set; }
    [MaxLength(50)]
    public string? OrganizationPost { get; set; }
}