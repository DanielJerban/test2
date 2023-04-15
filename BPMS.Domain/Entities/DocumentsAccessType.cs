namespace BPMS.Domain.Entities;

public class DocumentsAccessType
{
    public DocumentsAccessType()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public Guid FormClassificationAccessId { get; set; }
    public bool IsApproved { get; set; }
    public bool IsInProcess { get; set; }
    public bool IsExpired { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanRemove { get; set; }

}