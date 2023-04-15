using BPMS.Domain.Common.Dtos.Jira;
using BPMS.Domain.Common.ViewModels;
using BPMS.Infrastructure;
using BPMS.Infrastructure.Repositories;
using BPMS.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace BPMS.Application.Services;

public class JiraService : IJiraService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly IUnitOfWork _unitOfWork;
    private IJiraLogRepository _jiraLogRepository;
    private readonly IConfiguration _configuration;

    public JiraService(HttpClient httpClient, IUnitOfWork unitOfWork, IJiraLogRepository jiraLogRepository, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _unitOfWork = unitOfWork;
        _jiraLogRepository = jiraLogRepository;
        _configuration = configuration;
        _baseUrl = (configuration["Common:JiraBaseUrl"] ?? "") + "/rest/api/2/";
    }

    public async Task<ResponseCreateIssueDTO> CreateIssue(CreateIssueInputDTO createIssueDTO, string webRootPath)
    {

        ResponseCreateIssueDTO responseCreate = new ResponseCreateIssueDTO();
        SuccessResponseDTO successResponseDTO = new SuccessResponseDTO();
        CreateIssueOutputDTO createIssueOutputDTO = new CreateIssueOutputDTO();
        FileDTO fileDTO = new FileDTO();
        SearchDTO searchDTO = new SearchDTO();

        responseCreate.Message = "تسک در جیرا ثبت نشد.";
        string taskNumber = "";
        string response = "";

        long bpmsNo = GetBpmsNo(createIssueDTO);

        var username = _configuration["Common:JiraUsername"];
        var password = _configuration["Common:JiraPassword"];
        var authenticationString = $"{username}:{password}";
        var base64AuthHeader = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64AuthHeader);

        string JiraBpmsNo = _configuration["Common:JiraBpmsNo"];

        string url = _baseUrl + "search?jql=cf[" + JiraBpmsNo + "]=" + bpmsNo + "";
        var res = await _httpClient.GetAsync(url);

        var strResponse = await res.Content.ReadAsStringAsync();
        response = strResponse;

        if (res.StatusCode == HttpStatusCode.OK)
            searchDTO = JsonConvert.DeserializeObject<SearchDTO>(response);

        AddJiraLog(url, null, response, createIssueDTO.userName);

        string key = "";

        if (searchDTO.total == 0)
        {
            createIssueOutputDTO = await CreateIssueRequest(createIssueDTO.forms, createIssueDTO.userName);
            response = createIssueOutputDTO.response;
        }
        else
        {
            key = searchDTO.issues[0].key;
            responseCreate.Message = $"تسک بدلیل تکراری بودن ثبت نشد. شماره تسک:{key}";
            responseCreate.Success = false;
            return responseCreate;
        }

        if (createIssueOutputDTO.success)
        {
            successResponseDTO = JsonConvert.DeserializeObject<SuccessResponseDTO>(response);
            taskNumber = successResponseDTO.key;
            responseCreate.Message = taskNumber;
            responseCreate.Success = true;
        }
        else
        {

            string[] arr = response.Split(':');
            response = arr[3];
            arr = response.Split('\"');
            response = arr[1];
            responseCreate.Message = response;
        }
        List<FileDTO> list = new List<FileDTO>();
        list = CreateFile(bpmsNo, webRootPath);

        if (!string.IsNullOrEmpty(taskNumber) && list.Count > 0)
        {
            responseCreate = await CreateAttachmentRequest(list, taskNumber, createIssueDTO.userName);
        }

        return responseCreate;
    }

    public async Task<ResponseCreateIssueDTO> CreateIssueAgain(CreateIssueInputDTO createIssueDTO, string webRootPath)
    {
        ResponseCreateIssueDTO responseCreate = new ResponseCreateIssueDTO();
        SuccessResponseDTO successResponseDTO = new SuccessResponseDTO();
        CreateIssueOutputDTO createIssueOutputDTO = new CreateIssueOutputDTO();
        FileDTO fileDTO = new FileDTO();

        responseCreate.Message = "تسک در جیرا ثبت نشد.";
        string taskNumber = "";
        string response = "";

        long bpmsNo = GetBpmsNo(createIssueDTO);

        var username = _configuration["Common:JiraUsername"];
        var password = _configuration["Common:JiraPassword"];
        var authenticationString = $"{username}:{password}";
        var base64AuthHeader = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64AuthHeader);

        createIssueOutputDTO = await CreateIssueRequest(createIssueDTO.forms, createIssueDTO.userName);
        response = createIssueOutputDTO.response;

        if (createIssueOutputDTO.success)
        {
            successResponseDTO = JsonConvert.DeserializeObject<SuccessResponseDTO>(response);
            taskNumber = successResponseDTO.key;
            responseCreate.Message = taskNumber;
            responseCreate.Success = true;
        }
        else
        {

            string[] arr = response.Split(':');
            response = arr[3];
            arr = response.Split('\"');
            response = arr[1];
            responseCreate.Message = response;
        }
        List<FileDTO> list = new List<FileDTO>();
        list = CreateFile(bpmsNo, webRootPath);

        if (!string.IsNullOrEmpty(taskNumber) && list.Count > 0)
        {
            responseCreate = await CreateAttachmentRequest(list, taskNumber, createIssueDTO.userName);
        }

        return responseCreate;
    }


    private async Task<CreateIssueOutputDTO> CreateIssueRequest(NameValueCollection model, string userName)
    {
        string response = "";
        var strResponse = "";
        string url = _baseUrl + "issue";

        CreateIssueOutputDTO createIssueOutputDTO = new CreateIssueOutputDTO();
        List<CustomFieldDTO> customFieldDTO = new List<CustomFieldDTO>();

        var dict = NvcToDictionary(model, false);

        string urlGetFieldType = _baseUrl + "field";
        var res = await _httpClient.GetAsync(urlGetFieldType);

        strResponse = await res.Content.ReadAsStringAsync();
        response = strResponse;

        AddJiraLog(urlGetFieldType, null, response, userName);


        if (res.StatusCode == HttpStatusCode.OK)
            customFieldDTO = JsonConvert.DeserializeObject<List<CustomFieldDTO>>(response);

        string jsonData = CreateDataJson(dict, customFieldDTO);

        var content = new StringContent(jsonData, Encoding.UTF8, "Application/json");
        var partyResponse = await _httpClient.PostAsync(url, content);

        if (partyResponse.IsSuccessStatusCode)
            createIssueOutputDTO.success = true;

        strResponse = await partyResponse.Content.ReadAsStringAsync();
        response = strResponse;

        AddJiraLog(url, jsonData, response, userName);

        createIssueOutputDTO.response = response;

        return createIssueOutputDTO;
    }

    private async Task<ResponseCreateIssueDTO> CreateAttachmentRequest(List<FileDTO> fileDTOs, string taskNumber, string userName)
    {
        string response = "";
        var strResponse = "";
        string requestData = "";
        List<string> listFile = new List<string>();

        ResponseCreateIssueDTO responseCreate = new ResponseCreateIssueDTO();

        responseCreate.Success = true;
        responseCreate.Message = $"{taskNumber} فایل انتخابی در تسک آپلود نشد. شماره تسک";
        string url = $"{_baseUrl}issue/{taskNumber}/attachments";

        _httpClient.DefaultRequestHeaders.Add("X-Atlassian-Token", "no-check");
        MultipartFormDataContent multiPartContent = new MultipartFormDataContent("-data-");

        for (int i = 0; i < fileDTOs.Count; i++)
        {
            byte[] myFiles = fileDTOs[i].file;
            ByteArrayContent byteArrayContent;
            byteArrayContent = new ByteArrayContent(myFiles);
            multiPartContent.Add(byteArrayContent, "file", fileDTOs[i].fileName);
            listFile.Add(fileDTOs[i].fileName);
        }

        var requestAttachment = await _httpClient.PostAsync(url, multiPartContent);
        strResponse = await requestAttachment.Content.ReadAsStringAsync();
        response = strResponse;

        OutputJson json = new OutputJson
        {
            fileNames = listFile
        };
        requestData = JsonConvert.SerializeObject(json);
        AddJiraLog(url, requestData, response, userName);

        if (requestAttachment.IsSuccessStatusCode)
            responseCreate.Message = taskNumber;
        else
            responseCreate.Success = false;

        return responseCreate;
    }

    private Dictionary<string, object> NvcToDictionary(NameValueCollection nvc, bool handleMultipleValuesPerKey)
    {
        var result = new Dictionary<string, object>();

        foreach (string key in nvc.Keys)
        {
            if (handleMultipleValuesPerKey)
            {
                string[] values = nvc.GetValues(key);
                if (values.Length == 1)
                {
                    result.Add(key, values[0]);
                }
                else
                {
                    result.Add(key, values);
                }
            }
            else
            {
                result.Add(key, nvc[key]);
            }
        }

        return result;
    }

    private List<FileDTO> CreateFile(long bpmsNo, string webRootPath)
    {
        FileDTO fileDTO = new FileDTO();
        List<FileDTO> list = new List<FileDTO>();
        var request = _unitOfWork.Request.GetRequestsByRequestNo(bpmsNo);
        var requestId = request.Id;
        string fileName = "";

        var path = Path.Combine(webRootPath, "Images/upload/forms/" + requestId + "/");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        var files = Directory.GetFiles(path);

        foreach (var file in files)
        {
            fileName = Path.GetFileName(file);
            if (fileName != "")
            {
                path = path + fileName;
                byte[] myFiles = System.IO.File.ReadAllBytes(path);
                fileDTO.file = myFiles;
                fileDTO.fileName = fileName;
                list.Add(fileDTO);
            }
        }

        return list;
    }

    private long GetBpmsNo(CreateIssueInputDTO createIssueDTO)
    {
        string bpmsNo = "";
        string JiraBpmsNo = _configuration["Common:JiraBpmsNo"];
        bpmsNo = createIssueDTO.forms["customfield_" + JiraBpmsNo];
        long bpmsNumber = Int32.Parse(bpmsNo);

        return bpmsNumber;
    }

    private string CreateDataJson(Dictionary<string, object> dict, List<CustomFieldDTO> customFieldDTO)
    {

        string myJson = "{\"fields\":{";

        for (int i = 0; i < dict.Count; i++)
        {
            string key = dict.Keys.ElementAt(i);
            object value = dict.Values.ElementAt(i);

            foreach (var item in customFieldDTO)
            {
                if (item.id == key)
                {
                    if (i != 0)
                    {
                        myJson += ", ";
                    }

                    myJson += $"\"{key}\" : ";
                    if (key == "issuetype" || key == "project" || key == "priority")
                    {
                        myJson += ("{" + "\"id\"" + ":\"" + value + "\"}");
                    }
                    else
                    {
                        if (item.schema.type == "string" || item.schema.type == "datetime")
                        {
                            myJson += $"\"{value}\"";
                        }
                        else if (item.schema.type == "number")
                        {
                            myJson += value;
                        }
                    }

                    break;
                }
            }
        }

        myJson += "}}";

        return myJson;
    }

    private void AddJiraLog(string requestUrl, string requestData, string responseData, string userName)
    {
        var jiraLog = new JiraLogViewModel()
        {
            UserId = _unitOfWork.Users.Single(c => c.UserName == userName).Id,
            UserName = userName,
            RegisterDate = DateTime.UtcNow,
            RequestUrl = requestUrl,
            RequestData = requestData,
            ResponseData = responseData
        };

        _jiraLogRepository.AddJiraLog(jiraLog);
    }

    public IEnumerable<JiraLogsViewModel> GetJiraLogsList()
    {

        return _jiraLogRepository.GetJiraLogsList();

    }

    public JiraLogDetailViewModel GetJiraLogById(Guid Id)
    {
        return _jiraLogRepository.GetJiraLogById(Id);
    }

}