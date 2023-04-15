using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories;
using BPMS.Infrastructure.Services.Email;

namespace BPMS.Application.Services.Email;

public class EmailLogService : IEmailLogService
{
    private readonly IEmailLogRepository _emailLogRepository;

    public EmailLogService(IEmailLogRepository emailLogRepository)
    {
        this._emailLogRepository = emailLogRepository;
    }
    public void AddEmailLog(string SenderEmail, string RecieverEmail, string text, bool sentstatus, string errormessage)
    {
        var Emaillog = new EmailLog()
        {
            SenderEmail = SenderEmail,
            RecieverEmail = RecieverEmail,
            EmailText = text,
            SentDate = HelperBs.ConvertMiladyToShamsi((DateTime.Now.ToString())),
            Time = DateTime.Now.ToString("HHmm"),
            SentStatus = sentstatus,
            ErrorMessage = errormessage
        };
        _emailLogRepository.AddEmailLog(Emaillog);
    }

    public IEnumerable<EmailLogsViewModel> GetEmailLogsList()
    {
        return _emailLogRepository.GetAllEmailLogs().Select(s => new EmailLogsViewModel
        {
            SenderEmail = s.SenderEmail,
            RecieverEmail = s.RecieverEmail,
            SentDate = s.SentDate.ToString().Insert(4, "/").Insert(7, "/"),
            Time = s.Time.Insert(2, ":"),
            SentStatus = s.SentStatus == true ? "ارسال شده" : "ارسال نشده",
            ErrorMessage = s.ErrorMessage,
            EmailText = s.EmailText
        }).OrderByDescending(o => o.SentDate);
    }
}