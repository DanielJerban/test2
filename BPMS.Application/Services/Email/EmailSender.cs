using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;
using BPMS.Infrastructure.Services.Email;
using System.Net;
using System.Net.Mail;

namespace BPMS.Application.Services.Email;

public class EmailSender : IEmailSender
{
    public EmailSender(IEmailConfigService emailConfigService, IDistributedCacheHelper cacheHelper, IEmailLogService emailLogService)
    {
        _emailConfigService = emailConfigService;
        _cacheHelper = cacheHelper;
        _emailLogService = emailLogService;
    }

    private readonly IEmailLogService _emailLogService;
    private readonly IDistributedCacheHelper _cacheHelper;

    private readonly IEmailConfigService _emailConfigService;

    public SendMessageResult Send(List<string> recievers, MessageContent message)
    {
        var result = new SendMessageResult();
        var configs = _cacheHelper.GetObject<EmailConfigsViewModel>(CacheKeyHelper.GetActiveConfig());
        try
        {
            if (configs == null)
            {
                configs = _emailConfigService.GetActiveConfig();
                if (configs == null)
                {
                    result.Message = "تنظیمات فعالی وجود ندارد.";
                    return result;
                }
                _cacheHelper.Remove(CacheKeyHelper.GetActiveConfig());
                _cacheHelper.SetObject(CacheKeyHelper.GetActiveConfig(), configs, TimeSpan.FromDays(365));
            }

            var fromAddress = new MailAddress(configs.Username);
            var mail = new MailMessage
            {
                From = fromAddress,
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };
            foreach (var email in recievers)
            {
                mail.To.Add(email);
            }

            var client = new SmtpClient(configs.SmtpServerUrl, configs.PortNumber);
            client.Credentials = new NetworkCredential(configs.Username, configs.Password);
            client.EnableSsl = configs.SslRequired;

            client.Send(mail);
            foreach (var email in recievers)
            {
                _emailLogService.AddEmailLog(configs.Username, email, message.Body, true, null);
            }
            result.Message = "پیام با موفقیت ارسال شد.";
            result.IsSucceed = true;

        }
        catch (Exception e)
        {
            if (recievers != null)
            {
                foreach (var email in recievers)
                {
                    _emailLogService.AddEmailLog(configs.Username, email, message.Body, false, e.Message);
                }
            }

            result.Message = "ارسال پیام با خطا مواجه شد.";
            result.ErrorMessage = e.Message;
        }
        return result;
    }
    private static EmailConfigs GetConfigs()
    {
        var configs = new EmailConfigs();

        configs.SmtpServerUrl = "smtp.gmail.com";
        configs.PortNumber = 587;
        configs.Username = "email.torfehnegar@gmail.com";
        configs.Password = "pass@tnc";
        configs.SslRequired = true;

        return configs;
    }
}