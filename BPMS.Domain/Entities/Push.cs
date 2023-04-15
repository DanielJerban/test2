using System.Collections;
using Newtonsoft.Json;

namespace BPMS.Domain.Entities;

[JsonObject]
public class Push
{
    [JsonProperty("notification_target")]
    public Target Target { get; set; }

    [JsonProperty("notification_content")]
    public Content Content { get; set; }
}

[JsonObject]
public class Content
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("body")]
    public string Body { get; set; }

    [JsonProperty("custom_data")]
    public IDictionary<string, string> Payload { get; set; }
}

[JsonObject]
public class Target
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("devices")]
    public IEnumerable Devices { get; set; }
}

public class Constants
{
    public const string Url = "https://api.appcenter.ms/v0.1/apps/";
    public const string ApiKeyName = "X-API-Token";
    public const string ApiKey = "{Your App Center API Token}";
    public const string Organization = "{Your organization name}";
    public const string Android = "{Your Android App Name}";
    public const string IOS = "{Your iOS App Name}";
    public const string DeviceTarget = "devices_target";
    public class Apis { public const string Notification = "push/notifications"; }
}