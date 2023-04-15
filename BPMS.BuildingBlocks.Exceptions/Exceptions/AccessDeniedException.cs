using BPMS.BuildingBlocks.Exceptions.Base;

namespace BPMS.BuildingBlocks.Exceptions.Exceptions;

public class AccessDeniedException : BaseException
{
    public AccessDeniedException()
    {

    }
    public AccessDeniedException(string message)
        : base(message)
    {

    }
    protected override void SetStatusCode()
    {
        StatusCode = 401;//unathorized
    }
}