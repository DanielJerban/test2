namespace BPMS.Application.CQRS.Sample.Create;

public class CreateSampleCommandResult
{
    public CreateSampleCommandResult(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}