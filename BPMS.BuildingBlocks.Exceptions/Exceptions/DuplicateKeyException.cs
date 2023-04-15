using BPMS.BuildingBlocks.Exceptions.Base;

namespace BPMS.BuildingBlocks.Exceptions.Exceptions;

public class DuplicateKeyException : BaseException
{
    public DuplicateKeyException()
    {

    }
    public DuplicateKeyException(string message)
        : base(message)
    {

    }
    protected override void SetStatusCode()
    {
        StatusCode = 409;//Conflict
    }
}