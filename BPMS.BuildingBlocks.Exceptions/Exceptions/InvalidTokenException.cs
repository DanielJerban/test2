using BPMS.BuildingBlocks.Exceptions.Base;

namespace BPMS.BuildingBlocks.Exceptions.Exceptions;

public class InvalidTokenException : BaseException
{
    public InvalidTokenException()
    {

    }
    public InvalidTokenException(string message)
        : base(message)
    {

    }
    protected override void SetStatusCode()
    {
        StatusCode = 401;//Unauthorized
    }
}