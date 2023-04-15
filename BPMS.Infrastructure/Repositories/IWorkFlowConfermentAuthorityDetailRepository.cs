using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IWorkFlowConfermentAuthorityDetailRepository : IRepository<WorkFlowConfermentAuthorityDetail>
{
    IEnumerable<CreateAssignementViewModel> GetDetailsRecords(Guid id);
}