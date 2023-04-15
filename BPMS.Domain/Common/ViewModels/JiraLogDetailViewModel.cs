using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class JiraLogDetailViewModel
{
    [Display(Name = "RequestUrl")]
    public string RequestUrl { get; set; }
    [Display(Name = "RequestData")]
    public string RequestData { get; set; }
    [Display(Name = "ResponseData")]
    public string ResponseData { get; set; }
}