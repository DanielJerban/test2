using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Entities;

public class EmployementCertificate
{
    public EmployementCertificate()
    {
        Id = Guid.NewGuid();
    }
    [Key]
    public Guid Id { get; set; }
    public Guid RequestId { get; set; }
    public string RequestIntention { get; set; }
    public string Dsr { get; set; }

    public virtual Request Requests { get; set; }

    public static explicit operator EmployementCertificate(string v)
    {
        throw new NotImplementedException();
    }
}