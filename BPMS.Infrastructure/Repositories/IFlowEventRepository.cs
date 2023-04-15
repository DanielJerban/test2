using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IFlowEventRepository : IRepository<FlowEvent>
{
    List<FlowEvent> GetEvents(Guid flowId);
    List<FlowEvent> GetFlowEventsContainsintermediateCatchEventSignal(string remoteId);
    List<FlowEvent> GetFlowEventsContainsintermediateCatchEventMessage();
}