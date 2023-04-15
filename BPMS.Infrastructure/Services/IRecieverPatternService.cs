using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Common.ViewModels.Global;

namespace BPMS.Infrastructure.Services;

public interface IRecieverPatternService
{
    void AddPattern(string patternName, List<string> selectedPosts);
    List<string> GetPosts();
    List<PatternDto> GetAllPatterns();
    EditPatternDto GetPatternById(Guid id);
    Result DeletePattern(Guid id);
    Result EditPattern(EditPatternViewModel model);
    void ResetPostsCache();
}