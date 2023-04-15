using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class Company
{
    public Company()
    {
        Id = Guid.NewGuid();
    }
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
    [MaxLength(20)]
    public string? EconomicCode { get; set; }
    [MaxLength(30)]
    public string? ShortName { get; set; }
    [MaxLength(60)]
    public string? Telephone { get; set; }
    [MaxLength(30)]
    public string? Fax { get; set; }
    [MaxLength(30)]
    public string? Email { get; set; }
    [MaxLength(100)]
    public string? WebSite { get; set; }
    [MaxLength(20)]
    public string? PostalCode { get; set; }
    [MaxLength(500)]
    public string? FullAddress { get; set; }
    public string? Dsr { get; set; }
    [Required]
    public long RegisterDate { get; set; }
    [MaxLength(20)]
    public string? NationalCode { get; set; }
    [MaxLength(20)]
    public string? RegistrationNo { get; set; }
    public string? RegisterTime { get; set; }
}