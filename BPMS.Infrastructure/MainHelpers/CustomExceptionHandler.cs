namespace BPMS.Infrastructure.MainHelpers;

public class CustomExceptionHandler
{
    // TODO: U may need to uncomment later 
    // private readonly IErrorLogService _errorLogService;
    // public CustomExceptionHandler(IErrorLogService errorLogService)
    // {
        // _errorLogService = errorLogService;
    // }

    // TODO: Update to user elastic search for error handling 
    public virtual string HandleException(Exception ex, string model = null)
    {
        try
        {
            ex.Data.Add("User Message", model);

            //ErrorSignal.FromCurrentContext().Raise(ex);
            // int sequence = _errorLogService.GetLastLog();
            return $"خطایی پیش آمده است. شماره خطا: {/*sequence*/1} لطفا با پشتیبانی BPMS تماس بگیرید";
        }
        catch (Exception)
        {
            // ignored
        }

        return ex.Message;
    }

    public void AddCustomErrorInElmahByMessage(string errorMessage)
    {
        HandleException(new Exception(errorMessage));
    }
}