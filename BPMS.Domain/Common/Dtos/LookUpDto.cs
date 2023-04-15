namespace BPMS.Domain.Common.Dtos;

public class LookUpDto
{
    public Guid Id { get; set; }
    public int Code { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Aux { get; set; }
    public string Aux2 { get; set; }
    public bool IsActive { get; set; }
}