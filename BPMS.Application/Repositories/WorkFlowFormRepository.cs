using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Constants.PermissionStructure;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;

namespace BPMS.Application.Repositories;

public class WorkFlowFormRepository : Repository<WorkFlowForm>, IWorkFlowFormRepository
{
    private readonly XNamespace _bpmn2 = "http://www.omg.org/spec/BPMN/20100524/MODEL";
    private readonly XNamespace _workflow = "http://magic";
    private readonly List<ExpersionViewModel> _modelDynamic = new();
    private readonly IConfiguration _configuration;
    public WorkFlowFormRepository(BpmsDbContext context, IConfiguration configuration) : base(context)
    {
        _configuration = configuration;
    }

    public BpmsDbContext DbContext => Context;

    public List<PreviousFormViewModel> GetPreviousForm(string jsonFile, Flow flow)
    {
        var list = new List<PreviousFormViewModel>();
        var encoding = new UnicodeEncoding();
        var json = HttpUtility.UrlDecode(jsonFile, System.Text.Encoding.UTF8);

        var jobj = JObject.Parse(json);

        var fileds = jobj["fields"];
        foreach (JToken filed in fileds)
        {
            var fli = filed.First.ToObject<JObject>();
            var type = (string)fli["meta"]["id"];
            if (type == "loadform")
            {
                var value = (string)fli["attrs"]["value"];
                var formid = Guid.Parse(value);
                var frm = DbContext.WorkFlowForms.Find(formid);
                //string val = null;
                //if (flow != null)
                //{
                //    val = GetValueFromFormIfIsFill(flow, formid);
                //}

                var j = encoding.GetString(frm.Content);
                var encoodeJson = HelperBs.EncodeUri(j);
                list.Add(new PreviousFormViewModel()
                {
                    FormId = formid.ToString(),
                    JsonForm = encoodeJson
                    //ValueForm = val
                });
                var o = GetPreviousForm(j, flow);
                list.AddRange(o);
            }
        }

        return list;
    }

    public Guid CreateNewFormOrUpdate(WorkFlowFormViewModel model, string username)
    {
        var currentUser = DbContext.Users.Single(c => c.UserName == username);

        var content = HttpUtility.UrlDecode(model.JsonFile, Encoding.UTF8);
        var encoding = new UnicodeEncoding();
        byte[] bytes = encoding.GetBytes(content);

        byte[] bytesJquery = null;
        string contentJquery = "";
        if (!string.IsNullOrWhiteSpace(model.Jquery))
        {
            contentJquery = HttpUtility.UrlDecode(model.Jquery, Encoding.UTF8);
            bytesJquery = encoding.GetBytes(contentJquery);
        }

        byte[] bytesCss = null;
        string contentCss;
        var request = new Request();
        if (!string.IsNullOrWhiteSpace(model.AdditionalCssStyleCode))
        {
            contentCss = HttpUtility.UrlDecode(model.AdditionalCssStyleCode, Encoding.UTF8);
            bytesCss = encoding.GetBytes(contentCss);
        }

        var wff = DbContext.WorkFlowForms.Find(model.Id);
        if (wff == null)
        {
            DbContext.WorkFlowForms.Add(new WorkFlowForm
            {
                Id = model.Id,
                PName = model.PName,
                Content = bytes,
                Jquery = bytesJquery,
                AdditionalCssStyleCode = bytesCss,
                StaffId = currentUser.StaffId,
                OrginalVersion = model.OrginalVersion,
                SecondaryVersion = model.SecondaryVersion,
                DocumentCode = model.DocumentCode,
                RegisterDate = int.Parse(DateTime.Now.ToString("yyyyMMdd")),
                RegisterTime = DateTime.Now.ToString("HHmm")
            });

        }
        else
        {
            var flows = DbContext.Flows.Where(d => d.WorkFlowDetail.WorkFlowFormId == model.Id).ToList();
            var wfd = DbContext.WorkFlowDetails.Where(d => d.Step == 0 && d.WorkFlowFormId == model.Id).ToList();
            var hasRequst = false;
            foreach (var workFlowDetail in wfd)
            {
                request = DbContext.Requests.Where(d => d.WorkFlowId == workFlowDetail.WorkFlowId).OrderByDescending(t => t.RegisterDate).FirstOrDefault();
                if (request != null)
                {
                    hasRequst = true;
                    break;
                }
            }

            if (flows.Any() || hasRequst)
            {
                var jfile = encoding.GetString(wff.Content);
                var json = HttpUtility.UrlDecode(jfile, Encoding.UTF8);

                var jobj = JObject.Parse(json);
                var fileds = jobj["fields"];

                var contentjobj = JObject.Parse(content);
                var contentfields = contentjobj["fields"];

                foreach (JToken filed in fileds)
                {
                    var fli = filed.First.ToObject<JObject>();

                    var label = (string)fli["config"]["label"];
                    var type = (string)fli["meta"]["id"];
                    if (type == "divider") continue;
                    var flag = false;

                    foreach (var contentfield in contentfields)
                    {
                        var flicontent = contentfield.First.ToObject<JObject>();
                        if (type == "loadform")
                        {
                            var value = (string)fli["attrs"]["value"];
                            var attr = flicontent["attrs"];
                            if (attr == null) continue;
                            var r = (string)attr["value"];
                            if (value != r) continue;
                            flag = true;
                            break;
                        }
                        else
                        {
                            //var r = contentfield.ToString();
                            //if (d != r) continue;
                            var attrdb = fli["attrs"];
                            if (attrdb == null) continue;
                            var iddb = (string)attrdb["id"];
                            var attr = flicontent["attrs"];
                            if (attr == null) continue;
                            var formId = (string)attr["id"];
                            if (iddb != formId) continue;
                            flag = true;
                            break;
                        }

                    }

                    if (!flag)
                    {
                        bool checkFieldInRequest = CheckFieldInRequest(request, fli);
                        if (checkFieldInRequest)
                        {
                            throw new ArgumentException("کنترل " + label +
                                                        @" به دلیل استفاده در گردش کار قابل حذف و ویرایش نیست.");
                        }

                        if (request != null)
                        {
                            throw new ArgumentException(" امکان تغییر به علت استفاده شدن فرم وجود ندارد");
                        }
                    }
                }
            }

            wff.PName = model.PName;
            wff.OrginalVersion = model.OrginalVersion;
            wff.SecondaryVersion = model.SecondaryVersion;
            wff.DocumentCode = model.DocumentCode;
            wff.Content = bytes;
            wff.Jquery = bytesJquery;
            wff.AdditionalCssStyleCode = bytesCss;
            wff.ModifideDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
            wff.ModifideTime = DateTime.Now.ToString("HHmm");
            wff.ModifiedId = currentUser.StaffId;
        }
        return model.Id;

    }

    public IEnumerable<WorkFlowFormViewModel> GetAllWorkFlowForms()
    {
        return DbContext.WorkFlowForms.Select(w => new WorkFlowFormViewModel()
        {
            Id = w.Id,
            PName = w.PName,
            Staff = w.Staff.FName + " " + w.Staff.LName,
            OrginalVersion = w.OrginalVersion,
            SecondaryVersion = w.SecondaryVersion,
            Modifier = w.Modifier.FName + " " + w.Modifier.LName,
            RegisterDateTime = w.RegisterDate != 0 ? w.RegisterDate.ToString().Insert(4, "/").Insert(7, "/") + " " + w.RegisterTime.Insert(2, ":") : "",
            ModifiedDateTime = (w.ModifideDate != 0 && w.ModifideDate != null) ? w.ModifideDate.ToString().Insert(4, "/").Insert(7, "/") + " " + w.ModifideTime.Insert(2, ":") : "",

        }).ToList();
    }

    public IEnumerable<WorkFlowFormDto> GetWorkFlowForm()
    {
        return DbContext.WorkFlowForms.OrderBy(o => o.PName).Select(s => new WorkFlowFormDto()
        {
            Id = s.Id,
            //Content = s.Content,
            PName = s.PName,
            OrginalVersion = s.OrginalVersion,
            SecondaryVersion = s.SecondaryVersion
        });

    }
    public IEnumerable<WorkFlowFormDto> GetWorkFlowFormWithVersion()
    {
        return DbContext.WorkFlowForms.OrderBy(o => o.PName).Select(s => new WorkFlowFormDto()
        {
            Id = s.Id,
            Content = s.Content,
            PName = $"{s.PName} {s.OrginalVersion.ToString() + "," + s.SecondaryVersion.ToString()}"
        });

    }

    public List<ExpersionViewModel> GetPropertiesFromStaticForm(Guid requestTypeId)
    {
        var model = new List<ExpersionViewModel>();
        var look = DbContext.LookUps.Find(requestTypeId);
        if (look == null || string.IsNullOrWhiteSpace(look.Aux2))
        {
            throw new ArgumentException(@"در بخش اطلاعات پایه فرآیندهای سازمانی برای این فرآیند ویو مدل انتخاب نشده است.");
        }

        List<Tuple<PropertyInfo, string>> prop = null;
        try
        {
            // todo: Uncomment later 
            //prop = CompileRuntime.GetProperties(look.Aux2);
        }
        catch (Exception e)
        {
            throw new ArgumentException(@"ویو مدل (" + look.Aux2 + ") که برای این فرآیند وارد شده است، اشتباه وارد شده است.");
        }

        foreach (var propertyInfo in prop)
        {
            model.Add(new ExpersionViewModel()
            {
                Property = propertyInfo.Item2 + propertyInfo.Item1.Name,
                Type = propertyInfo.Item1.PropertyType.Name
            });
        }

        return model;
    }

    public List<ExpersionViewModel> GetPropertiesFromSingleForm(Guid formId)
    {
        AddExpression(formId.ToString());
        return _modelDynamic;
    }

    public List<ExpersionViewModel> GetPropertiesFromDynamicFormByConnectionId(string connectionId, string xml)
    {
        var uniqXml = HttpUtility.UrlDecode(xml, Encoding.UTF8);
        var myxml = XDocument.Parse(uniqXml);

        var seq = myxml.Descendants(_bpmn2 + "sequenceFlow").ToList()
            .FirstOrDefault(d => (string)d.Attribute("id") == connectionId);
        var sourceId = (string)seq.Attribute("sourceRef");

        var sourceElement = myxml.Descendants().ToList().Single(d => (string)d.Attribute("id") == sourceId);
        if (sourceElement.Name.LocalName == "task" || sourceElement.Name.LocalName == "businessRuleTask")
        {
            var wff = (string)sourceElement.Attribute(_workflow + "WorkFlowFormId");
            if (wff != "null" && wff != null) AddExpression(wff);
            if (!_modelDynamic.Any())
                throw new ArgumentException(@"قبل از نوشتن قواعد باید فرم را انتخاب کنید.");
            return _modelDynamic;
        }

        if (sourceElement.Name.LocalName.Contains("Gateway"))
        {
            TraversGatway(sourceId, myxml);
            if (!_modelDynamic.Any())
                throw new ArgumentException(@"قبل از نوشتن قواعد باید فرم را انتخاب کنید.");
            return _modelDynamic.DistinctBy(d => d.Property).ToList();
        }

        throw new ArgumentException("فرمی پیدا نشد");

    }
    public List<ExpersionViewModel> GetPropertiesFromDynamicForm(string xml, Guid? workflowId)
    {
        var uniqXml = HttpUtility.UrlDecode(xml, Encoding.UTF8);
        var myxml = XDocument.Parse(uniqXml);
        var workflowForms = myxml.Descendants().ToList().Attributes(_workflow + "WorkFlowFormId");

        foreach (var xAttribute in workflowForms.DistinctBy(d => d.Value))
        {
            var workflowformId = xAttribute.Value;
            if (workflowformId != "null") AddExpression(workflowformId);
        }
        var callProcess = myxml.Descendants().ToList().Attributes(_workflow + "WorkflowId");

        foreach (var xAttribute in callProcess.DistinctBy(d => d.Value))
        {
            var id = xAttribute.Value;
            if (id != "null") AddCallProcess(id, null);
        }

        if (workflowId.HasValue)
        {
            var current = DbContext.Workflows.Find(workflowId);
            if (current?.SubProcessId != null)
            {
                var fatherWorkflow = DbContext.Workflows.Find(current.SubProcessId);
                if (fatherWorkflow != null)
                {
                    AddFatherSubProcess(fatherWorkflow, workflowId);
                }
            }
        }

        if (!_modelDynamic.Any())
            throw new ArgumentException(@"قبل از نوشتن قواعد باید فرم را انتخاب کنید.");
        return _modelDynamic.DistinctBy(d => d.Property).ToList();

    }

    private bool CheckFieldInRequest(Request request, JToken dbfield)
    {
        if (request.Value == null)
        {
            return false;
        }

        var encoding = new UnicodeEncoding();
        var requestJfile = encoding.GetString(request.Value);
        var jsonRequest = HttpUtility.UrlDecode(requestJfile, System.Text.Encoding.UTF8);
        var jobjRequest = JObject.Parse(jsonRequest);
        var flag = false;
        foreach (KeyValuePair<string, JToken> field in jobjRequest)
        {
            var keyFild = field.Key;
            var attr = dbfield["attrs"];
            var formId = (string)attr["id"];
            if (keyFild == formId)
            {
                flag = true;
                break;
            }
        }
        return flag;
    }

    private void AddFatherSubProcess(Workflow fatherWorkflow, Guid? childWorkflow)
    {
        var encoding = new UnicodeEncoding();
        var xml = encoding.GetString(fatherWorkflow.Content);
        var uniqXml = HttpUtility.UrlDecode(xml, Encoding.UTF8);
        var myxml = XDocument.Parse(uniqXml);
        var workflowForms = myxml.Descendants().ToList().Attributes(_workflow + "WorkFlowFormId");

        foreach (var xAttribute in workflowForms.DistinctBy(d => d.Value))
        {
            var workflowformId = xAttribute.Value;
            if (workflowformId != "null") AddExpression(workflowformId);
        }
        var callProcess = myxml.Descendants().ToList().Attributes(_workflow + "WorkflowId");

        foreach (var xAttribute in callProcess.DistinctBy(d => d.Value))
        {
            var id = xAttribute.Value;
            if (id == childWorkflow.ToString()) continue;
            if (id != "null") AddCallProcess(id, fatherWorkflow.Id);
        }

        if (fatherWorkflow?.SubProcessId != null)
        {
            var father = DbContext.Workflows.Find(fatherWorkflow.SubProcessId);
            if (father != null)
            {
                AddFatherSubProcess(father, childWorkflow);
            }
        }
    }

    private void AddCallProcess(string subprocessId, Guid? currentWorkflow)
    {
        var id = Guid.Parse(subprocessId);
        var workflow = DbContext.Workflows.Find(id);
        if (workflow != null)
        {
            var encoding = new UnicodeEncoding();
            var str = encoding.GetString(workflow.Content);
            GetPropertiesFromDynamicForm(str, currentWorkflow);
        }
    }

    private void TraversGatway(string elementId, XDocument myxml)
    {
        var sequenceFlows = myxml.Descendants(_bpmn2 + "sequenceFlow")
            .Where(d => d.Attribute("targetRef").Value == elementId);
        foreach (var sequenceFlow in sequenceFlows)
        {
            var sourceId = (string)sequenceFlow.Attribute("sourceRef");
            var sourceElement = myxml.Descendants().ToList().Single(d => (string)d.Attribute("id") == sourceId);
            if (sourceElement.Name.LocalName == "task" || sourceElement.Name.LocalName == "businessRuleTask")
            {
                var wff = (string)sourceElement.Attribute(_workflow + "WorkFlowFormId");
                if (wff != "null" && wff != null) AddExpression(wff);
            }

            if (sourceElement.Name.LocalName.Contains("Gateway"))
            {
                TraversGatway(sourceId, myxml);
            }
        }


    }

    /// <summary>
    /// FormControls
    /// </summary>
    /// <param name="wff"></param>
    private void AddExpression(string wff)
    {
        var id = Guid.Parse(wff);
        var workflowform = DbContext.WorkFlowForms.Find(id);
        UnicodeEncoding encoding = new UnicodeEncoding();
        var json = encoding.GetString(workflowform.Content);

        var jobj = JObject.Parse(json);

        var fileds = jobj["fields"];

        foreach (JToken filed in fileds)
        {
            var fli = filed.First.ToObject<JObject>();
            // var isHidden = false;
            var type = (string)fli["meta"]["id"];
            //label = Regex.Replace(label, "<[^>]*>", string.Empty);
            var option = "";
            if (fli["options"] != null)
                option = fli["options"].ToString();
            var arr = new JArray();
            var label = (string)fli["config"]["label"];
            switch (type)
            {
                case "paragraph": continue;
                case "upload": continue;

                case "grid": continue;
                case "button": continue;
                case "header": continue;
                case "divider": continue;
                case "gridList": continue;
                case "gridApi": continue;
                case "loadform":
                    var value = (string)fli["attrs"]["value"];
                    AddExpression(value);
                    continue;
                case "hidden":
                    type = "String";
                    label = "";
                    break;
                case "company":
                    var comp = DbContext.Companies.ToList();
                    arr = new JArray();
                    foreach (var company in comp)
                    {
                        dynamic obj = new JObject();
                        obj.label = company.Name;
                        obj.value = company.Id;
                        arr.Add(obj);
                    }

                    type = "Select";
                    option = arr.ToString();
                    break;
                case "staff":
                    var staffs = DbContext.Staffs.ToList();
                    arr = new JArray();
                    foreach (var staff in staffs)
                    {
                        dynamic obj = new JObject();
                        obj.label = staff.FullName;
                        obj.value = staff.Id;
                        arr.Add(obj);
                    }

                    type = "Select";
                    option = arr.ToString();
                    break;
                case "client":
                    var clients = DbContext.Clients.ToList();
                    arr = new JArray();
                    foreach (var client in clients)
                    {
                        dynamic obj = new JObject();
                        obj.label = client.FName + " " + client.LName;
                        obj.value = client.Id;
                        arr.Add(obj);
                    }

                    type = "Select";
                    option = arr.ToString();
                    break;
                case "lookup":
                    var lookupId = Guid.Parse((string)fli["attrs"]["value"]);
                    var type2 = DbContext.LookUps.Find(lookupId).Aux;
                    var look = DbContext.LookUps.Where(l => l.Type == type2).ToList();
                    arr = new JArray();
                    foreach (var item in look)
                    {
                        dynamic obj = new JObject();
                        obj.label = item.Title;
                        obj.value = item.Id;
                        arr.Add(obj);
                    }

                    type = "Select";
                    option = arr.ToString();
                    break;
                case "lookup2N":
                    type = "Select";
                    var lookup2NId = Guid.Parse((string)fli["attrs"]["value"]);
                    var lookup2N = DbContext.FormLookUp2N.Find(lookup2NId);
                    var look2N = DbContext.LookUps.Where(l => l.Type == lookup2N.Type1).ToList();
                    var look2NType2 = DbContext.LookUps.Where(l => l.Type == lookup2N.Type2).ToList();
                    arr = new JArray();
                    foreach (var item in look2N)
                    {
                        dynamic obj = new JObject();
                        obj.label = item.Title;
                        obj.value = item.Id;
                        arr.Add(obj);
                    }
                    option = arr.ToString();

                    arr = new JArray();
                    foreach (var item in look2NType2)
                    {
                        dynamic obj = new JObject();
                        obj.label = item.Title;
                        obj.value = item.Id;
                        arr.Add(obj);
                    }
                    var attrid2 = (string)fli["attrs"]["id"] + "2";
                    var label2 = (string)fli["attrs"]["data-title2"];
                    _modelDynamic.Add(new ExpersionViewModel()
                    {
                        Property = attrid2,
                        Type = type,
                        Options = HelperBs.EncodeUri(arr.ToString()),
                        Label = label2 + " " + attrid2
                    });
                    break;
                case "number":
                    type = "Single";
                    break;
                case "checkbox":
                    type = "Boolean";
                    break;
                case "newCheckbox":
                    type = "Boolean";
                    label = (string)fli["attrs"]["label"];
                    break;
                case "select":
                    type = "Select";
                    break;
                case "radio":
                    type = "Select";
                    break;
                case "textarea":
                    type = "Textarea";
                    break;

                default:
                    type = "String";
                    break;
            }

            var attrid = (string)fli["attrs"]["id"];


            _modelDynamic.Add(new ExpersionViewModel()
            {
                Property = attrid,
                Type = type,
                Options = HelperBs.EncodeUri(option),
                Label = label + " " + attrid
            });
        }

    }
    public void CheckVersion(Guid formId, Guid reqTypeId, int orgVersion, int secVersion)
    {
        var editedForm = DbContext.WorkFlowForms.FirstOrDefault(r => r.Id == formId);
        var maxOrg = DbContext.WorkFlowForms.Where(r => r.Id == reqTypeId).Max(l => l.OrginalVersion);
        var maxSec = DbContext.WorkFlowForms.Where(r => r.Id == reqTypeId && r.OrginalVersion == maxOrg)
            .Max(l => l.SecondaryVersion);
        if (orgVersion < maxOrg)
        {
            throw new ArgumentException("نسخه وارد شده کوچک تر یا مساوی نسخه فعلی وارد شده است.");
        }

        var condition = editedForm == null ? secVersion <= maxSec : secVersion < maxSec;
        if (editedForm == null)
        {
            if (orgVersion == maxOrg && condition)
                throw new ArgumentException(" نسخه فرم تکراری است.");
        }
        else
        {
            if (orgVersion == maxOrg && condition)
                throw new ArgumentException(" نسخه فرم تکراری است.");
        }
    }
    public void ValidateForm(WorkFlowFormViewModel model)
    {
        if (model.PreviousId != null)
            CheckVersion(model.Id, (Guid)model.PreviousId, model.OrginalVersion, model.SecondaryVersion);
        var json = HttpUtility.UrlDecode(model.JsonFile, System.Text.Encoding.UTF8);

        var jobj = JObject.Parse(json);

        var fileds = jobj["fields"];
        var ids = new List<string>();
        var subformids = new List<string>();
        List<string> gridCulomnIds = new List<string>();

        var rex = new Regex(@"^[A-Za-z_]([A-Za-z0-9_]+)?$");
        foreach (JToken filed in fileds)
        {
            gridCulomnIds.Clear();
            var fli = filed.First.ToObject<JObject>();
            var type = (string)fli["meta"]["id"];
            var label = (string)fli["config"]["label"];
            if (string.IsNullOrWhiteSpace(label))
            {
                throw new ArgumentException(@"تمام کنترل های " + type + @" باید عنوان داشته باشند");
            }
            if (type == "grid")
            {

                var options = fli["options"];
                foreach (var option in options)
                {
                    string gridCulomnId = option["value"].ToString();
                    if (string.IsNullOrWhiteSpace(gridCulomnId))
                    {
                        throw new ArgumentException(@"شناسه ستون در کنترل " + label + @"  وارد نشده است.");
                    }
                    else
                    {
                        gridCulomnIds.Add(gridCulomnId);
                    }
                }

                var gridCulomnIdDublicated = gridCulomnIds.GroupBy(s => s).SelectMany(ab => ab.Skip(1).Take(1)).ToList();
                if (gridCulomnIdDublicated.Count > 0)
                {
                    throw new ArgumentException($"شناسه ستون {gridCulomnIdDublicated[0]} در کنترل {label} تکراری وارد شده است.");
                }
            }

            if (type == "loadform")
            {

                var value = (string)fli["attrs"]["value"];
                if (string.IsNullOrWhiteSpace(value) || value == "null")
                {
                    throw new ArgumentException(label + @" انتخاب نشده است.");
                }

                if (value == model.Id.ToString())
                {
                    throw new ArgumentException("فرم نمیتواند به عنوان زیرفرم خودش انتخاب شود");
                }

                subformids.AddRange(GetIdsFromSubForm(model, value, label));
                continue;
            }

            if (type == "divider") continue;
            if (type == "newCheckbox")
            {
                label = (string)fli["attrs"]["label"];
            };
            if (type == "lookup2N")
            {
                var label2 = (string)fli["attrs"]["data-title2"];
                if (string.IsNullOrWhiteSpace(label2))
                    throw new ArgumentException(" در کنترل " + label + " عنوان سطح دوم وارد نشده است.");
            };
            var id = (string)fli["attrs"]["id"];
            if (string.IsNullOrWhiteSpace(id))
            {

                throw new ArgumentException(@"کنترل " + label + @" باید شناسه داشته باشد");


                // throw new ArgumentException(@"تمام کنترل های " + type + @" باید شناسه داشته باشند");
            }

            if (!rex.IsMatch(id))
            {
                throw new ArgumentException(@"فرمت شناسه " + label + @" اشتباه است");
            }

            ids.Add(id.ToUpper());
        }

        var duplicated = ids.GroupBy(s => s).SelectMany(ab => ab.Skip(1).Take(1)).ToList();

        if (duplicated.Count > 0)
        {
            throw new ArgumentException(@"شناسه " + duplicated[0] + @" تکراری وارد شده است.");
        }

        ids.AddRange(subformids);
        var subformDublicated = ids.GroupBy(s => s).SelectMany(ab => ab.Skip(1).Take(1)).ToList();
        if (subformDublicated.Count > 0)
        {
            throw new ArgumentException(@"شناسه " + subformDublicated[0] + @" در زیر فرم تکراری وارد شده است.");
        }
    }

    private List<string> GetIdsFromSubForm(WorkFlowFormViewModel model, string value, string label)
    {
        var list = new List<string>();
        var formid = Guid.Parse(value);
        var form = DbContext.WorkFlowForms.Find(formid);
        UnicodeEncoding encoding = new UnicodeEncoding();
        var jsonfile = encoding.GetString(form.Content);
        var json = HttpUtility.UrlDecode(jsonfile, System.Text.Encoding.UTF8);

        var jobj = JObject.Parse(json);

        var fileds = jobj["fields"];
        foreach (JToken filed in fileds)
        {
            var fli = filed.First.ToObject<JObject>();
            var type = (string)fli["meta"]["id"];

            if (type == "loadform")
            {
                var sValue = (string)fli["attrs"]["value"];
                if (sValue == model.Id.ToString())
                {
                    throw new ArgumentException(label + @" استفاده شده در فرم خودش نقش پدر این فرم را دارد.");
                }

                list.AddRange(GetIdsFromSubForm(model, sValue, label));
                continue;
            }

            if (type == "divider") continue;
            if (type == "staff") continue;
            if (type == "company") continue;
            var id = (string)fli["attrs"]["id"];

            list.Add(id.ToUpper());

        }

        return list;
    }

    public List<PropertySubformViewModel> GetAllPropertyOfForm(Guid workFlowFormId)
    {
        var list = new List<PropertySubformViewModel>();
        var form = DbContext.WorkFlowForms.Find(workFlowFormId);
        var encoding = new UnicodeEncoding();
        var jsonFile = encoding.GetString(form.Content);
        var json = HttpUtility.UrlDecode(jsonFile, System.Text.Encoding.UTF8);

        var jobj = JObject.Parse(json);

        var fileds = jobj["fields"];
        foreach (JToken filed in fileds)
        {
            var fli = filed.First.ToObject<JObject>();

            var type = (string)fli["meta"]["id"];
            switch (type)
            {
                case "divider": continue;
                case "loadform":
                    var value = (string)fli["attrs"]["value"];
                    list.AddRange(GetPropertySubFormOfForm(Guid.Parse(value)));
                    continue;
                case "lookup2N":
                    var id2 = (string)fli["attrs"]["id"] + "2";
                    var label2 = (string)fli["attrs"]["data-title2"];
                    list.Add(new PropertySubformViewModel()
                    {
                        Property = id2,
                        Label = label2,
                        SubformName = form.PName,
                        //IsChecked = true
                    });
                    break;
            }

            var attrid = (string)fli["attrs"]["id"];
            string label;
            if (type == "newCheckbox")
                label = (string)fli["attrs"]["label"];
            else
                label = (string)fli["config"]["label"];
            list.Add(new PropertySubformViewModel()
            {
                Property = attrid,
                Label = label,
                SubformName = form.PName,
                //IsChecked = true
            });
        }

        return list;
    }
    public List<PropertySubformViewModel> GetPropertySubFormOfForm(Guid workFlowFormId)
    {
        var list = new List<PropertySubformViewModel>();
        var form = DbContext.WorkFlowForms.Find(workFlowFormId);
        var encoding = new UnicodeEncoding();
        var jsonFile = encoding.GetString(form.Content);
        var json = HttpUtility.UrlDecode(jsonFile, System.Text.Encoding.UTF8);

        var jobj = JObject.Parse(json);

        var fileds = jobj["fields"];
        foreach (JToken filed in fileds)
        {
            var fli = filed.First.ToObject<JObject>();

            var type = (string)fli["meta"]["id"];
            switch (type)
            {
                case "paragraph": continue;
                // case "hidden": continue;
                case "button": continue;
                case "header": continue;
                //   case "staff": continue;
                case "client": continue;
                case "divider": continue;
                case "staffRegister": continue;
                case "loadform":
                    var value = (string)fli["attrs"]["value"];
                    list.AddRange(GetPropertySubFormOfForm(Guid.Parse(value)));
                    continue;
                case "lookup2N":
                    var id2 = (string)fli["attrs"]["id"] + "2";
                    var label2 = (string)fli["attrs"]["data-title2"];
                    list.Add(new PropertySubformViewModel()
                    {
                        Property = id2,
                        Label = label2,
                        SubformName = form.PName,
                        //IsChecked = true
                    });
                    break;
            }

            var attrid = (string)fli["attrs"]["id"];
            string label;
            if (type == "newCheckbox")
                label = (string)fli["attrs"]["label"];
            else
                label = (string)fli["config"]["label"];
            list.Add(new PropertySubformViewModel()
            {
                Property = attrid,
                Label = label,
                SubformName = form.PName,
                //IsChecked = true
            });
        }

        return list;
    }

    public void UploadFile(List<IFormFile> files, Guid requestId, string webRootPath)
    {
        var format = DbContext.LookUps.Where(l => l.Type == "FormatType" && l.IsActive).Select(l => l.Title).ToList();

        string filePath = _configuration["Common:FilePath:FormFilePath"];

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                if (file.Length > 20 * 1024 * 1024)
                    throw new ArgumentException("حجم فایل آپلود شده نمیتوان بیش از 20 مگابایت باشد.");

                //var stream = fileContent.InputStream;
                var fileName = file.FileName;
                var extention = Path.GetExtension(fileName).Replace(".", "").ToLower().Trim();
                if (!format.Contains(extention))
                    throw new ArgumentException("فرمت فایل غیر مجاز است.");
                var targetfolder = Path.Combine(webRootPath, filePath + requestId);

                if (Directory.Exists(targetfolder) == false)
                {
                    Directory.CreateDirectory(targetfolder);
                }

                var path = Path.Combine(targetfolder, fileName);
                using Stream fileStream = new FileStream(path, FileMode.Create);
                file.CopyTo(fileStream);
            }
            else
                throw new ArgumentException("فایل مورد نظر آپلود نشد");
        }
    }

    public IEnumerable<SelectListItem> GetPropertySubFormOfForm(string content)
    {
        var list = new List<SelectListItem>();
        var encoding = new UnicodeEncoding();
        var jobj = JObject.Parse(content);

        var fileds = jobj["fields"];
        foreach (JToken filed in fileds)
        {
            var fli = filed.First.ToObject<JObject>();
            var type = (string)fli["meta"]["id"];
            switch (type)
            {

                case "divider": continue;
                case "loadform":
                    var value = (string)fli["attrs"]["value"];
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        var formid = Guid.Parse(value);
                        var form = DbContext.WorkFlowForms.Single(w => w.Id == formid);
                        var jsonFile = encoding.GetString(form.Content);
                        list.AddRange(GetPropertySubFormOfForm(jsonFile));
                    }

                    continue;
            }

            var attrid = (string)fli["attrs"]["id"];
            var label = (string)fli["config"]["label"];
            list.Add(new SelectListItem()
            {
                Value = attrid,
                Text = attrid + @"-" + label
            });
        }

        return list;
    }

    public string CheckReadonlNotChange(string model, Guid paramRequestId, List<string> readonlis)
    {
        var encoding = new UnicodeEncoding();
        var value = encoding.GetString(DbContext.Requests.Find(paramRequestId)?.Value ?? throw new InvalidOperationException());
        var orginalobj = JObject.Parse(value);
        var currentobj = JObject.Parse(model);
        foreach (var field in readonlis)
        {
            var orgValue = orginalobj[field].ToString();
            var curValue = currentobj[field].ToString();
            if (orgValue != curValue)
            {
                currentobj[field] = orginalobj[field];
            }
        }

        return currentobj.ToString(Newtonsoft.Json.Formatting.None);
    }

    private IQueryable<Guid> GetRoleMapPostTypeAccessId(Guid staffId)
    {
        var userPostTypes = from orgInfo in DbContext.OrganiztionInfos
                            join orgPostType in DbContext.LookUps on orgInfo.OrganiztionPostId equals orgPostType.Id
                            where orgInfo.StaffId == staffId && orgInfo.IsActive

                            select new { orgInfo, orgPostType };
        var userPostTypeIds = new List<Guid>();
        foreach (var item in userPostTypes)
        {
            var postTypeId = DbContext.LookUps.Where(l => l.Id == item.orgInfo.OrganiztionPostId)
                .Select(o => o.Aux).FirstOrDefault();
            if (postTypeId != null)
            {
                userPostTypeIds.Add(Guid.Parse(postTypeId));
            }
        }


        var roleMapPostTypeAccessId = from lookup in DbContext.LookUps
                                      join roleMapPostType in DbContext.RoleMapPostTypes
                                          on lookup.Id equals roleMapPostType.PostTypeId
                                      where userPostTypeIds.Contains(roleMapPostType.PostTypeId)
                                      select roleMapPostType.RoleId;

        return roleMapPostTypeAccessId;

    }

    private IQueryable<Guid> GetRoleMapPostTitleAccessId(Guid staffId)
    {

        var userPostTitleIds = new List<Guid>();

        var userPostTitles = from orgInfo in DbContext.OrganiztionInfos
                             join orgPostTitle in DbContext.LookUps on orgInfo.OrganiztionPostId equals orgPostTitle.Id
                             where orgInfo.StaffId == staffId && orgInfo.IsActive

                             select new { orgInfo, orgPostTitle };

        foreach (var item in userPostTitles)
        {
            var postTitleId = DbContext.LookUps.Where(l => l.Id == item.orgInfo.OrganiztionPostId)
                .Select(o => o.Id).FirstOrDefault();
            userPostTitleIds.Add(postTitleId);

        }

        var roleMapPostTitleAccessId = from lookup in DbContext.LookUps
                                       join roleMapPostTitle in DbContext.RoleMapPostTitles
                                           on lookup.Id equals roleMapPostTitle.PostTitleId
                                       where userPostTitleIds.Contains(roleMapPostTitle.PostTitleId)
                                       select roleMapPostTitle.RoleId;

        return roleMapPostTitleAccessId;

    }
    public IEnumerable<WorkFlowFormViewModel> GetByAccessPolicy(string username)
    {
        var user = DbContext.Users.Single(c => c.UserName == username);
        var userId = user.Id;
        var staffId = user.StaffId;

        var roleAccessesId = from ra in DbContext.RoleAccesses
                             where ra.UserId == userId
                             select ra.RoleId;

        var roleMapChartAccessId = from organizationInfo in DbContext.OrganiztionInfos
                                   join chart in DbContext.Charts on organizationInfo.ChartId equals chart.Id
                                   join roleMapChart in DbContext.RoleMapCharts on chart.Id equals roleMapChart.ChartId
                                   where organizationInfo.StaffId == staffId
                                   select roleMapChart.RoleId;

        var roleMapPostTypeAccessId = GetRoleMapPostTypeAccessId(staffId);

        var roleMapPostTitleAccessId = GetRoleMapPostTitleAccessId(staffId);

        var roleIds = new List<Guid>();
        roleIds.AddRange(roleMapPostTitleAccessId);
        roleIds.AddRange(roleMapChartAccessId);
        roleIds.AddRange(roleAccessesId);
        roleIds.AddRange(roleMapPostTypeAccessId);
        roleIds = roleIds.Distinct().ToList();

        var checkFormsForPerson = from form in DbContext.WorkFlowForms
                                  where form.StaffId == staffId
                                  select new WorkFlowFormViewModel()
                                  {
                                      Id = form.Id,
                                      PName = form.PName,
                                      Staff = form.Staff.FName + " " + form.Staff.LName,
                                      Modifier = form.Modifier == null ? "" : form.Modifier.FName + " " + form.Modifier.LName,
                                      RegisterDateTime = form.RegisterDate != 0 ? form.RegisterDate.ToString().Insert(4, "/").Insert(7, "/") + " " + form.RegisterTime.Insert(2, ":") : "",
                                      ModifiedDateTime = (form.ModifideDate != 0 && form.ModifideDate != null) ? form.ModifideDate.ToString().Insert(4, "/").Insert(7, "/") + " " + form.ModifideTime.Insert(2, ":") : "",
                                      Time = form.RegisterTime,
                                      SortingRegisterDate = form.RegisterDate,
                                      OrginalVersion = form.OrginalVersion,
                                      SecondaryVersion = form.SecondaryVersion
                                  };

        var checkFormsInAccess = from form in DbContext.WorkFlowForms
                                 join roleClaim in DbContext.RoleClaims on form.Id.ToString() equals roleClaim.ClaimValue
                                 join roleId in roleIds on roleClaim.RoleId equals roleId
                                 where roleClaim.ClaimType == PermissionPolicyType.WorkFlowFormPreviewPermission
                                 select new WorkFlowFormViewModel()
                                 {
                                     Id = form.Id,
                                     PName = form.PName,
                                     Staff = form.Staff.FName + " " + form.Staff.LName,
                                     Modifier = form.Modifier == null ? "" : form.Modifier.FName + " " + form.Modifier.LName,
                                     RegisterDateTime = form.RegisterDate != 0 ? form.RegisterDate.ToString().Insert(4, "/").Insert(7, "/") + " " + form.RegisterTime.Insert(2, ":") : "",
                                     ModifiedDateTime = (form.ModifideDate != 0 && form.ModifideDate != null) ? form.ModifideDate.ToString().Insert(4, "/").Insert(7, "/") + " " + form.ModifideTime.Insert(2, ":") : "",
                                     Time = form.RegisterTime,
                                     SortingRegisterDate = form.RegisterDate,
                                     OrginalVersion = form.OrginalVersion,
                                     SecondaryVersion = form.SecondaryVersion
                                 };

        var all = checkFormsForPerson.Union(checkFormsInAccess);

        var allDistinct = new List<WorkFlowFormViewModel>();
        var allDistinctById = all.AsEnumerable().DistinctBy(i => i.Id).ToList();
        foreach (var item in allDistinctById)
        {
            allDistinct.Add(new WorkFlowFormViewModel()
            {
                SortingRegisterTime = (item.Time != null && int.Parse(item.Time) != 0) ? int.Parse(item.Time) : 0,
                Id = item.Id,
                PName = item.PName,
                Staff = item.Staff,
                Modifier = item.Modifier,
                RegisterDateTime = item.RegisterDateTime,
                ModifiedDateTime = item.ModifiedDateTime,
                Time = item.Time,
                SortingRegisterDate = item.SortingRegisterDate,
                OrginalVersion = item.OrginalVersion,
                SecondaryVersion = item.SecondaryVersion

            });
        }

        return allDistinct.OrderByDescending(i => i.SortingRegisterDate).ThenByDescending(i => i.SortingRegisterTime);
    }

    public IEnumerable<WorkFlowFormViewModel> GetByAccess(Guid staffId)
    {
        var userId = DbContext.Users.Single(d => d.StaffId == staffId).Id;
        var access = from user in DbContext.Users
                     join roleAccess in DbContext.RoleAccesses on user.Id equals roleAccess.UserId
                     join role in DbContext.Roles on roleAccess.RoleId equals role.Id
                     where role.Name == "sysadmin" && user.Id == userId
                     select role;
        if (access.Any())
        {
            return GetAllWorkFlowForms();
        }



        var roleAccessesId = from ra in DbContext.RoleAccesses
                             where ra.UserId == userId
                             select ra.RoleId;

        var rolemapchartaccessId = from organiztionInfo in DbContext.OrganiztionInfos
                                   join chart in DbContext.Charts on organiztionInfo.ChartId equals chart.Id
                                   join rolemapchar in DbContext.RoleMapCharts on chart.Id equals rolemapchar.ChartId
                                   where organiztionInfo.StaffId == staffId
                                   select rolemapchar.RoleId;

        var roleMapPostTypeAccessId = GetRoleMapPostTypeAccessId(staffId);

        var roleMapPostTitleAccessId = GetRoleMapPostTitleAccessId(staffId);

        var roleIds = new List<Guid>();
        roleIds.AddRange(roleMapPostTitleAccessId);
        roleIds.AddRange(rolemapchartaccessId);
        roleIds.AddRange(roleAccessesId);
        roleIds.AddRange(roleMapPostTypeAccessId);
        roleIds = roleIds.Distinct().ToList();

        var checkFormsForPerson = from form in DbContext.WorkFlowForms.ToList()
                                  where form.StaffId == staffId
                                  select new WorkFlowFormViewModel()
                                  {
                                      Id = form.Id,
                                      PName = form.PName,
                                      Staff = form.Staff.FName + " " + form.Staff.LName,
                                      Modifier = form.Modifier == null ? "" : form.Modifier.FName + " " + form.Modifier.LName,
                                      RegisterDateTime = form.RegisterDate != 0 ? form.RegisterDate.ToString().Insert(4, "/").Insert(7, "/") + " " + form.RegisterTime.Insert(2, ":") : "",
                                      ModifiedDateTime = (form.ModifideDate != 0 && form.ModifideDate != null) ? form.ModifideDate.ToString().Insert(4, "/").Insert(7, "/") + " " + form.ModifideTime.Insert(2, ":") : "",
                                      OrginalVersion = form.OrginalVersion,
                                      SecondaryVersion = form.SecondaryVersion
                                  };

        var checkFormsInAccess = from form in DbContext.WorkFlowForms.ToList()
                                 join roleClaim in DbContext.RoleClaims on form.Id.ToString() equals roleClaim.ClaimValue
                                 join roleId in roleIds on roleClaim.RoleId equals roleId
                                 where roleClaim.ClaimType == PermissionPolicyType.WorkFlowFormPreviewPermission
                                 select new WorkFlowFormViewModel()
                                 {
                                     Id = form.Id,
                                     PName = form.PName,
                                     Modifier = form.Modifier == null ? "" : form.Modifier.FName + " " + form.Modifier.LName,
                                     RegisterDateTime = form.RegisterDate != 0 ? form.RegisterDate.ToString().Insert(4, "/").Insert(7, "/") + " " + form.RegisterTime.Insert(2, ":") : "",
                                     ModifiedDateTime = (form.ModifideDate != 0 && form.ModifideDate != null) ? form.ModifideDate.ToString().Insert(4, "/").Insert(7, "/") + " " + form.ModifideTime.Insert(2, ":") : "",
                                     Staff = form.Staff.FName + " " + form.Staff.LName,
                                     OrginalVersion = form.OrginalVersion,
                                     SecondaryVersion = form.SecondaryVersion
                                 };

        var all = checkFormsForPerson.Union(checkFormsInAccess);
        return all.DistinctBy(d => d.Id).ToList();
    }

    public void UpdateWorkFlowDetails(string[] formData, string formTitle)
    {
        if (formData is null || formData[1] == formTitle)
            return;

        var formId = formData[0];
        var allWorkFlowDetails = DbContext.WorkFlowDetails.Where(a => a.WorkFlowFormId.ToString() == formId);

        var workFlows = allWorkFlowDetails.Select(a => a.WorkFlow).Distinct().ToList();

        foreach (var workFlow in workFlows)
        {
            var workFlowDetails = workFlow.WorkflowDetails.Intersect(allWorkFlowDetails);

            foreach (var workFlowDetail in workFlowDetails)
            {
                UnicodeEncoding encoding = new UnicodeEncoding();
                var str = encoding.GetString(workFlow.Content);

                var stringReader = new StringReader(str);
                var reader = XmlReader.Create(stringReader);

                var xdoc = XDocument.Load(reader);

                XNamespace workflow = "http://magic";
                XNamespace bpmn2 = "http://www.omg.org/spec/BPMN/20100524/MODEL";

                foreach (var xElement in xdoc.Descendants().Elements(bpmn2 + "task")
                             .Where(a => a.Attribute(workflow + "WorkFlowFormId").Value == workFlowDetail.WorkFlowFormId.ToString()))
                {
                    xElement.Attribute(workflow + "WorkFlowFormText").Value = formTitle;
                }

                workFlow.Content = encoding.GetBytes(xdoc.ToString());
            }
        }
    }


    public WorkFlowFormVersionViewModel MakeVersion(int maxOrg, int maxSec)
    {
        if (maxSec >= 9)
        {
            maxOrg++;
            maxSec = 0;
        }
        else
        {
            maxSec++;
        }
        return new WorkFlowFormVersionViewModel()
        {
            SecondaryVersion = maxSec,
            OrginalVersion = maxOrg
        };
    }

}