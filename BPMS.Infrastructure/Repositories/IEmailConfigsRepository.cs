using BPMS.Domain.Entities;

namespace BPMS.Infrastructure.Repositories;

public interface IEmailConfigsRepository
{
    void AddConfig(EmailConfigs configs);
    IEnumerable<EmailConfigs> GetConfigs();
    void UpdateConfig(EmailConfigs configs);
    void Remove(Guid id);
    void DeactivePrevious();
    EmailConfigs GetActiveConfig();
}