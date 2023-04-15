namespace BPMS.Domain.Common.ViewModels;

public class SmsProviderViewModel : ProviderViewModel
{
    public string Name { get; set; }
    public string ProviderNumber { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ApiKey { get; set; }
    public string Uri { get; set; }
}