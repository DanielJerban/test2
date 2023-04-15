using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IServiceTaskLogRepository : IRepository<ServiceTaskLog>
{
    ServiceTaskLog CreateServiceTaskLog(ServiceTaskLog model);
}