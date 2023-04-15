using BPMS.Infrastructure;
using BPMS.Infrastructure.Services;

namespace BPMS.Application.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IUnitOfWork _unitOfWork;
    public WorkflowService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void CheckUniqueVersion(Guid workflowId, Guid reqTypeId, int orgVersion, int secVersion)
    {
        int result = _unitOfWork.Workflows.Where(r => r.RequestTypeId == reqTypeId && r.OrginalVersion == orgVersion && r.SecondaryVersion == secVersion && r.Id != workflowId).Count();

        if (result > 0)
        {
            throw new ArgumentException("نسخه وارد شده تکراری است.");
        }
    }
}