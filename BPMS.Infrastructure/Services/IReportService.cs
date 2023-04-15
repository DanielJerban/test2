using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Services;

public interface IReportService
{
    List<Guid> GetAdminSubStaffs(string personalCode);
    List<RequestDelayViewModel> GetIncompletedRequests();
    void SetPrintReportModelInCache(PrintReportViewModel model, Guid id);
    PrintReportViewModel GetPrintReportModelFromCache(Guid id);
    void ResetPrintReportCache(Guid id);
}