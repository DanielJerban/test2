using BPMS.Application.Repositories.Base;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class WorkFlowBoundaryRepository : Repository<WorkFlowBoundary>, IWorkFlowBoundaryRepository
{
    public WorkFlowBoundaryRepository(BpmsDbContext context) : base(context)
    {
    }
}