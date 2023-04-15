using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IWorkFlowFormListRepository : IRepository<WorkFlowFormList>
{
    void RemoveList(Guid id);
    IEnumerable<WorkFlowFormListListDto> GetLists();
    WorkFlowFormListViewModel GetListElementById(Guid id);
    void CheckForDelete(Guid id);
    WorkFlowFormListViewModel GetListForEditGrid(Guid idValue, bool hasData, bool hasCurrent, Guid requestId);
    WorkFlowFormListViewModel GetListForEdit(Guid idValue, bool hasData);
    void CheckVersion(Guid reqTypeId, int orgVersion, int secVersion);
    IEnumerable<WorkFlowFormListListDto> GetListsWithAccess(string username);
    void UpdateListInCartable(string gridList, Guid requestId);
    IEnumerable<WorkFlowFormListListDto> GetListsWithAccessPolicy(string username);
}