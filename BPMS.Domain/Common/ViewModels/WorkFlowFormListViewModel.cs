using System.ComponentModel.DataAnnotations;

namespace BPMS.Domain.Common.ViewModels;

public class WorkFlowFormListViewModel
{

    public Guid Id { get; set; }

    [Required(ErrorMessage = "{0} وارد نشده است")]
    [Display(Name = "عنوان")]
    public string Title { get; set; }

    public string JsonContent { get; set; }
    public string ColumnData { get; set; }
    public string GeneratedData { get; set; }
    public string SelectedTitle { get; set; }
    public string SelectedValue { get; set; }

    public IEnumerable<ComboListViewModel> ComboList { get; set; }
    public string GridColumns { get; set; }

    public bool IsMobile { get; set; }
}

public class ComboListViewModel
{

    public string Id { get; set; }
    public string Title { get; set; }
}