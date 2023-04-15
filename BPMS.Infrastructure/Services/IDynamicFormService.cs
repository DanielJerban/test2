using BPMS.Domain.Common.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BPMS.Infrastructure.Services;

public interface IDynamicFormService
{
    //object Create(string model, Guid unig, Guid requestId, string staffSelected, string gridList, string UserName, List<IFormFile> files, ControllerContext context, string webRootPath);
    bool IsViewNameCreateProcess(Guid requestTypeId, Guid? organizationPostTitleId, Guid unig, Guid? staffId, out WorkFlowFormViewModel model, out string[] viewName, string userName,string webRootPath);
    Guid GetParentWorkFlowId(Guid? id);
}