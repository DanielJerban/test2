using Kendo.Mvc;

namespace BPMS.Domain.Common.ViewModels;

public class DataSourceRequestViewModel
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Code { get; set; }
    public IList<FilterKendoViewModel> Filters { get; set; }
    public List<SortDescriptor> Sorts { get; set; }
}

public class FilterKendoViewModel
{
    public string Value { get; set; }
    public string Member { get; set; }
}