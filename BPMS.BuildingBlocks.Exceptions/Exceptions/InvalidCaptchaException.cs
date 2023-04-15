using BPMS.BuildingBlocks.Exceptions.Base;

namespace BPMS.BuildingBlocks.Exceptions.Exceptions;

public class InvalidCaptchaException : BaseException
{
    public InvalidCaptchaException() : this("کد امنیتی بدرستی وارد نشده است")
    {

    }
    public InvalidCaptchaException(string message)
        : base(message)
    {

    }
    protected override void SetStatusCode()
    {
        StatusCode = 417;//ExpectationFailed
    }
}