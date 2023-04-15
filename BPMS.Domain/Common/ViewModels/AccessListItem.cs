namespace BPMS.Domain.Common.ViewModels;

public class AccessListItem
{
    public AccessListItem()
    {
        Children = new List<AccessListItem>();
    }

    public string Title { get; set; }
    public string PersianName { get; set; }
    public string ClaimGuid { get; set; }
    public bool HasChild { get; set; }
    public bool IsParent { get; set; }
    public List<AccessListItem> Children { get; set; }
    public int? ParentId { get; set; }
    public int Id { get; set; }
    public bool Checked { get; set; }
    public string Datatest { get; set; }
    public string Icon { get; set; }
    public string ParentTitle { get; set; }
    public bool ConsiderInMenue { get; set; } = true;

}