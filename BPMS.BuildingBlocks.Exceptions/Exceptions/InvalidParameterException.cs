using BPMS.BuildingBlocks.Exceptions.Base;

namespace BPMS.BuildingBlocks.Exceptions.Exceptions;

public class InvalidParameterException : BaseException
{
    public InvalidParameterException()
    {

    }
    public InvalidParameterException(string message)
        : base(message)
    {

    }
    protected override void SetStatusCode()
    {
        StatusCode = 400;//BadRequest
    }
}