using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IFormClassificationAccessRepository : IRepository<FormClassificationAccess>
{
    List<Guid> GetChartByFormClassificationId(Guid id);
}