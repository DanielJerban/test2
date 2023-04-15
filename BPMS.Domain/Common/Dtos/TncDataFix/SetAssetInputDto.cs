namespace BPMS.Domain.Common.Dtos.TncDataFix;

public class SetAssetInputDto
{
    public Uri Url { get; set; }
    public string Token { get; set; }

    // Data to get From User
    public string AssetNo { get; set; }
    public int LocationId { get; set; }
    public string ToPersonalCode { get; set; }
    public DateTime DateTime { get; set; }
    public string DatabaseName { get; set; }
}