using BPMS.Domain.Common.Dtos.Jira;
using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Services;

public interface IJiraService
{
    Task<ResponseCreateIssueDTO> CreateIssue(CreateIssueInputDTO createIssueDTO, string webRootPath);
    IEnumerable<JiraLogsViewModel> GetJiraLogsList();
    JiraLogDetailViewModel GetJiraLogById(Guid Id);
    Task<ResponseCreateIssueDTO> CreateIssueAgain(CreateIssueInputDTO createIssueDTO, string webRootPath);
}