using BPMS.Domain.Common.Enums;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories;
using BPMS.Infrastructure.Services.SMS;

namespace BPMS.Application.Services.SMS;

public class SmsLogService : ISmsLogService
{
    private readonly ISmsLogRepository _smsLogRepository;
    public SmsLogService(ISmsLogRepository smsLogRepository)
    {
        _smsLogRepository = smsLogRepository;
    }
    public void AddSmsLog(string ProviderNumber, string phoneNumber, string text, bool sentstatus, string errormessage, SmsSenderType smsSenderType)
    {

        var smslog = new SmsLog()
        {
            SenderNumber = ProviderNumber,
            RecieverNumber = phoneNumber,
            SmsText = text,
            SentDate = HelperBs.ConvertMiladyToShamsi((DateTime.Now.ToString())),
            Time = DateTime.Now.ToString("HHmm"),
            SentStatus = sentstatus,
            ErrorMessage = errormessage,
            SmsSendType = (int)smsSenderType
        };
        _smsLogRepository.AddSmsLog(smslog);
    }

    public IEnumerable<SmsLogsViewModel> GetSmsLogsList()
    {
        return _smsLogRepository.GetAllSmsLogs();
    }
}