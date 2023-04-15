using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IUsefulLinksRepository : IRepository<UsefulLinks>
{
    void AddUsefulLink(UsefulLinks model);
    void UpdateUsefulLink(UsefulLinks model);
    bool RemoveUsefulLink(Guid id);
    IQueryable<UsefulLinks> GetAllUsefulLinks();
}