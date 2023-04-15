namespace BPMS.Domain.Common.ViewModels;

public class RequestTypeBarChartViewModel
{
    public string Title2 { get; set; }
    public string Title { get; set; }
    public List<FirstType> ColumnChart { get; set; }
    public List<SecondType> ColumnChart2 { get; set; }
}

public class Datas
{
    public int TotalCount1 { get; set; }
        
}
public class FirstType
{
    public string Title { get; set; }
    public int Count { get; set; }

}
public class SecondType
{
    public string Title { get; set; }
    public int Count { get; set; }

}