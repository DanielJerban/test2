using BPMS.BuildingBlocks.Exceptions.Base;

namespace BPMS.BuildingBlocks.Exceptions.Exceptions;

public class AuthTokenExpiredException : BaseException
{
    public AuthTokenExpiredException()
    {

    }
    public AuthTokenExpiredException(string message)
        : base(message)
    {

    }
    protected override void SetStatusCode()
    {
        StatusCode = 400;//bad request
    }
}