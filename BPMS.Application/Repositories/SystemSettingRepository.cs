using BPMS.Application.Repositories.Base;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class SystemSettingRepository : Repository<SystemSetting>, ISystemSettingRepository
{
    public SystemSettingRepository(BpmsDbContext context) : base(context)
    {
    }
}