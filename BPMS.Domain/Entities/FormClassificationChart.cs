namespace BPMS.Domain.Entities;

public class FormClassificationAccess
{
    public FormClassificationAccess()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public Guid FormClassificationId { get; set; }
    public string Type { get; set; }
    public Guid AccessId { get; set; }

    public FormClassification FormClassification { get; set; }
}