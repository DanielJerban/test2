using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Services.Email;

public interface IEmailConfigService
{
    EmailConfigsViewModel GetActiveConfig();
}