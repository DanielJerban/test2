using System.Net;

namespace BPMS.Domain.Common.ViewModels;

public class SystemApiResultViewModel
{
    public string Content { get; set; }
    public string ErrorMessage { get; set; }
    public bool Success { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public bool UseInGrid { get; set; }
}