using BPMS.Domain.Common.Dtos;
using BPMS.Infrastructure.Services;
using Newtonsoft.Json;
using RestSharp;

namespace BPMS.Application.Services;

public class AutomationService : IAutomationService
{
    public List<LookUpPhpDto> GetLookUpsFromAutomation(string type, string baseUrl)
    {
        string url = $"{baseUrl}";
            
        switch (type)
        {
            case "ChartLevel": url += "/api/chartrank"; break;
            case "OrganizationPostType": url += "/api/dignity"; break;
            case "OrganizationPostTitle": url += "/api/position"; break;
        }

        var client = new RestClient(url);
        var request = new RestRequest(new Uri(url));
        var response = client.Execute(request);

        return JsonConvert.DeserializeObject<List<LookUpPhpDto>>(response.Content);
    }
}