using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Services;

public interface IErrorLogService
{
    IEnumerable<ElmahErrorLogsViewModel> GetErrorLogsList();
    ElmahErrorLogsViewModel GetErrorBySecuence(int? Sequence);
    int GetLastLog();
}