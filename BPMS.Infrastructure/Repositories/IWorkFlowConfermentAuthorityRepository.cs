using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IWorkFlowConfermentAuthorityRepository : IRepository<WorkFlowConfermentAuthority>
{
    List<CreateAssignementViewModel> GetUsersForCartbotAssignment(Guid[] personIds, Guid confirmAuthorityId);
    IEnumerable<WorkFlowConfermentAuthorityViewModel> GetMasterGridData(string username);
    void CheckInputDates(int fromDate, int toDate);
}