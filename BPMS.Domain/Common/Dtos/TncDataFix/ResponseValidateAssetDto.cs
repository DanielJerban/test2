namespace BPMS.Domain.Common.Dtos.TncDataFix;

public class ResponseValidateAssetDto
{
    public List<string> RejectedAssetNumbers { get; set; }
    public List<string> SuccessAssetNumbers { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }
}