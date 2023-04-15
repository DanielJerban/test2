using BPMS.BuildingBlocks.Domain;

namespace BPMS.BuildingBlocks.Application.Configuration.Commands;

public interface ICommand : IRequestMessage
{
    CorrelationId CorrelationId { get; }
}
public interface ICommand<out TResult> : ICommand, IRequestMessage<TResult>
{
}