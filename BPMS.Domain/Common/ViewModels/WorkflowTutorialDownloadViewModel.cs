using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class WorkflowTutorialDownloadViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "نام فرآیند")]
    public string RequestType { get; set; }

    [DisplayName("گروه فرآیند")]
    public string RequestGroupType { get; set; }

    [Display(Name = "نسخه")]
    public string Version { get; set; }
}