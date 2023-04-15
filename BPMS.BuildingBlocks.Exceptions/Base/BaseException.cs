namespace BPMS.BuildingBlocks.Exceptions.Base;

public abstract class BaseException : ApplicationException, IBaseException
{
    public int StatusCode { get; protected set; }

    protected abstract void SetStatusCode();

    public BaseException()
    {
        SetStatusCode();
    }

    public BaseException(string message) : base(message)
    {
        SetStatusCode();
    }
}
public interface IBaseException
{
    int StatusCode { get; }
}