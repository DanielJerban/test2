using Newtonsoft.Json;

namespace BPMS.Domain.Common.Dtos.Recaptcha;

public class CaptchaResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("error-codes")]
    public List<string> ErrorMessage { get; set; }
}