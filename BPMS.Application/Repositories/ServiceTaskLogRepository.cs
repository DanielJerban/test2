using BPMS.Application.Repositories.Base;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class ServiceTaskLogRepository : Repository<ServiceTaskLog>, IServiceTaskLogRepository
{
    public ServiceTaskLogRepository(BpmsDbContext context) : base(context)
    {

    }

    public ServiceTaskLog CreateServiceTaskLog(ServiceTaskLog model)
    {
        var entity = Context.ServiceTaskLogs.Add(model);
        return entity.Entity;
    }
}