using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Services;

public interface IThirdPartyService
{
    void AddOrUpdateThirdParty(ThirdPartyViewModel model);
    void RemoveThirdParty(Guid id);
    List<ThirdPartyViewModel> GetThirdParties();
}