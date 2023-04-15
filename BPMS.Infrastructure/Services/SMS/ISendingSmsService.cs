namespace BPMS.Infrastructure.Services.SMS;

public interface ISendingSmsService
{
    void SendSms(string phoneNumber, string text);
    void SendSms(List<string> phoneNumbers, string text);
}