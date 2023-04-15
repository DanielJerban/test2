using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IAssingnmentRepository : IRepository<Assingnment>
{
    void ModifyBpmsGroupMapStaffId(IEnumerable<Guid> staffIds, Guid bpmsGroupId);
    IEnumerable<StaffViewModel> StaffsInSpecificBpmnGroup(Guid id);
}