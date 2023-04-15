using BPMS.BuildingBlocks.Exceptions.Base;

namespace BPMS.BuildingBlocks.Exceptions.Exceptions;

public class CredentialNotFoundException : BaseException
{
    public CredentialNotFoundException()
    {

    }
    public CredentialNotFoundException(string message)
        : base(message)
    {

    }
    protected override void SetStatusCode()
    {
        StatusCode = 401;//unauthorized
    }
}