namespace BPMS.Domain.Common.Dtos.TncDataFix;

public class AssetDto
{
    public bool IsChecked { get; set; }
    public string AssetNumber { get; set; }
    public string Title { get; set; }
    public string Location { get; set; }
    public string OwnerName { get; set; }

    public string DatabaseName { get; set; }
    public string CompanyName { get; set; }
}