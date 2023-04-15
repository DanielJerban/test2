namespace BPMS.Domain.Common.ViewModels;

public class WorkflowEsbViewModel
{
    public IList<Guid> Staffs { get; set; }
    public IList<Guid> Charts { get; set; }
    public IList<Guid> Companies { get; set; }
    public IList<Guid> Clients { get; set; }
    public IList<Other> Others { get; set; }
    public FromForm FromForm { get; set; }
}

public class Other
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}

public class FromForm
{
    public Guid? WorkFlowId { get; set; }
    public string FieldId { get; set; }
}