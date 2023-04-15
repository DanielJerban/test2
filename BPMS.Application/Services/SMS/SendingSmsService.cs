using BPMS.Infrastructure.Services.SMS;

namespace BPMS.Application.Services.SMS;

public class SendingSmsService : ISendingSmsService
{
    private readonly IServiceProvider _serviceProvider;

    public SendingSmsService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void SendSms(string phoneNumber, string text)
    {
        var numbers = new List<string> { phoneNumber };

        SendSms(numbers, text);
    }

    public void SendSms(List<string> phoneNumbers, string text)
    {
        Task.Factory.StartNew(() =>
        {
            var currentProvider = new ProviderFactory(_serviceProvider).GetProvider();
            foreach (string phoneNumber in phoneNumbers)
            {
                currentProvider.Send(phoneNumber, text);
            }
        });
    }
}