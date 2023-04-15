using BPMS.BuildingBlocks.Exceptions.Base;

namespace BPMS.BuildingBlocks.Exceptions.Exceptions;

public class UserNotFoundException : BaseException
{
    public UserNotFoundException()
    {

    }
    public UserNotFoundException(string message)
        : base(message)
    {

    }
    protected override void SetStatusCode()
    {
        StatusCode = 404;//NotFound
    }
}