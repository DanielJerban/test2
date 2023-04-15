using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Repositories;

public interface IJiraLogRepository
{
    void AddJiraLog(JiraLogViewModel model);
    IEnumerable<JiraLogsViewModel> GetJiraLogsList();
    JiraLogDetailViewModel GetJiraLogById(Guid Id);
}