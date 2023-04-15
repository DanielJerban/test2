namespace BPMS.Domain.Common.Dtos;

public class RelatedFormClassificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string FormTypeAuxAndNumber { get; set; }
    public string FormType_Title { get; set; }
    public string StandardType_Title { get; set; }
    public bool IsActive { get; set; }
}