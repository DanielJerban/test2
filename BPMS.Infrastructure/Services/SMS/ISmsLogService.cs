using BPMS.Domain.Common.Enums;
using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Services.SMS;

public interface ISmsLogService
{
    void AddSmsLog(string providerNumber, string phoneNumber, string text, bool sentStatus, string errorMessage, SmsSenderType smsSenderType);
    IEnumerable<SmsLogsViewModel> GetSmsLogsList();
}