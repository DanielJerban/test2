using BPMS.BuildingBlocks.Domain;

namespace BPMS.BuildingBlocks.Application.Configuration.Commands;

public class CommandBase<TResult> : ICommand<TResult>
{
    public CorrelationId CorrelationId { get; protected set; } = CorrelationId.New();
}