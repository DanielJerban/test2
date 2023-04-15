namespace BPMS.Domain.Common.Dtos;

public class StaffPhpDto
{
    public IList<string> PersonalCode { get; set; }
    public string UserName { get; set; }
    public string FName { get; set; }
    public string LName { get; set; }
    public string PhoneNumber { get; set; }
    public string ImagePath { get; set; }
    public string Password { get; set; }
    public string IsActive { get; set; }
    public string Action { get; set; }
    public string ApiKey { get; set; }
    public string EngType { get; set; }
    public string InsuranceNumber { get; set; }
    public string IdNumber { get; set; }
    public string EmploymentDate { get; set; }
    public string AgreementEndDate { get; set; }

    public string LocalPhone { get; set; }
    public string Building { get; set; }

    public string Gender { get; set; }

}