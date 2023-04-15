using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;

namespace BPMS.Infrastructure.Repositories;

public interface ISmsLogRepository
{
    void AddSmsLog(SmsLog model);
    IEnumerable<SmsLogsViewModel> GetAllSmsLogs();
}