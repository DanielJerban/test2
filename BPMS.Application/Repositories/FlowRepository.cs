using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Constants;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Dtos.FlowServiceDTOs;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;
using System.Web;
using BPMS.Application.Hubs;
using Microsoft.EntityFrameworkCore;

namespace BPMS.Application.Repositories;

public class FlowRepository : Repository<Flow>, IFlowRepository
{
    public BpmsDbContext DbContext => Context;
    private readonly IServiceProvider _serviceProvider;

    public FlowRepository(BpmsDbContext context, IServiceProvider serviceProvider) : base(context)
    {
        _serviceProvider = serviceProvider;
    }

    public ILookUpRepository LookUpRepository => _serviceProvider.GetRequiredService<ILookUpRepository>();
    public IRequestRepository RequestRepository => _serviceProvider.GetRequiredService<IRequestRepository>();

    public void ChangeIsBalloonStatusInFlow(List<Guid> flowIds)
    {
        foreach (var id in flowIds)
        {
            if (id != null && id != Guid.Empty)
            {
                var currentFlow = DbContext.Flows.FirstOrDefault(f => f.Id == id);
                currentFlow.IsBalloon = true;
            }
        }
    }

    public IEnumerable<NotificationViewModel> FindStaffsToNotify(Request request)
    {
        var confermAuthorityStaffs = new List<NotificationViewModel>();
        //شخص دریافت کننده
        var userNames = from flow in request.Flows
                        join users in DbContext.Users on flow.StaffId equals users.StaffId
                        select new NotificationViewModel { UserName = users.UserName, FlowId = flow.Id };
        var staffsToSendNotification = from flow in request.Flows
                                       join users in DbContext.Users on flow.StaffId equals users.StaffId
                                       select users.StaffId;
        foreach (var realStaff in staffsToSendNotification)
        {
            confermAuthorityStaffs = (from flows in request.Flows
                                      join wca in DbContext.WorkFlowConfermentAuthority on flows.WorkFlowDetail.WorkFlow.RequestTypeId
                                          equals wca.RequestTypeId
                                      join wcfad in DbContext.WorkFlowConfermentAuthorityDetails on wca.Id equals wcfad
                                          .ConfermentAuthorityId
                                      join users in DbContext.Users on wcfad.StaffId equals users.StaffId
                                      where wca.StaffId == realStaff && (request.RegisterDate >= wcfad.FromDate && request.RegisterDate <= wcfad.ToDate)
                                      select new NotificationViewModel { UserName = users.UserName, FlowId = Guid.Empty }).ToList();
        }
        var totalStaffsToSendNotification = userNames.Union(confermAuthorityStaffs);
        return totalStaffsToSendNotification;
    }

    public void FindParentFromChartId(Guid? chartId, List<Chart> chartList)
    {
        var chart = DbContext.Charts.FirstOrDefault(c => c.Id == chartId && c.IsActive);
        if (chart != null)
        {
            chartList.Add(chart);
            FindParentFromChartId(chart.ParentId, chartList);
        }
    }

    public Flow ChangeFlowStatus(FlowParam param, Guid statusId)
    {
        byte[] bytes = null;
        if (param.Work != null)
        {
            Type typeWork = param.Work.GetType();
            if (typeWork.Name.ToLower() == "string")
            {
                var content = HttpUtility.UrlDecode(param.Work, Encoding.UTF8);
                var encoding = new UnicodeEncoding();
                bytes = encoding.GetBytes(content);
            }
        }
        var flow = DbContext.Flows.Find(param.CurrentFlowId);
        flow.ResponseDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
        flow.ResponseTime = DateTime.Now.ToString("HHmm");
        flow.OrganizationPostTitleId = param.ConfirmOrganizationPostTitleId == Guid.Empty ? param.OrganizationPostTitleId : param.ConfirmOrganizationPostTitleId;
        flow.FlowStatusId = statusId;
        // todo: uncomment the code below and use the username u get from the input parameters
        var currentStaff = param.ApiStaffId /*?? GlobalVariables.User.StaffId*/;
        if (param.IsEnd) flow.IsEnd = true;
        if (flow.StaffId != currentStaff)
        {
            flow.ConfermentAuthorityStaffId = currentStaff;
        }
        flow.Dsr = param.Dsr;
        flow.Request.Value = bytes;
        DbContext.Flows.Update(flow);
        return flow;
    }

    public List<WorkFlowDetail> GetAllWorkFlowNextSteps(Guid wfdId)
    {
        var data = new List<WorkFlowDetail>();
        GetAllWfdns(data, wfdId);

        return data;
    }

    private void GetAllWfdns(List<WorkFlowDetail> wfdList, Guid wfdId)
    {
        var wfnsList = DbContext.WorkFlowNextSteps.Where(c => c.FromWfdId == wfdId && c.NextStepToWfd.Step == null).ToList();
        if (wfnsList.Count < 1)
        {
            return;
        }

        foreach (var wfns in wfnsList)
        {
            if (wfdList.Any(c => c.Id == wfns.NextStepFromWfd.Id))
            {
                continue;
            }

            wfdList.Add(wfns.NextStepToWfd);
            GetAllWfdns(wfdList, wfns.NextStepToWfd.Id);
        }
    }

    public IQueryable<Workflow> GetWorkFlows()
    {
        return DbContext.Workflows;
    }

    public IQueryable<Staff> GetStaffs()
    {
        return DbContext.Staffs;
    }

    public IQueryable<Assingnment> GetAssingnments()
    {
        return DbContext.Assingnments;
    }

    public IQueryable<LookUp> GetLookUps()
    {
        return DbContext.LookUps;
    }
    public IQueryable<WorkFlowNextStep> GetWorkFlowNextSteps()
    {
        return DbContext.WorkFlowNextSteps;
    }

    public IQueryable<OrganiztionInfo> GetOrganiztionInfos()
    {
        return DbContext.OrganiztionInfos;
    }
    public Guid? NextFlowIdIfAcepptorExist(Guid staffId)
    {
        return DbContext.ChangeTracker.Entries<Flow>().FirstOrDefault(d =>
                d.State == EntityState.Added && d.Entity.IsActive &&
                d.Entity.StaffId == staffId)
            ?.Entity.Id;
    }

    public bool CheckInclusivePath(WorkFlowDetail workFlowD, FlowParam param, string gatewayName)
    {
        var workFlowDetails = DbContext.WorkFlowDetails
            .Where(f => f.WorkFlowId == workFlowD.WorkFlowId && (f.Step == null || f.Step == 0))
            .ToList();

        var wfdHasGetWay = new List<WorkFlowDetail>();

        foreach (var workFlowDetail in workFlowDetails)
        {
            var wfnss = workFlowDetail.WorkFlowNextStepsFrom.ToList();
            foreach (var next in wfnss)
            {
                if (next.Gateway == null) continue;
                // var gateways = next.Gateway.Split('@');
                if (next.Gateway.Contains(gatewayName))
                {
                    wfdHasGetWay.Add(workFlowDetail);
                }
            }
        }

        var activeInclusive = new List<bool>();
        foreach (var workFlowDetail in wfdHasGetWay)
        {
            activeInclusive.Add(
                workFlowDetail.Flows.Any(d => d.RequestId == param.RequestId && d.LookUpFlowStatus.Code != 1));
        }

        if (activeInclusive.Any() && activeInclusive.All(d => d == false))
        {
            return false;
        }

        return true;
    }

    public void SendNotification(Guid requestId, MainFlowStaff nextflowstaffresult)
    {
        var acceptorPersonelCode = from reqs in DbContext.Requests
                                   join staffs in DbContext.Staffs on reqs.StaffId equals staffs.Id
                                   where reqs.Id == requestId
                                   select staffs.PersonalCode;
        if (nextflowstaffresult.FlowStaves != null)
        {
            var staffIds = nextflowstaffresult.FlowStaves.Select(s => s.StaffIds).FirstOrDefault();
            var personelCodes = from staffs in DbContext.Staffs
                                join staffid in staffIds on staffs.Id equals staffid
                                select staffs.PersonalCode;
            var totalPersonelCodes = acceptorPersonelCode.Union(personelCodes).ToList();
            MainHub.UpdateNotificationCount(totalPersonelCodes.ToList());
        }
        else
        {
            MainHub.UpdateNotificationCount(acceptorPersonelCode.ToList());
        }
    }
    public IEnumerable<FlowViewModel> GetAllCartbotRecords(Guid? id, string username)
    {
        var user = DbContext.Users.Single(c => c.UserName == username);

        return DbContext.Flows.Where(f => f.FlowStatusId == id && f.StaffId == user.StaffId).ToList().Select(l => new FlowViewModel()
        {
            Id = l.Id,
            ConfermentAuthorityStaffId = l.ConfermentAuthorityStaffId.Value,
            StaffId = l.StaffId,
            Dsr = l.Dsr,
            RequestId = l.RequestId,
            ResponseDate = l.ResponseDate,
            ResponseTime = l.ResponseTime,
            IsBalloon = l.IsBalloon,
            PreviousFlowId = l.PreviousFlowId,
            FlowStatusId = l.FlowStatusId,
            WorkFlowDetailId = l.WorkFlowDetailId
        });
    }

    public dynamic GetValueForStaticForms(Flow flow)
    {
        dynamic value = null;
        //گواهی اشتغال
        if (flow.Request.Workflow.RequestTypeId == Guid.Parse("C7C474AE-1EAE-4FB7-91C8-B379C3103E22"))
        {
            var currentEmployementCertificate = DbContext.EmployementCertificate
                .FirstOrDefault(u => u.Requests.Workflow.RequestTypeId == flow.Request.Workflow.RequestTypeId && u.RequestId == flow.RequestId);
            var model = new RequestEmployementCertificationViewModel()
            {
                RequestIntention = currentEmployementCertificate.RequestIntention,
                RequestId = currentEmployementCertificate.RequestId,
                Dsr = currentEmployementCertificate.Dsr,
                RequestTypeId = flow.Request.Workflow.RequestTypeId,
                UserDsr = flow.Dsr
            };
            value = model;
        }
        return value;
    }

    public void ChangeBalloonStatus(Request request)
    {
        //ارسال نوتیفیکیشن 
        var totalStaffsToSendNotification = FindStaffsToNotify(request).ToList();
        var requestType = DbContext.LookUps.FirstOrDefault(l => l.Id == request.Workflow.RequestTypeId)?
            .Title;
        var requestorStaff = DbContext.Staffs.FirstOrDefault(s => s.Id == request.StaffId);
        var model = new SendNotificationViewModel()
        {
            RequesterStaff = requestorStaff.FName + " " + requestorStaff.LName,
            RequesterPersonelCode = requestorStaff.PersonalCode,
            RequestTitle = requestType,
            RequestNo = request.WorkFlowId.ToString(),
            FlowStaffs = totalStaffsToSendNotification
        };
        var flowList = MainHub.SendNotificationToSpecificClients(model);
        foreach (var id in flowList)
        {
            if (id != null && id != Guid.Empty)
            {
                var currentFlow = DbContext.Flows.Where(f => f.Id == id).FirstOrDefault();
                currentFlow.IsBalloon = true;
                DbContext.Flows.Update(currentFlow);
            }

        }
    }

    public WorkFlowFormViewModel ExternalCreateProcess(ProcessDto model)
    {

        if (model.OrganizationPostTitleId == Guid.Empty || model.OrganizationPostTitleId == null)
        {
            throw new ArgumentException("پست سازمانی انتخاب نشده است");
        }
        var workFlowDetail = DbContext.WorkFlowDetails.Single(p => p.Step == 0 &&
                                                                   p.WorkFlow.RequestTypeId == model.RequestTypeId &&
                                                                   p.WorkFlow.IsActive);
        if (workFlowDetail.ViewName != null)
        {
            throw new ArgumentException("فقط فرآیند های داینامیک پشتیبانی می شود");
        }



        var encoding = new UnicodeEncoding();
        var json = encoding.GetString(workFlowDetail.WorkFlowForm.Content);
        var encoodeJson = HelperBs.EncodeUri(json);
        string jquery = null;
        string CssCode = null;
        if (workFlowDetail.WorkFlowForm.Jquery != null)
        {
            jquery = HelperBs.EncodeUri(encoding.GetString(workFlowDetail.WorkFlowForm.Jquery));
        }
        if (workFlowDetail.WorkFlowForm.AdditionalCssStyleCode != null)
        {
            CssCode = HelperBs.GetCssStyleFromByteArray_Unicode(workFlowDetail.WorkFlowForm.AdditionalCssStyleCode);
        }

        var previousForm = GetPreviousForm(json, null);
        var workFlowForm = new WorkFlowFormViewModel()
        {
            Id = workFlowDetail.WorkFlowForm.Id,
            PName = workFlowDetail.WorkFlowForm.PName,
            JsonFile = encoodeJson,
            PreviousForm = previousForm,
            EditableCheck = workFlowDetail.EditableFields,
            Value = "{}",
            Content = workFlowDetail.WorkFlowForm.Content,
            WorkFlowDetailId = workFlowDetail.Id,
            Jquery = jquery,
            AdditionalCssStyleCode = CssCode,
            PrintFileName = workFlowDetail.PrintFileName
        };
        return workFlowForm;
    }

    private List<PreviousFormViewModel> GetPreviousForm(string jsonFile, Flow flow)
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

                var j = encoding.GetString(frm.Content);
                var encoodeJson = HelperBs.EncodeUri(j);
                list.Add(new PreviousFormViewModel()
                {
                    FormId = formid.ToString(),
                    JsonForm = encoodeJson
                });
                var o = GetPreviousForm(j, flow);
                list.AddRange(o);
            }
        }

        return list;
    }

    public object GetFlowsByStaffId(Guid id, int code)
    {
        return DbContext.Flows.Where(d => d.LookUpFlowStatus.Code == code && d.StaffId == id).ToList().Select(s =>
            new FlowsByStaffViewModel()
            {
                FlowId = s.Id.ToString(),
                RequestNo = s.Request.RequestNo,
                RequestType = s.WorkFlowDetail.WorkFlow.RequestType.Title,
                Version = " نسخه " + s.WorkFlowDetail.WorkFlow.OrginalVersion + '.' + s.WorkFlowDetail.WorkFlow.SecondaryVersion,
                FlowTitle = s.WorkFlowDetail.Title,
                RegisterDate = HelperBs.MakeDate(s.Request.RegisterDate.ToString()),
                RegisterTime = HelperBs.MakeTime(s.Request.RegisterTime),
                RequesterName = s.Request.Staff.FullName,
                StaffDropDown = new StaffDropDownViewModel()
                {
                    text = "انتخاب کاربر جایگزین",
                    value = ""
                }
            }).OrderByDescending(r => r.RegisterDate).ThenByDescending(r => r.RegisterTime);
    }

    public void ChangeFlowsStaff(List<FlowViewModel> model, string oldStaffId)
    {
        List<string> usernames = new List<string>();

        if (model == null)
            throw new ArgumentException("رکوردی انتخاب نشده است.");
        foreach (var item in model)
        {
            var flow = DbContext.Flows.Find(item.Id);
            if (flow != null)
            {
                flow.StaffId = item.StaffId;
                flow.DelayDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                flow.DelayTime = DateTime.Now.ToString("HHmm");
            }

            var userName = Context.Users.Where(c => c.StaffId == item.StaffId).Select(c => c.UserName).SingleOrDefault();
            if (userName != null)
            {
                usernames.Add(userName);
            }

            Guid oldStaffGuid = Guid.Parse(oldStaffId);
            var oldStaffUserName = Context.Users.Where(c => c.StaffId == oldStaffGuid).Select(c => c.UserName).SingleOrDefault();
            if (oldStaffUserName != null)
            {
                usernames.Add(oldStaffUserName);
            }
        }

        MainHub.RefreshCartbotGridOnChangingFlow(usernames);
    }

    public DataSourceResult GetFlows(Guid userStaffId, DataSourceRequest request, int code)
    {
        // Code 1 => اقدام نشده
        // Code 2 => تایید شده
        // Code 3 => رد شده

        var query1 = from w in DbContext.WorkFlowConfermentAuthority
                     join wd in DbContext.WorkFlowConfermentAuthorityDetails on w.Id equals wd.ConfermentAuthorityId
                     join workflows in DbContext.Workflows on w.RequestTypeId equals workflows.RequestTypeId
                     join req in DbContext.Requests on workflows.Id equals req.WorkFlowId
                     join flow in DbContext.Flows on req.Id equals flow.RequestId
                     where wd.StaffId == userStaffId
                           && flow.StaffId == w.StaffId
                           && (req.RegisterDate >= wd.FromDate && req.RegisterDate <= wd.ToDate)
                           && (!wd.OnlyOwnRequest || req.StaffId == userStaffId)
                           && (code == 0 || flow.IsActive)
                           && (code == 0 || flow.LookUpFlowStatus.Code == code)
                     select flow;
        var query2 = from f in DbContext.Flows
                     where f.StaffId == userStaffId
                           && (code == 0 || f.IsActive)
                           && (code == 0 || f.LookUpFlowStatus.Code == code)
                     select f;
        var all = query1.Union(query2).Select(s => new FlowsInCartbotViewModel()
        {
            RequestNo = s.Request.RequestNo,
            FullName = s.Request.Staff.FName + " " + s.Request.Staff.LName,
            PersonalCode = s.Request.Staff.PersonalCode,
            CurrentStatus = s.LookUpFlowStatus.Title,
            RequestDate = s.Request.RegisterDate.ToString().Insert(4, "/").Insert(7, "/"),
            RequestTime = s.Request.RegisterTime.Insert(2, ":"),
            StepTitle = s.WorkFlowDetail.Title,
            FlowId = s.Id,
            RequestTypeTitle = s.WorkFlowDetail.WorkFlow.RequestType.Title + " / نسخه " + s.WorkFlowDetail.WorkFlow.OrginalVersion + "." + s.WorkFlowDetail.WorkFlow.SecondaryVersion,
            //RequestTypeTitle = GetRequestTypeTitle(s.WorkFlowDetailId, s.WorkFlowDetail.WorkFlow.RequestType.Title + " / نسخه " + s.WorkFlowDetail.WorkFlow.OrginalVersion + "." + s.WorkFlowDetail.WorkFlow.SecondaryVersion),
            RequestId = s.RequestId,
            RequestTypeId = s.WorkFlowDetail.WorkFlow.RequestTypeId,
            IsBalloon = s.IsBalloon,
            WorkflowDetailId = s.WorkFlowDetailId,
            IsMultiConfirmReject = s.WorkFlowDetail.IsMultiConfirmReject,
            Message = s.PreviousFlow.Dsr,
            StaffId = s.StaffId,
            ImagePath = s.Request.Staff.ImagePath,
            WorkflowId = s.WorkFlowDetail.WorkFlowId,
            SubprocessId = s.WorkFlowDetail.WorkFlow.SubProcessId,
            // for delay calculate
            DelayDate = s.DelayDate,
            DelayTime = s.DelayTime,
            DelayDateTime = s.DelayDate.ToString().Insert(4, "/").Insert(7, "/") + " " + s.DelayTime.Insert(2, ":"),
            WaitingTime = s.WorkFlowDetail.WaitingTime,
            IsRead = s.IsRead,
            ResponseDate = s.ResponseDate,
            ResponseTime = s.ResponseTime,
            ResponseDateTime = (s.ResponseDate != null ? s.ResponseDate.ToString().Insert(4, "/").Insert(7, "/") : s.ResponseDate.ToString()) + " " + (s.ResponseTime != "" ? s.ResponseTime.Insert(2, ":") : ""),
            DynamicWaitingTime = s.DynamicWaitingTime
        });

        // var all2 = new List<FlowsInCartbotViewModel>();
        // foreach (var item in all.ToList())
        // {
        //     item.RequestTypeTitle = GetRequestTypeTitle(item.WorkflowDetailId, item.RequestTypeTitle);
        //     all2.Add(item);
        // }

        var data = all.ToDataSourceResult(request);
        if (code == 1)
        {
            var s2W = Context.LookUps.SingleOrDefault(d => d.Type == "WorkTime" && d.Code == 1);
            var thr = Context.LookUps.SingleOrDefault(d => d.Type == "WorkTime" && d.Code == 2);
            var holiday = DbContext.Holydays.ToList();
            foreach (var item in data.Data.Cast<FlowsInCartbotViewModel>())
            {
                var reqDate = DateTime.ParseExact(item.DelayDate + item.DelayTime, "yyyyMMddHHmm", null,
                    DateTimeStyles.None);
                if (item.DynamicWaitingTime != null)
                    item.WaitingTime = item.DynamicWaitingTime;


                var timeToDo = CalculateTimeToDo(reqDate, item.WaitingTime, s2W, thr, holiday);
                var diff = Math.Round(CalculateDelay(reqDate, DateTime.Now, s2W, thr, holiday));
                var sediment = (diff > item.WaitingTime)
                    ? (diff - item.WaitingTime)
                    : 0;
                item.IsRed = sediment > 0;

                item.TimeToDo = timeToDo?.ToString("HH:mm yyyy/MM/dd");
                item.Delay = sediment.ToString();

                // Get subprocess name and workflow name 
                item.RequestTypeTitle = GetRequestTypeTitle(item.WorkflowId, item.SubprocessId, item.RequestTypeTitle);
            }
        }
        else
        {

            foreach (var item in data.Data.Cast<FlowsInCartbotViewModel>())
            {

                item.RequestTypeTitle = GetRequestTypeTitle(item.WorkflowId, item.SubprocessId, item.RequestTypeTitle);
            }
        }

        return data;
    }

    public GetUsersContainsInFlowStatusChangeOutputDTO GetUsersContainsInFlowStatusChange(Guid requestId)
    {
        var request = (from req in DbContext.Requests
                       join staff in DbContext.Staffs on req.StaffId equals staff.Id
                       where req.Id == requestId
                       select new
                       {
                           req,
                           staff
                       }).Single();

        var noActionFlowsStaff = (from flow in DbContext.Flows
                                  join staff in DbContext.Staffs on flow.StaffId equals staff.Id
                                  join flowStatus in DbContext.LookUps on flow.FlowStatusId equals flowStatus.Id
                                  join wfd in DbContext.WorkFlowDetails on flow.WorkFlowDetailId equals wfd.Id
                                  where flowStatus.Code == 1 && flowStatus.Type == "FlowStatus" && flow.RequestId == request.req.Id
                                  select new StaffFlowDTO
                                  {
                                      FirstName = staff.FName,
                                      LastName = staff.LName,
                                      PersonalCode = staff.PersonalCode,
                                      StaffId = staff.Id,
                                      FlowId = flow.Id,
                                      WorkFlowDetailId = wfd.Id,
                                      WorkFlowDetailTitle = wfd.Title
                                  }).ToList();


        return new GetUsersContainsInFlowStatusChangeOutputDTO()
        {
            NoActionStaffs = noActionFlowsStaff,
            RequestNo = request.req.RequestNo,
            RequesterStaff = new StaffSimpleDTO()
            {
                Id = request.staff.Id,
                PersonalCode = request.staff.PersonalCode,
                LastName = request.staff.LName,
                FirstName = request.staff.FName
            }
        };
    }

    #region BpmsHelper

    public List<Guid> GetParentWorkFlowIds(Guid? id)
    {
        List<Guid> workflowIds = new List<Guid>();
        var thisWorkFlow = DbContext.Workflows.SingleOrDefault(c => c.Id == id);

        while (thisWorkFlow.SubProcessId != null)
        {
            id = thisWorkFlow.SubProcessId;
            thisWorkFlow = DbContext.Workflows.SingleOrDefault(c => c.Id == id);
            workflowIds.Add(thisWorkFlow.Id);
        }

        return workflowIds;
    }

    public string GetRequestTypeTitle(Guid workflowId, Guid? subprocessId, string restOfTitle)
    {
        string requestTypeTitle = "";
        string versionParentWorkflow = "";
        string workflowTitle = "";

        if (subprocessId != null)
        {
            List<Guid> parentWorkFlowIds = GetParentWorkFlowIds(workflowId);

            for (int i = parentWorkFlowIds.Count; i > 0; i--)
            {
                var parentWorkFlowId = parentWorkFlowIds[i - 1];
                var parentWorkflow = DbContext.Workflows.SingleOrDefault(c => c.Id == parentWorkFlowId);
                workflowTitle += DbContext.LookUps.SingleOrDefault(c => c.Id == parentWorkflow.RequestTypeId).Title;
                if (i == parentWorkFlowIds.Count)
                {
                    versionParentWorkflow = parentWorkflow.OrginalVersion + "." + parentWorkflow.SecondaryVersion;
                    workflowTitle += " / نسخه " + versionParentWorkflow;
                }
                workflowTitle += " / ";
            }

            requestTypeTitle = workflowTitle + restOfTitle.Split('/')[0];
        }
        else
        {
            requestTypeTitle = restOfTitle;
        }

        return requestTypeTitle;
    }


    public void CheckEventGateWay(FlowEvent flowsEvent)
    {
        if (!string.IsNullOrWhiteSpace(flowsEvent.GatewayEventBase))
        {
            var flowevt = DbContext.FlowEvents.Where(d => d.GatewayEventBase == flowsEvent.GatewayEventBase && d.Id != flowsEvent.Id && d.Flow.RequestId == flowsEvent.Flow.RequestId).ToList();
            foreach (var item in flowevt)
            {
                var f = DbContext.Flows.Find(item.FlowId);
                if (f != null)
                {
                    DbContext.Flows.Remove(f);
                }

            }
        }
    }

    #endregion
    public void SetDynamicWaitingTime(Guid? flowId, dynamic work)
    {
        if (flowId != null)
        {
            var flow = DbContext.Flows.FirstOrDefault(a => a.Id == flowId);
            var wfd = flow.WorkFlowDetail;

            if (wfd != null)
            {
                if (wfd.Info != null)
                {
                    var obj = JObject.Parse(wfd.Info);
                    string dynamicWaitingDate = obj.GetValue("DynamicWaitingDate").ToString();

                    var formData = JObject.Parse(Convert.ToString(work));
                    string time = formData.GetValue(dynamicWaitingDate).ToString();

                    bool isNumber = int.TryParse(time, out int result);
                    if (isNumber)
                    {
                        flow.DynamicWaitingTime = result;
                    }
                    else
                    {
                        flow.DynamicWaitingTime = null;
                    }
                    Context.SaveChanges();
                }

            }
        }
    }

    public Flow GetFlowById(Guid flowId)
    {
        return Context.Flows.FirstOrDefault(a => a.Id == flowId);
    }

    public Flow GetFlowByRequestIdAndWorkFlowDetailId(Guid requestId, Guid workflowDetailId)
    {
        return Context.Flows.FirstOrDefault(s =>
            s.WorkFlowDetailId == workflowDetailId && s.RequestId == requestId);
    }

    public List<Guid> GetTimerStartEvents()
    {
        var list = new List<Guid>();
        var timersEligibleToRun = (from startTimerEvent in DbContext.StartTimerEvents
                 join workflow in DbContext.Workflows on startTimerEvent.WorkFlowId equals workflow.Id
                 where 
                     workflow.IsActive && 
                     startTimerEvent.StartDateTime < DateTime.Now && 
                     (startTimerEvent.ExpireDateTime > DateTime.Now || startTimerEvent.ExpireDateTime == null)
                 select startTimerEvent).ToList();

        foreach (var item in timersEligibleToRun)
        {
            DbContext.Entry(item).Reload();
        }

        foreach (var timer in timersEligibleToRun)
        {
            if (timer.LastRunTime is null)
            {
                list.Add(timer.Id);
            }
            else if (timer.IsSequential)
            {
                if (DateTime.Now > GetNextRunTime(timer))
                    list.Add(timer.Id);

            }
        }
        return list;
    }

    public void SetTimerLastRunDate(Guid id)
    {
        Context.StartTimerEvents.Single(t => t.Id == id).LastRunTime = DateTime.Now;
    }

    private DateTime GetNextRunTime(StartTimerEvent timer)
    {
        var timeBtwStartAndLastRun = ((DateTime)timer.LastRunTime - timer.StartDateTime);

        var timesRan = Convert.ToInt32(timeBtwStartAndLastRun.TotalHours / (double)timer.IntervalHours);

        var nextRunTime = timer.StartDateTime.AddHours((timesRan + 1) * (int)timer.IntervalHours);
        return nextRunTime;
    }

    public double CalculateDelay(DateTime reqDate, DateTime compareTime, LookUp saturday2Wendsday, LookUp thursday, List<Holyday> holidays)
    {

        var startTimeWork = int.Parse(saturday2Wendsday.Aux.Substring(0, 2));


        var endTimeWork = int.Parse(saturday2Wendsday.Aux2.Substring(0, 2));


        var startTimeWorkThu = int.Parse(thursday.Aux.Substring(0, 2));


        var endTimeWorkThu = int.Parse(thursday.Aux2.Substring(0, 2));


        var workTime = endTimeWork - startTimeWork;
        var workTimeThu = endTimeWorkThu - startTimeWorkThu;

        var reqDateInt = int.Parse(reqDate.ToString("yyyyMMdd"));
        var compareTimeInt = int.Parse(compareTime.ToString("yyyyMMdd"));

        var tb = holidays.Where(d => d.Date > reqDateInt && d.Date < compareTimeInt).ToList();

        int allTur = 0;
        int allHoly = 0;

        foreach (var holiday in tb)
        {
            int holidayTypeCode;

            if (holiday.HolydayType == null)
            {
                var holidayType = DbContext.Holydays.Include(c => c.HolydayType).Single(c => c.Id == holiday.Id).HolydayType;
                holidayTypeCode = holidayType.Code;
            }
            else
            {
                holidayTypeCode = holiday.HolydayType.Code;
            }

            if (holidayTypeCode == 3)
            {
                allTur++;
            }
            else
            {
                allHoly++;
            }
        }

        var reqTime = int.Parse(reqDate.ToString("HHmm").Substring(0, 2));
        var time = int.Parse(compareTime.ToString("HHmm").Substring(0, 2));

        var reqMin = int.Parse(reqDate.ToString("HHmm").Substring(2, 2));
        var compareMin = int.Parse(compareTime.ToString("HHmm").Substring(2, 2));

        long hour = 0;
        var diiffday = (int)(compareTime.Date - reqDate.Date).TotalDays;
        // چک کردن روز درخواست وقتی برابر روز مقایسه است
        if (diiffday == 0)
        {
            if (reqDate.DayOfWeek == DayOfWeek.Friday)
            {
                return 0;
            }

            if (reqDate.DayOfWeek == DayOfWeek.Thursday)
            {

                if (time - reqTime > workTimeThu)
                {
                    hour += workTimeThu;

                }
                else
                {
                    hour += time - reqTime;
                    hour = GetDelayAsHour(compareMin, reqMin, hour);
                }
            }
            else
            {
                if (time - reqTime > workTime)
                {
                    hour += workTime;

                }
                else
                {

                    hour += time - reqTime;
                    hour = GetDelayAsHour(compareMin, reqMin, hour);
                }
            }
            return hour;

        }
        else if (diiffday >= 1)
        {
            var diff = diiffday - 1 - allTur - allHoly;
            hour = diff * workTime + (allTur * workTimeThu);
        }

        //  چک کردن روز اول و آخر

        if (reqDate.DayOfWeek == DayOfWeek.Thursday)
        {
            if (endTimeWorkThu - reqTime > workTimeThu)
            {
                hour += workTimeThu;
            }
            else
            {
                hour += endTimeWorkThu - reqTime > 0 ? endTimeWorkThu - reqTime : 0;
            }


        }
        else if (reqDate.DayOfWeek != DayOfWeek.Friday)
        {
            if (endTimeWork - reqTime > workTime)
            {
                hour += workTime;
            }
            else
            {
                hour += endTimeWork - reqTime > 0 ? endTimeWork - reqTime : 0;
            }


        }

        if (compareTime.DayOfWeek == DayOfWeek.Thursday)
        {


            if (time - startTimeWorkThu > workTimeThu)
            {
                hour += workTimeThu;
            }
            else
            {
                hour += time - startTimeWorkThu > 0 ? time - startTimeWorkThu : 0;
            }
        }
        else if (compareTime.DayOfWeek != DayOfWeek.Friday)
        {

            if (time - startTimeWork > workTime)
            {
                hour += workTime;
            }
            else
            {
                hour += time - startTimeWork > 0 ? time - startTimeWork : 0;
            }

        }

        return hour;
    }

    public double CalculateDoneTime(DateTime reqDate, DateTime compareTime, LookUp saturday2Wendsday, LookUp thursday, List<Holyday> holidays)
    {

        var startTimeWork = int.Parse(saturday2Wendsday.Aux.Substring(0, 2));


        var endTimeWork = int.Parse(saturday2Wendsday.Aux2.Substring(0, 2));


        var startTimeWorkThu = int.Parse(thursday.Aux.Substring(0, 2));


        var endTimeWorkThu = int.Parse(thursday.Aux2.Substring(0, 2));


        var workTime = endTimeWork - startTimeWork;
        var workTimeThu = endTimeWorkThu - startTimeWorkThu;

        var reqDateInt = int.Parse(reqDate.ToString("yyyyMMdd"));
        var compareTimeInt = int.Parse(compareTime.ToString("yyyyMMdd"));

        var tb = holidays.Where(d => d.Date > reqDateInt && d.Date < compareTimeInt).ToList();

        int allTur = 0;
        int allHoly = 0;

        foreach (var holiday in tb)
        {
            int holidayTypeCode;

            if (holiday.HolydayType == null)
            {
                var holidayType = DbContext.Holydays.Include(c => c.HolydayType).Single(c => c.Id == holiday.Id).HolydayType;
                holidayTypeCode = holidayType.Code;
            }
            else
            {
                holidayTypeCode = holiday.HolydayType.Code;
            }

            if (holidayTypeCode == 3)
            {
                allTur++;
            }
            else
            {
                allHoly++;
            }
        }

        var reqTime = int.Parse(reqDate.ToString("HHmm").Substring(0, 2));
        var time = int.Parse(compareTime.ToString("HHmm").Substring(0, 2));

        var reqMin = int.Parse(reqDate.ToString("HHmm").Substring(2, 2));
        var compareMin = int.Parse(compareTime.ToString("HHmm").Substring(2, 2));

        long hour = 0;
        var diiffday = (int)(compareTime.Date - reqDate.Date).TotalDays;
        // چک کردن روز درخواست وقتی برابر روز مقایسه است
        if (diiffday == 0)
        {
            if (reqDate.DayOfWeek == DayOfWeek.Friday)
            {
                return 0;
            }

            if (reqDate.DayOfWeek == DayOfWeek.Thursday)
            {

                if (time - reqTime > workTimeThu)
                {
                    hour += workTimeThu;

                }
                else
                {
                    hour += time - reqTime;
                    hour = GetDoneTimeAsHour(compareMin, reqMin, hour);
                }
            }
            else
            {
                if (time - reqTime > workTime)
                {
                    hour += workTime;

                }
                else
                {

                    hour += time - reqTime;
                    hour = GetDoneTimeAsHour(compareMin, reqMin, hour);
                }
            }
            return hour;

        }
        else if (diiffday >= 1)
        {
            var diff = diiffday - 1 - allTur - allHoly;
            hour = diff * workTime + (allTur * workTimeThu);
        }

        //  چک کردن روز اول و آخر

        if (reqDate.DayOfWeek == DayOfWeek.Thursday)
        {
            if (endTimeWorkThu - reqTime > workTimeThu)
            {
                hour += workTimeThu;
            }
            else
            {
                hour += endTimeWorkThu - reqTime > 0 ? endTimeWorkThu - reqTime : 0;
            }


        }
        else if (reqDate.DayOfWeek != DayOfWeek.Friday)
        {
            if (endTimeWork - reqTime > workTime)
            {
                hour += workTime;
            }
            else
            {
                hour += endTimeWork - reqTime > 0 ? endTimeWork - reqTime : 0;
            }


        }

        if (compareTime.DayOfWeek == DayOfWeek.Thursday)
        {


            if (time - startTimeWorkThu > workTimeThu)
            {
                hour += workTimeThu;
            }
            else
            {
                hour += time - startTimeWorkThu > 0 ? time - startTimeWorkThu : 0;
            }
        }
        else if (compareTime.DayOfWeek != DayOfWeek.Friday)
        {

            if (time - startTimeWork > workTime)
            {
                hour += workTime;
            }
            else
            {
                hour += time - startTimeWork > 0 ? time - startTimeWork : 0;
            }

        }

        return hour;
    }

    public DateTime? CalculateTimeToDo(DateTime reqDate, int? waitingTime, LookUp saturday2Wendsday, LookUp thursday, List<Holyday> holidays)
    {
        var finishDate = reqDate;
        var startTimeWork = int.Parse(saturday2Wendsday.Aux.Substring(0, 2));

        var endTimeWork = int.Parse(saturday2Wendsday.Aux2.Substring(0, 2));

        var startTimeWorkThu =
            int.Parse(thursday.Aux.Substring(0, 2));

        var endTimeWorkThu =
            int.Parse(thursday.Aux2.Substring(0, 2));

        finishDate = AddHour(waitingTime, reqDate, startTimeWork, endTimeWork, startTimeWorkThu,
            endTimeWorkThu, finishDate, holidays);

        return finishDate;
    }

    public IEnumerable<ThirdChartViewModel> CalculateSediment(int a, List<Holyday> holidays)
    {
        return RequestRepository.GetSediment(a, holidays);
    }

    public List<Flow> GetFlowList(Guid requestId, Guid? flowId, Guid workflowDetailId)
    {
        return DbContext.Flows
            .Where(f => f.RequestId == requestId && f.Id != flowId &&
                        f.WorkFlowDetailId == workflowDetailId && f.LookUpFlowStatus.Code == 1).ToList();
    }

    public IQueryable<string> GetRejectorPersonalCode(Guid staffId, Guid requestId)
    {
        return from reqs in DbContext.Requests
               join staffs in DbContext.Staffs on staffId equals staffs.Id
               where reqs.Id == requestId
               select staffs.PersonalCode;
    }

    public List<WorkFlowBoundary> GetWorkflowBoundaries(Guid workflowId)
    {
        return (from wfb in DbContext.WorkFlowBoundaries
                join wfd in DbContext.WorkFlowDetails on wfb.WorkflowDetailId equals wfd.Id
                join workflow in DbContext.Workflows on wfd.WorkFlowId equals workflow.Id
                where workflow.Id == workflowId && wfb.BoundaryId.Contains(BpmnNodeConstant.Boundary.BoundaryErrorEvent)
                select wfb).ToList();
    }


    public List<FlowAndWorkFlowDetailViewModel> GetFlowAndWorkFlowDetailByRequestIdAndStaffId(Guid requestId, Guid staffId)
    {
        return (from flow in DbContext.Flows
                join wfd in DbContext.WorkFlowDetails on flow.WorkFlowDetailId equals wfd.Id
                join lookUpStatus in DbContext.LookUps on flow.FlowStatusId equals lookUpStatus.Id
                join staff in DbContext.Staffs on flow.StaffId equals staff.Id
                where lookUpStatus.Code == 1 &&
                      wfd.IsScriptTask &&
                      flow.RequestId == requestId &&
                      staff.Id == staffId
                select new FlowAndWorkFlowDetailViewModel
                {
                    Flow = flow,
                    WorkFlowDetail = wfd
                }).ToList();
    }

    public List<FlowAndWorkFlowDetailViewModel> GetFlowAndWorkFlowDetailByRequestIdAndStaffIdForManualTask(
        Guid requestId, Guid staffId)
    {
        return (from flow in DbContext.Flows
                join wfd in DbContext.WorkFlowDetails on flow.WorkFlowDetailId equals wfd.Id
                join lookUpStatus in DbContext.LookUps on flow.FlowStatusId equals lookUpStatus.Id
                join staff in DbContext.Staffs on flow.StaffId equals staff.Id
                where lookUpStatus.Code == 1 &&
                      wfd.IsManualTask &&
                      flow.RequestId == requestId &&
                      staff.Id == staffId
                select new FlowAndWorkFlowDetailViewModel
                {
                    Flow = flow,
                    WorkFlowDetail = wfd
                }).ToList();
    }
    public List<FlowAndWorkFlowDetailViewModel> GetFlowAndWorkFlowDetailByRequestIdAndStaffIdForServiceTask(
        Guid requestId, Guid staffId)
    {
        return (from flow in DbContext.Flows
                join wfd in DbContext.WorkFlowDetails on flow.WorkFlowDetailId equals wfd.Id
                join lookUpStatus in DbContext.LookUps on flow.FlowStatusId equals lookUpStatus.Id
                join staff in DbContext.Staffs on flow.StaffId equals staff.Id
                where lookUpStatus.Code == 1 &&
                      wfd.IsServiceTask &&
                      flow.RequestId == requestId &&
                      staff.Id == staffId
                select new FlowAndWorkFlowDetailViewModel
                {
                    Flow = flow,
                    WorkFlowDetail = wfd
                }).ToList();
    }

    public List<FlowAndWorkFlowDetailAndBoudaryViewModel> GetFlowAndWorkFlowDetailAndBoudary()
    {
        return (from flows in DbContext.Flows
                join workflowDetails in DbContext.WorkFlowDetails on flows.WorkFlowDetailId equals workflowDetails.Id
                join boundary in DbContext.WorkFlowBoundaries on workflowDetails.Id equals boundary.WorkflowDetailId
                join status in DbContext.LookUps on flows.FlowStatusId equals status.Id
                where status.Code == 1 && boundary.BoundaryId.StartsWith(BpmnNodeConstant.Boundary.BoundaryTimerEvent)
                select new FlowAndWorkFlowDetailAndBoudaryViewModel
                {
                    Flow = flows,
                    WorkFlowDetail = workflowDetails,
                    WorkFlowBoundary = boundary
                }).ToList();


    }

    public List<FlowAndWorkFlowDetailAndBoudaryViewModel> GetByContainNonInterruptingBoundaryTimerEvent()
    {
        return (from flows in DbContext.Flows
                join workflowDetails in DbContext.WorkFlowDetails on flows.WorkFlowDetailId equals workflowDetails.Id
                join boundary in DbContext.WorkFlowBoundaries on workflowDetails.Id equals boundary.WorkflowDetailId
                join status in DbContext.LookUps on flows.FlowStatusId equals status.Id
                where status.Code == 1 &&
                      boundary.BoundaryId.Contains(BpmnNodeConstant.Boundary.NonInterruptingBoundaryTimerEvent)
                select new FlowAndWorkFlowDetailAndBoudaryViewModel
                {
                    Flow = flows,
                    WorkFlowDetail = workflowDetails,
                    WorkFlowBoundary = boundary
                }).ToList();
    }

    private DateTime AddHour(int? hour, DateTime reqDate, int startTimeWork, int endTimeWork, int startTimeWorkThu, int endTimeWorkThu, DateTime finishDate, List<Holyday> holidays)
    {
        var timeInHour = int.Parse(reqDate.ToString("HHmm").Substring(0, 2));
        var reqDateInt = int.Parse(reqDate.ToString("yyyyMMdd"));
        var holy = holidays.FirstOrDefault(d => d.Date == reqDateInt);
        if (holy != null)
        {
            //درخواست در روز تعطیل است
            if (holy.HolydayType.Code != 3)
            {
                finishDate = GetNewDate(reqDate, holidays);
                if (finishDate.DayOfWeek == DayOfWeek.Thursday)
                {
                    //timeInMinute = startTimeWorkThu;
                    finishDate = finishDate.Date.AddHours(startTimeWorkThu);
                    finishDate = AddHour(hour, finishDate, startTimeWork, endTimeWork, startTimeWorkThu,
                        endTimeWorkThu, finishDate, holidays);
                }
                else
                {
                    //timeInMinute = startTimeWork;
                    finishDate = finishDate.Date.AddHours(startTimeWork);
                    finishDate = AddHour(hour, finishDate, startTimeWork, endTimeWork, startTimeWorkThu,
                        endTimeWorkThu, finishDate, holidays);
                }
            }
            //درخواست در روز پنجشنبه   
            else
            {
                //زمان درخواست از زمان شروع روز پنجشنبه جلوتر است؟
                if (timeInHour < startTimeWorkThu)
                {
                    timeInHour = startTimeWorkThu;
                    finishDate = finishDate.Date.AddHours(startTimeWorkThu);
                }

                // زمان درخواست از زمان پایان روز پنجشنبه گذشته است؟
                if (timeInHour >= endTimeWorkThu)
                {
                    finishDate = GetNewDate(reqDate, holidays);
                    if (finishDate.DayOfWeek == DayOfWeek.Thursday)
                    {
                        finishDate = finishDate.Date.AddHours(startTimeWorkThu);
                        finishDate = AddHour(hour, finishDate, startTimeWork, endTimeWork, startTimeWorkThu,
                            endTimeWorkThu, finishDate, holidays);
                    }
                    else
                    {
                        finishDate = finishDate.Date.AddHours(startTimeWork);
                        finishDate = AddHour(hour, finishDate, startTimeWork, endTimeWork, startTimeWorkThu,
                            endTimeWorkThu, finishDate, holidays);
                    }
                }
                else
                {


                    //تفاضل زمان پایان روز پنجشنبه از زمان درخواست از WaitingTimeInMinute  بیش تر است؟ 
                    if (endTimeWorkThu - timeInHour > hour)
                    {
                        finishDate = finishDate.AddHours(hour.Value);
                    }
                    else
                    {
                        var time = endTimeWorkThu - timeInHour;
                        finishDate = finishDate.AddHours(time);

                        hour = hour - (time);

                        finishDate = AddHour(hour, finishDate, startTimeWork, endTimeWork, startTimeWorkThu,
                            endTimeWorkThu, finishDate, holidays);
                    }

                }


            }
        }
        // درخواست در روز عادی
        else
        {
            //زمان درخواست از زمان شروع روز عادی جلوتر است؟
            if (timeInHour < startTimeWork)
            {
                timeInHour = startTimeWork;
                finishDate = finishDate.Date.AddHours(startTimeWork);
            }

            // زمان درخواست از زمان پایان روز عادی گذشته است؟
            if (timeInHour >= endTimeWork)
            {
                finishDate = GetNewDate(reqDate, holidays);
                if (finishDate.DayOfWeek == DayOfWeek.Thursday)
                {
                    //timeInMinute = startTimeWorkThu;
                    finishDate = finishDate.Date.AddHours(startTimeWorkThu);
                    finishDate = AddHour(hour, finishDate, startTimeWork, endTimeWork, startTimeWorkThu,
                        endTimeWorkThu, finishDate, holidays);
                }
                else
                {
                    //timeInMinute = startTimeWork;
                    finishDate = finishDate.Date.AddHours(startTimeWork);
                    finishDate = AddHour(hour, finishDate, startTimeWork, endTimeWork, startTimeWorkThu,
                        endTimeWorkThu, finishDate, holidays);
                }
            }
            else
            {

                //تفاضل زمان پایان روز عادی از زمان درخواست از WaitingTimeInMinute  بیش تر است؟ 
                if (endTimeWork - timeInHour > hour)
                {
                    finishDate = finishDate.AddHours(hour.Value);
                    // return finishDate;
                }
                else
                {
                    var time = endTimeWork - timeInHour;
                    finishDate = finishDate.AddHours(time);

                    hour = hour - (time);

                    finishDate = AddHour(hour, finishDate, startTimeWork, endTimeWork, startTimeWorkThu,
                        endTimeWorkThu, finishDate, holidays);
                }

            }

        }
        return finishDate;
    }
    public Workflow GetWorkFlowIncludedTypeByIB(Guid Id)
    {
        return DbContext.Workflows.Include(c => c.RequestType)
            .Single(c => c.Id == Id);
    }

    public List<WorkFlowNextStep> GetWorkFlowNextSteps(string path, string boundaryId, Guid formWorkflowDetailId)
    {
        return DbContext.WorkFlowNextSteps
            .Include(p => p.NextStepToWfd)
            .Where(p => path == null || p.Path == path)
            .Where(w => w.FromWfdId == formWorkflowDetailId && w.BoundaryName.Contains(boundaryId))
            .ToList();

    }

    public List<WorkFlowNextStep> GetWorkFlowNextStepsWithoutBoundaryName(string path, Guid formWorkflowDetailId)
    {
        return DbContext.WorkFlowNextSteps
            .Include(p => p.NextStepToWfd)
            .Where(p => path == null || p.Path == path)
            .Where(w => w.FromWfdId == formWorkflowDetailId && w.BoundaryName == null)
            .ToList();
    }

    private static DateTime GetNewDate(DateTime date, List<Holyday> holidays)
    {
        var newDate = date.AddDays(1);
        var newDayInt = int.Parse(newDate.ToString("yyyyMMdd"));
        var holy = holidays.FirstOrDefault(d => d.Date == newDayInt);
        if (holy != null)
        {
            if (holy.HolydayType.Code != 3)
            {
                newDate = GetNewDate(newDate, holidays);
            }
        }

        return newDate;
    }

    private static long GetDelayAsHour(int compareMin, int reqMin, long hour)
    {
        if (compareMin <= reqMin)
            return --hour;
        return hour;

    }
    private static long GetDoneTimeAsHour(int compareMin, int reqMin, long hour)
    {
        if (hour > 0 && compareMin < reqMin)
            return --hour;
        return hour;

    }

}