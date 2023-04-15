using BPMS.Domain.Common.Enums;
using BPMS.Infrastructure.Services.SMS;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS.Application.Services.SMS;

public class ProviderFactory
{
    private readonly ISmsLogService _smsLogService;
    private readonly ISmsProviderConfigService _smsProviderConfigService;
    public ProviderFactory(IServiceProvider serviceProvider)
    {
        _smsLogService = serviceProvider.GetRequiredService<ISmsLogService>();
        _smsProviderConfigService = serviceProvider.GetRequiredService<ISmsProviderConfigService>();
    }
    public ISMSProvider GetProvider()
    {
        var smsClient = _smsProviderConfigService.GetActiveProviderConfig();

        ISMSProvider smsProvider;

        var activeType = smsClient.SmsSendType;

        switch (activeType)
        {
            case SmsSenderType.webservice:
                {
                    smsProvider = new ParsaSMSProvider(_smsProviderConfigService, _smsLogService);
                    break;
                }
            case SmsSenderType.gsm:
                {
                    smsProvider = new GSMSMSProvider(/*_smsProviderConfigService, _smsLogService*/);
                    break;
                }
            default:
                {
                    smsProvider = new GSMSMSProvider(/*_smsProviderConfigService, _smsLogService*/);
                    break;
                }

        }

        return smsProvider;
    }
}