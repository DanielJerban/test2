namespace BPMS.Domain.Common.Dtos.TncDataFix;

public class SetAssetActionInputDto
{
    public List<AssetNumberDatabase> AssetNumbers { get; set; }
    public Guid ToStaffId { get; set; }
}

public class AssetNumberDatabase
{
    public string AssetNumber { get; set; }
    public string DatabaseName { get; set; }
}