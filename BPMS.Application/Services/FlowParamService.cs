using BPMS.Domain.Common.ViewModels;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;

namespace BPMS.Application.Services;

public class FlowParamService : IFlowParamService
{
    private readonly IDistributedCacheHelper _cacheHelper;

    public FlowParamService(IDistributedCacheHelper cacheHelper)
    {
        _cacheHelper = cacheHelper;
    }

    public void SetFlowParamInCache(FlowParam param, string userName)
    {
        _cacheHelper.SetObject(CacheKeyHelper.GetFlowParamCacheKey(userName), param, TimeSpan.FromDays(45));
    }

    public void ResetFlowParamCache(string userName)
    {
        _cacheHelper.Remove(CacheKeyHelper.GetFlowParamCacheKey(userName));
    }

    public FlowParam GetFlowParamFromCache(string userName)
    {
        return _cacheHelper.GetObject<FlowParam>(CacheKeyHelper.GetFlowParamCacheKey(userName));
    }
}