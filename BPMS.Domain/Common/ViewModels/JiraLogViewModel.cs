namespace BPMS.Domain.Common.ViewModels;

public class JiraLogViewModel
{
    public JiraLogViewModel()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public DateTime RegisterDate { get; set; }
    public string RequestUrl { get; set; }
    public string RequestData { get; set; }
    public string ResponseData { get; set; }
}