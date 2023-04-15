using BPMS.Domain.Common.Dtos;

namespace BPMS.Domain.Common.ViewModels;

public class ReciverPatternViewModel
{
    public List<string> OrganizationPosts { get; set; }
    public List<PatternDto> Patterns { get; set; }
}