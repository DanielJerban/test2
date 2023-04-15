using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Constants;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class FlowEventRepository : Repository<FlowEvent>, IFlowEventRepository
{
    public BpmsDbContext DbContext => Context;
    public FlowEventRepository(BpmsDbContext context) : base(context)
    {
    }

    public List<FlowEvent> GetEvents(Guid flowId)
    {
        return Context.FlowEvents.Where(d => d.FlowId == flowId).OrderBy(d => d.Order).ToList();
    }

    public List<FlowEvent> GetFlowEventsContainsintermediateCatchEventSignal(string remoteId)
    {
        return Context.FlowEvents.Where(d => d.WorkflowEsb.EventId.Contains(BpmnNodeConstant.Boundary.IntermediateCatchEventSignal) && d.Value == remoteId && d.Flow.LookUpFlowStatus.Code == 1).ToList();
    }
    public List<FlowEvent> GetFlowEventsContainsintermediateCatchEventMessage()
    {
        return Context.FlowEvents.Where(d => d.WorkflowEsb.EventId.Contains(BpmnNodeConstant.Boundary.IntermediateCatchEventMessage) && d.Flow.LookUpFlowStatus.Code == 1).ToList();
    }
}