namespace BPMS.Domain.Common.ViewModels;

public class GsmProviderViewModel : ProviderViewModel
{
    public string GsmPort { get; set; }
    public string GsmPortRate { get; set; }
    public string GsmPortReadTimeout { get; set; }
    public string GsmPortWriteTimeout { get; set; }

}