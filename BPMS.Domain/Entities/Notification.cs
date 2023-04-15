using Newtonsoft.Json;

namespace BPMS.Domain.Entities;

public class Notification
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}

public class Targets
{
    [JsonProperty("type")]
    public string Type { get; set; }//target type Const = "user_ids_target"   

    [JsonProperty("user_ids")]
    public IEnumerable<string> UserId { get; set; }
}

public class SendObjects
{
    [JsonProperty("notification_content")]
    public Notification NotificationContent { set; get; }
    [JsonProperty("notification_target")]
    public Targets NotificationTarget { set; get; }
}