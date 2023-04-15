using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Services.Email;

public interface IEmailLogService
{
    void AddEmailLog(string SenderEmail, string RecieverEmail, string text, bool sentstatus, string errormessage);
    IEnumerable<EmailLogsViewModel> GetEmailLogsList();
}