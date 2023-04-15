using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Common.ViewModels.Global;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IWorkFlowDetailPatternRepository : IRepository<WorkflowDetailPattern>
{
    void AddPattern(string patternName, List<(Guid, string, int)> items);
    Result DeletePattern(Guid id);
    Result EditPattern(EditPatternViewModel model);
    Guid GetIdByName(string name);
}