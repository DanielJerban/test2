using BPMS.Domain.Common.Enums;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;

namespace BPMS.Infrastructure.Services.SMS;

public interface ISmsProviderConfigService
{
    List<SmsConfigGetListViewModel> GetProviderList();
    void ActivateProvider(Guid id);
    SystemSetting GetById(Guid id);
    SmsConfigGetListViewModel GetActiveProviderConfig();
    (bool, string) RemoveSMSConfig(Guid id);
    SystemSetting AddSMSConfig(object data, string username, SystemSettingType type);
}