namespace BPMS.Domain.Common.Dtos;

public class ClientDto
{
    public Guid Id { get; set; }
    public string FName { get; set; }
    public string LName { get; set; }
    public string NationalNo { get; set; }
    public string FromDsr { get; set; }
    public string Address { get; set; }
    public string CellPhone { get; set; }
    public string Dsr { get; set; }
    public bool Avtive { get; set; }
    public string Email { get; set; }
    public string OrganizationPost { get; set; }
    public string FullName { get; set; }

}