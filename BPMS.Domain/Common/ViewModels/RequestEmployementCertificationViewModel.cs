namespace BPMS.Domain.Common.ViewModels;

public class RequestEmployementCertificationViewModel
{
    public Guid uniqeid { get; set; }
    public Guid RequestTypeId { get; set; }
    public string Dsr { get; set; }
    public string RequestIntention { get; set; }
    public string ConfirmPerson { get; set; }
    public bool CanPrint { get; set; }
    public string UserDsr { get; set; }
    public Guid RequestId { get; set; }
    public Guid StaffId { get; set; }
    public Guid WorkflowDetailId { get; set; }
    public string FullName { get; set; }
    public string InsuranceNumber { get; set; }
    public string IdNumber { get; set; }
    public string EmployementDate { get; set; }

}