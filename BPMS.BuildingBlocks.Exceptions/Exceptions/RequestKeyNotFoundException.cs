using BPMS.BuildingBlocks.Exceptions.Base;

namespace BPMS.BuildingBlocks.Exceptions.Exceptions;

public class RequestKeyNotFoundException : BaseException
{
    public RequestKeyNotFoundException()
    {

    }
    public RequestKeyNotFoundException(string message)
        : base(message)
    {

    }
    protected override void SetStatusCode()
    {
        StatusCode = 404;//NotFound
    }
}