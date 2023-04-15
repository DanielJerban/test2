namespace BPMS.Domain.Common.Dtos.Automation;

public class AutomationResultDto<T> where T : class
{
    public bool status { get; set; }
    public string message { get; set; }
    public object error { get; set; }
    public IList<T> data { get; set; }
}