using BPMS.BuildingBlocks.Application.Configuration.Commands;

namespace BPMS.Application.CQRS.Sample.Create;

public class CreateSampleCommand : CommandBase<CreateSampleCommandResult>
{
    public CreateSampleCommand(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}