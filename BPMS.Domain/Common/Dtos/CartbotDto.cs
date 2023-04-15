namespace BPMS.Domain.Common.Dtos;

public class CartbotDto
{
    public Guid Id { get; set; }

    public Guid StaffId { get; set; }
    public string PersonalCode { get; set; }
    // public string ApiKey { get; set; }

    public string Type { get; set; }
    public int? Code { get; set; }

}