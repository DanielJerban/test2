namespace BPMS.Domain.Entities;

public class UsefulLinks
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public bool IsExternalLink { get; set; }
}