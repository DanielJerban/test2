namespace BPMS.Domain.Common.Dtos;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string EconomicCode { get; set; }
    public string ShortName { get; set; }
    public string Telephone { get; set; }
    public string Fax { get; set; }
    public string Email { get; set; }
    public string WebSite { get; set; }
    public string PostalCode { get; set; }
    public string FullAddress { get; set; }
    public string Dsr { get; set; }
    public long RegisterDate { get; set; }
    public string NationalCode { get; set; }
    public string RegistrationNo { get; set; }
}