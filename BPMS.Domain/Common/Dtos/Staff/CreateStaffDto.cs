namespace BPMS.Domain.Common.Dtos.Staff;

public class CreateStaffDto
{
    public string PersonalCode { get; set; }
    public string FName { get; set; }
    public string LName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Guid StaffTypeId { get; set; }
    public string ImagePath { get; set; }
    public string InsuranceNumber { get; set; }
    public string IdNumber { get; set; }
    public string Password { get; set; }
    public string LdapUserName { get; set; }
    public string LdapDomainName { get; set; }
}