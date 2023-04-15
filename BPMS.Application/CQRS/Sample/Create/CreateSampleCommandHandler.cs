using BPMS.BuildingBlocks.Application.Configuration.Commands;
using Microsoft.Extensions.Logging;

namespace BPMS.Application.CQRS.Sample.Create;

public class CreateSampleCommandHandler : CommandHandlerBase<CreateSampleCommand, CreateSampleCommandResult>
{
    public CreateSampleCommandHandler(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }

    protected override Task<CreateSampleCommandResult> Batch(CreateSampleCommand command, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new CreateSampleCommandResult(command.Name));
    }
}