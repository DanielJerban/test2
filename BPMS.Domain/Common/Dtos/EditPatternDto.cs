namespace BPMS.Domain.Common.Dtos;

public class EditPatternDto
{
    public List<PatternItemDto> Items { get; set; }
    public string Title { get; set; }
    public Guid PatternId { get; set; }
    public List<string> OrganizationPosts { get; set; }
}