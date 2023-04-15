using BPMS.Application.Repositories.Base;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class WorkflowNextStepsRepository : Repository<WorkFlowNextStep>, IWorkflowNextStepsRepository
{
    public WorkflowNextStepsRepository(BpmsDbContext context) : base(context)
    {
    }
    public BpmsDbContext DbContext => Context;


}