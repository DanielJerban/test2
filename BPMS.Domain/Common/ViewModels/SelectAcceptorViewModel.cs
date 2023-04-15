using BPMS.Domain.Entities;

namespace BPMS.Domain.Common.ViewModels;

public class SelectAcceptorViewModel
{
    public Request Request { get; set; }
    public bool IsSelectAcceptor { get; set; }


    public Guid? FlowId { get; set; }

}