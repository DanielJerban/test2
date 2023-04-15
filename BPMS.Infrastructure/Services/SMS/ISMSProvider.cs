namespace BPMS.Infrastructure.Services.SMS;

public interface ISMSProvider
{
    bool Send(string phoneNumber, string text);
}