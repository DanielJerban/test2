using BPMS.Domain.Entities;

namespace BPMS.Infrastructure.Services.Email;

public interface IEmailSender
{
    SendMessageResult Send(List<string> recievers, MessageContent message);
}