using BPMS.BuildingBlocks.Exceptions.Base;

namespace BPMS.BuildingBlocks.Exceptions.Exceptions;

public class EntityNotFoundException : BaseException
{
    public EntityNotFoundException()
    {

    }
    public EntityNotFoundException(string message)
        : base(message)
    {

    }
    protected override void SetStatusCode()
    {
        StatusCode = 404;//not found
    }
}