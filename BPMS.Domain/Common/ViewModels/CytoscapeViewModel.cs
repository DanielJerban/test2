namespace BPMS.Domain.Common.ViewModels;

public class CytoscapeViewModel
{
    public node data { get; set; }
    public edge edge { get; set; }
}

public class totalmodel
{
    public List<node> nodes { get; set; }
    public List<edge> edges { get; set; }
}
public class node
{
    public Guid id { get; set; }
    public string title { get; set; }
    public Guid? parent { get; set; }
}

public class edge
{
    public int id { get; set; }
    public Guid source { get; set; }
    public Guid target { get; set; }


}

public class data
{
    public Guid id { get; set; }
    public Guid source { get; set; }
    public Guid target { get; set; }
    public string label { get; set; }
}