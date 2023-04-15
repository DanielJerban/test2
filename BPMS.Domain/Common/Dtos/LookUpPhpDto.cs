namespace BPMS.Domain.Common.Dtos;

public class LookUpPhpDto
{
    public Guid Id { get; set; }
    public int Code { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Aux { get; set; }
    public string IsActive { get; set; }

    public string Action { get; set; }
    public string ApiKey { get; set; }
}