using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Services;

public interface IFlowParamService
{
    void SetFlowParamInCache(FlowParam param, string userName);
    void ResetFlowParamCache(string userName);
    FlowParam GetFlowParamFromCache(string userName);
}