using System.ComponentModel;

namespace BPMS.Domain.Common.ViewModels;

public class DynamicViewModel
{
    public string Tree { get; set; }

    [DisplayName("عنوان")]
    public dynamic Title { get; set; }
}