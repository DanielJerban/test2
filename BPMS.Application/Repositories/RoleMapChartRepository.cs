using BPMS.Application.Repositories.Base;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class RoleMapChartRepository : Repository<RoleMapChart>, IRoleMapChartRepository
{
    public RoleMapChartRepository(BpmsDbContext context) : base(context)
    {
    }
}