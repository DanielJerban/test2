using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;

namespace BPMS.Infrastructure.Repositories;

public interface IPermissionLogsRepository
{
    void AddPermissionLog(List<PermissionLog> model);
    List<PermissionLogsViewModel> GetPermissionLogsModels();
}