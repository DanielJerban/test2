using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Enums;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;
using System.Text;

namespace BPMS.Application.Repositories;

public class ExternalApiRepository : Repository<ExternalApi>, IExternalApiRepository
{
    public BpmsDbContext DbContext => Context;
    public ExternalApiRepository(BpmsDbContext context) : base(context)
    {
    }

    public SystemApiResultViewModel TestApi(ExternalApiViewModel model, string webRootPath, bool useInServiceTask = false)
    {
        var formDataFileTextObj = JsonConvert.DeserializeObject<List<FormDataViewModel>>(model.FormDataFileText);
        var formDataTextObj = JsonConvert.DeserializeObject<List<FormDataViewModel>>(model.FormDataText);

        var client = new RestClient(model.Url);
        var request = new RestRequest();

        switch (model.AuthorizationType)
        {
            case AuthorizationType.BearerToken:
                if (string.IsNullOrWhiteSpace(model.Token))
                {
                    throw new ArgumentException("هیچ توکنی وارد نشده است.");
                }
                request.AddHeader("Authorization", $"Bearer {model.Token}");
                break;
            case AuthorizationType.BasicAuth:
                if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
                {
                    throw new ArgumentException("نام کاربری و کلمه عبور نمی تواد خالی باشد");
                }
                var userPassByteArray = Encoding.ASCII.GetBytes(model.Username + ":" + model.Password);
                var base64UserPass = Convert.ToBase64String(userPassByteArray);
                request.AddHeader("Authorization", $"Basic {base64UserPass}");
                break;
            case AuthorizationType.NoAuth:
                break;
        }

        switch (model.ActionType)
        {
            case ActionType.Get:
                request.Method = Method.Get;
                break;
            case ActionType.Post:
                request.Method = Method.Post;
                break;
            case ActionType.Put:
                request.Method = Method.Put;
                break;
            case ActionType.Delete:
                request.Method = Method.Delete;
                break;
        }

        switch (model.ContentType)
        {
            case Domain.Common.Enums.ContentTypeEnum.Json:
                if (string.IsNullOrWhiteSpace(model.Content) && (model.ActionType == ActionType.Post || model.ActionType == ActionType.Put))
                {
                    throw new ArgumentException("ورودی درخواست خالی می باشد.");
                }

                if (!string.IsNullOrWhiteSpace(model.Content))
                {
                    if (!HelperBs.IsValidJson(model.Content))
                    {
                        throw new ArgumentException("فرمت ورودی باید به صورت JSON باشد.");
                    }
                }

                request.AddHeader("Content-Type", "application/json; charset=utf-8");
                request.AddParameter("application/json", model.Content, ParameterType.RequestBody);

                break;
            case ContentTypeEnum.FormData:

                if (model.FormDataFiles.Count < 1 && !formDataFileTextObj.Any())
                {
                    request.AlwaysMultipartFormData = true;
                }

                /* 1. add file
                 * 2. add files from directory
                 * 3. add text key value pairs 
                 */
                foreach (var formDataFile in model.FormDataFiles)
                {
                    using var ms = new MemoryStream();
                    formDataFile.File.CopyTo(ms);
                    byte[] fileBytes = ms.ToArray();
                    string s = Convert.ToBase64String(fileBytes);
                    request.AddFile(formDataFile.FileKey, fileBytes, formDataFile.File.FileName);
                }

                foreach (var item in formDataFileTextObj)
                {
                    // Get file from directory 
                    string path = Path.Combine(webRootPath, "Images/upload/extApiFormData/" + model.Id + "/");
                    var directoryFileFullPath = Directory.GetFiles(path).FirstOrDefault(c => c.Contains(item.Value.ToString()));
                    var thisFile = File.ReadAllBytes(directoryFileFullPath ?? throw new ArgumentException("هیچ فایلی با این عنوان یافت نشد."));
                    request.AddFile(item.Key, thisFile, item.Value.ToString());
                    //var x = directoryFiles.Where(c => c.Equals(item.Value));
                }
                foreach (var item in formDataTextObj)
                {
                    request.AddParameter(item.Key, item.Value, ParameterType.RequestBody);
                }

                break;
        }

        if (model.Headers != null)
        {
            foreach (var header in model.Headers)
            {
                request.AddHeader(header.Key, header.Value);
            }
        }

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        try
        {
            var result = client.Execute(request);
            if (!result.IsSuccessful && !useInServiceTask)
            {
                throw new ArgumentException(result.StatusDescription + "<br/>" + JsonConvert.DeserializeObject(result.Content));
            }

            return new SystemApiResultViewModel()
            {
                Content = ExecuteRequest(result, model.UseInGrid),
                ErrorMessage = result.ErrorMessage,
                Success = result.IsSuccessful,
                StatusCode = result.StatusCode,
                UseInGrid = model.UseInGrid
            };
        }
        catch
        {
            throw new Exception("ارتباط با اینترنت قطع می باشد");
        }
    }

    public SystemApiResultViewModel TestApiById(Guid externalApiId, dynamic work, string webRootPath, bool useInServiceTask = false)
    {
        var extApi = Context.ExternalApis.AsNoTracking().SingleOrDefault(c => c.Id == externalApiId);
        if (extApi == null)
        {
            throw new ArgumentException("هیچ API ای با این شناسه یافت نشد.");
        }

        ReplaceDynamicFields(extApi, work);

        var model = new ExternalApiViewModel()
        {
            Id = extApi.Id,
            Content = extApi.Content,
            Token = extApi.Token,
            Url = extApi.Url,
            Title = extApi.Title,
            UseInGrid = extApi.UseInGrid,
            AuthorizationType = extApi.AuthorizationType,
            AuthorizationTypeString = extApi.AuthorizationType.ToDisplay(),
            Username = extApi.Username,
            ActionType = extApi.ActionType,
            ActionTypeString = extApi.ActionType.ToDisplay(),
            Password = extApi.Password,
            ContentType = extApi.ContentType,
            ContentTypeString = extApi.ContentType.ToDisplay(),
            ResponseStructute = extApi.ResponseStructute,
            Headers = JsonConvert.DeserializeObject<List<ExternalApiHeaderDto>>(extApi.Headers)
        };

        switch (extApi.ContentType)
        {
            case Domain.Common.Enums.ContentTypeEnum.Json:
                model.FormDataFileText = "[]";
                model.FormDataText = "[]";
                break;
            case Domain.Common.Enums.ContentTypeEnum.FormData:
                var contetDes = JsonConvert.DeserializeObject<List<FormDataViewModel>>(model.Content);
                List<FormDataViewModel> formDataTextObj = new List<FormDataViewModel>();
                List<FormDataViewModel> formDataFiileTextObj = new List<FormDataViewModel>();
                foreach (var item in contetDes)
                {
                    if (item.ParamType == ParamType.Text)
                    {
                        formDataTextObj.Add(item);
                    }
                    else if (item.ParamType == ParamType.File)
                    {
                        formDataFiileTextObj.Add(item);
                    }
                }

                model.FormDataText = JsonConvert.SerializeObject(formDataTextObj);
                model.FormDataFileText = JsonConvert.SerializeObject(formDataFiileTextObj);

                break;
        }


        return TestApi(model, webRootPath, useInServiceTask);
    }

    public IEnumerable<ExternalApiViewModel> GetTableData()
    {
        var data = DbContext.ExternalApis.Select(c => new ExternalApiViewModel()
        {
            Id = c.Id,
            Content = c.Content,
            Token = c.Token,
            Url = c.Url,
            Title = c.Title,
            UseInGrid = c.UseInGrid,
            AuthorizationType = c.AuthorizationType,
            AuthorizationTypeString = c.AuthorizationType.ToString(),
            Username = c.Username,
            ActionType = c.ActionType,
            ActionTypeString = c.ActionType.ToString(),
            Password = c.Password,
            ContentType = c.ContentType,
            ContentTypeString = c.ContentType.ToString(),
            ResponseStructute = c.ResponseStructute,
            HeadersJson = c.Headers
        });

        return data;
    }

    public List<DropdownViewModel<Guid>> GetAllExternalApiAsDropdown()
    {
        return Context.ExternalApis.Select(c => new DropdownViewModel<Guid>()
        {
            Id = c.Id,
            Text = c.Title
        }).OrderBy(c => c.Text).ToList();
    }

    public string ExecuteRequest(RestResponse result, bool useInGrid)
    {
        string content;
        //if (!result.IsSuccessful)
        //{
        //    throw new ArgumentException(result.ErrorMessage);
        //}

        if (string.IsNullOrWhiteSpace(result.Content))
        {
            return null;
        }

        if (useInGrid)
        {
            try
            {
                JArray dataArr = new JArray();
                string data = "";
                var jObj = JObject.Parse(result.Content);
                Dictionary<string, object> jObjKeyCaseinsensitive = new Dictionary<string, object>(jObj.ToObject<IDictionary<string, object>>(), StringComparer.CurrentCultureIgnoreCase);

                if (jObj["data"] != null || jObj["Data"] != null)
                {
                    data = jObjKeyCaseinsensitive["data"].ToString();
                }
                else
                {
                    data = "[" + jObj.ToString() + "]";
                }

                dataArr = JArray.Parse(data);

                var column = new JArray();
                foreach (var item in dataArr.First)
                {
                    var prop = item.GetType().GetProperty("Name");
                    if (prop == null) continue;
                    var name = prop.GetValue(item, null).ToString();
                    column.Add(new JObject() { { "field", name }, { "title", name.Replace("_", " ") } });
                }

                var finalResult = new JObject { { "columns", column }, { "data", dataArr } };
                content = finalResult.ToString();
            }
            catch (Exception)
            {
                throw new ArgumentException("امکان استفاده از این api در جدول نمی باشد.");
            }
        }
        else
        {
            content = result.Content;
        }

        return content;
    }

    private void ReplaceDynamicFields(ExternalApi extApi, dynamic work)
    {
        string start = "[";
        string end = "]";

        string url = extApi.Url;
        var formData = JObject.Parse(Convert.ToString(work));

        if (url.Contains(start) && url.Contains(end))
        {
            var urlDataSplit = url.Split('[');
            foreach (var str in urlDataSplit)
            {
                if (!str.Contains(end))
                {
                    continue;
                }
                var endIndex = str.IndexOf(']');
                var key = str.Substring(0, endIndex);
                var formValue = formData.GetValue(key).ToString();
                url = url.Replace(key, formValue);
            }

            url = url.Replace(start, "").Replace(end, "");
        }

        extApi.Url = url;

        if (extApi.Content != null && !string.IsNullOrEmpty(extApi.Content))
        {
            var content = JObject.Parse(extApi.Content);

            var newContent = (dynamic)content;

            foreach (var item in content)
            {
                if (item.Value.ToString().StartsWith(start))
                {
                    var value = item.Value.ToString().Substring(1, item.Value.ToString().IndexOf(end, StringComparison.Ordinal) - 1);

                    var formValue = formData.GetValue(value).ToString();

                    if (formValue != null)
                        newContent[item.Key] = formValue;
                }
            }

            extApi.Content = newContent.ToString();
        }

        if (extApi.Headers != null && !string.IsNullOrEmpty(extApi.Headers))
        {
            var headers = JsonConvert.DeserializeObject<List<ExternalApiHeaderDto>>(extApi.Headers);
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    string str = header.Value;
                    if (!str.Contains(start)) continue;

                    var splitStr = str.Split('[');
                    foreach (var s in splitStr)
                    {
                        if (!s.Contains(end)) continue;
                        var endIndex = s.IndexOf(']');
                        var key = s.Substring(0, endIndex);
                        var formValue = formData.GetValue(key).ToString();
                        header.Value = header.Value.Replace("[" + key + "]", formValue);
                    }
                }

                extApi.Headers = JsonConvert.SerializeObject(headers);
            }
        }
    }
}