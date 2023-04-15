using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IThirdPartyRepository : IRepository<ThirdParty>
{
    void AddOrUpdateThirdParty(ThirdPartyViewModel model);
    void RemoveThirdParty(Guid id);
    List<ThirdPartyViewModel> GetThirdParties();
}