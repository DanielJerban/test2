using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class FormDataViewModel
{
    public string Key { get; set; }
    public ParamType ParamType { get; set; }
    public object Value { get; set; }
}

public enum ParamType
{
    [Display(Name = "متن")]
    Text,
    [Display(Name = "فایل")]
    File
}