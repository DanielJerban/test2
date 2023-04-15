namespace BPMS.Domain.Common.Dtos;

public class StaffDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid StaffTypeId { get; set; }
    public string StaffType { get; set; }
    public string PersonalCode { get; set; }
    public string FName { get; set; }
    public string LName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string ImagePath { get; set; }
    public string InsuranceNumber { get; set; }
    public string IdNumber { get; set; }
    public Guid EngTypeId { get; set; }
    public int? EmploymentDate { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public Guid SerialNumber { get; set; }

    //for List
    public string Title { get; set; }

    public bool InActive { get; set; }

}