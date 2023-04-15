using BPMS.Domain.Common.ViewModels;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;

namespace BPMS.Application.Services;

public class ThirdPartyService : IThirdPartyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCacheHelper _cacheHelper;

    public ThirdPartyService(IUnitOfWork unitOfWork, IDistributedCacheHelper cacheHelper)
    {
        _unitOfWork = unitOfWork;
        _cacheHelper = cacheHelper;
    }

    public void AddOrUpdateThirdParty(ThirdPartyViewModel model)
    {
        _unitOfWork.ThirdPartyRepository.AddOrUpdateThirdParty(model);
        _cacheHelper.Remove(CacheKeyHelper.GetThirdPartyCacheKey());
    }

    public void RemoveThirdParty(Guid id)
    {
        _unitOfWork.ThirdPartyRepository.RemoveThirdParty(id);
        _cacheHelper.Remove(CacheKeyHelper.GetThirdPartyCacheKey());
    }

    public List<ThirdPartyViewModel> GetThirdParties()
    {
        return _cacheHelper.GetOrSet(CacheKeyHelper.GetThirdPartyCacheKey(), () => _unitOfWork.ThirdPartyRepository.GetThirdParties(), TimeSpan.FromDays(30));
    }
}