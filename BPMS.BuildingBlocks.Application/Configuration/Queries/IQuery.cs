namespace BPMS.BuildingBlocks.Application.Configuration.Queries;

public interface IQuery<out TResult> : IRequestMessage, IRequestMessage<TResult>
{
}