namespace BPMS.Domain.Common.Dtos;

public class GetUserFullDataByUsernameDTO
{
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool IsActive { get; set; }
    public string LDAPUserName { get; set; }
    public string LDAPDomainName { get; set; }
    public Guid SerialNumber { get; set; }
    public bool TwoStepVerification { get; set; }
    public bool TwoStepVerificationByEmail { get; set; }
    public bool TwoStepVerificationByGoogleAuthenticator { get; set; }
    public string GoogleAuthKey { get; set; }

    public GetUserFullDataByUsernameStaffDTO Staff { get; set; }
}

public class GetUserFullDataByUsernameStaffDTO
{
    public Guid Id { get; set; }
    public string PersonalCode { get; set; }
    public string FName { get; set; }
    public string LName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string ImagePath { get; set; }
    public Guid StaffTypeId { get; set; }
    public Guid EngTypeId { get; set; }
    public string LocalPhone { get; set; }
    public Guid? BuildingId { get; set; }
    public string InsuranceNumber { get; set; }
    public string IdNumber { get; set; }
    public int? EmploymentDate { get; set; }
    public int? AgreementEndDate { get; set; }
    public byte Gender { get; set; }
    public string FullName => FName + " " + LName;
}