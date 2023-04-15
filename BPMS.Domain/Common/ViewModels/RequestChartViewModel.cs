namespace BPMS.Domain.Common.ViewModels;

public class ChartsViewModel
{
    public List<ColumnChartViewModel> ColumnChart { get; set; }
    public List<PieChartViewModel> FlowPieChart { get; set; }
    public List<PieChartViewModel> RequestPieChart { get; set; }
    public List<BarChartViewModel> BarChart { get; set; }
}

public class PieChartViewModel
{
    public string category { get; set; }
    public float value { get; set; }
    public string color { get; set; }
    public float Tooltip { get; set; }
}
    
public class ColumnChartViewModel
{
    public float Count { get; set; }
    public string Title { get; set; }
}
    
public class BarChartViewModel
{
    public float Count { get; set; }
    public string Title { get; set; }
}