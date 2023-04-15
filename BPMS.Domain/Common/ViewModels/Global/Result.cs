namespace BPMS.Domain.Common.ViewModels.Global;

public class Result
{
    public bool Success { get; set; }
    public bool IsValid { get; set; }
    public string Message { get; set; }
    public IDictionary<string, object> Messages { get; set; }
    public IDictionary<string, object> ErrorMessage { get; set; }
}