namespace BPMS.Domain.Common.Dtos.Jira;

public class CustomFieldDTO
{
    public CustomFieldDTO()
    {
        schema = new SchemaDTO();
    }
    public string id { get; set; }
    public SchemaDTO schema { get; set; }
}