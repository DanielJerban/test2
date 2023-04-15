namespace BPMS.Infrastructure.Services;

public interface IWorkflowService
{
    public void CheckUniqueVersion(Guid workflowId, Guid reqTypeId, int orgVersion, int secVersion);
}