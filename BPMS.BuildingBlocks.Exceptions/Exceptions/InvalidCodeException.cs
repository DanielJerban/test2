using BPMS.BuildingBlocks.Exceptions.Base;

namespace BPMS.BuildingBlocks.Exceptions.Exceptions;

public class InvalidCodeException : BaseException
{
    public InvalidCodeException()
    {

    }
    public InvalidCodeException(string message)
        : base(message)
    {

    }
    protected override void SetStatusCode()
    {
        StatusCode = 401;//Unauthorized
    }
}