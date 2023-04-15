namespace BPMS.Domain.Common.Dtos.TncDataFix;

public class AssetInputDto
{
    public System.Uri Url { get; set; }
    public string PersonalCode { get; set; }
    public string Token { get; set; }

    public string DatabaseName { get; set; }
    public string CompanyName { get; set; }
}