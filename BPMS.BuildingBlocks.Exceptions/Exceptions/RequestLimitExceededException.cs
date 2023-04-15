using BPMS.BuildingBlocks.Exceptions.Base;

namespace BPMS.BuildingBlocks.Exceptions.Exceptions;

public class RequestLimitExceededException : BaseException
{
    public RequestLimitExceededException()
    {

    }
    public RequestLimitExceededException(string message)
        : base(message)
    {

    }
    protected override void SetStatusCode()
    {
        StatusCode = 429;// too many requests
    }
}