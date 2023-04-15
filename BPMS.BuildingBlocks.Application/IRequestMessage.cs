using MediatR;

namespace BPMS.BuildingBlocks.Application;

public interface IRequestMessage : IBaseRequest
{
}
public interface IRequestMessage<out TResult> : IRequestMessage, IRequest<TResult>
{

}