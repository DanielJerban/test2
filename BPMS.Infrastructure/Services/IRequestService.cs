using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Services;

public interface IRequestService
{
    string CreateRequest(CreateRequestDto dto,string webRootPath);
    ShowFlowDetailsViewModel GetFlowDetailsByRequestId(Guid requestId, Guid? workFlowDetailId);
    IEnumerable<FlowDetailsViewModel> GetRequestWorkFlowDetails(Guid requestId);
}