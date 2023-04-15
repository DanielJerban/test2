using BPMS.Domain.Common.Constants;
using BPMS.Domain.Common.Enums;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.Repositories;
using BPMS.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Diagnostics;
using System.Globalization;
using Exceptions = BPMS.Infrastructure.MainHelpers.CustomExceptionHandler;

namespace BPMS.Application.Services;

public class JobService : IJobService
{
    private readonly IConfiguration _configuration;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IDynamicFormService _dynamicForm;
    private readonly IFlowRepository _flowRepository;
    private readonly IFlowService _flowService;
    private readonly IReportWareHouseService _reportWareHouseService;
    private readonly ILDAPService _ldapService;

    public JobService(IConfiguration configuration, IUnitOfWork unitOfWork, IDynamicFormService dynamicForm, IFlowRepository flowRepository, IFlowService flowService, IReportWareHouseService reportWareHouseService, ILDAPService ldapService)
    {
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _dynamicForm = dynamicForm;
        _flowRepository = flowRepository;
        _flowService = flowService;
        _reportWareHouseService = reportWareHouseService;
        _ldapService = ldapService;
    }

    public void SendReminderMessage()
    {
        try
        {
            Guid statusId = Guid.Empty;
            //ایدی مربوط به وضعیت اقدام نشده
            var status = _unitOfWork.LookUps.Find(d => d.Type == "FlowStatus" && d.Code == 1).SingleOrDefault();
            if (status != null)
                statusId = status.Id;
            var result = (from flow in _unitOfWork.Flows.GetAll()
                          join staff in _unitOfWork.Staffs.GetAll() on flow.StaffId equals staff.Id
                          where flow.FlowStatusId == statusId && flow.IsActive
                          select new
                          {
                              flow.Id,
                              staff.FName,
                              staff.LName,
                              staff.PersonalCode,
                              staff.Gender
                          }).Distinct();

            var personelCodesToSend = (from en in result
                                       group new { en.Id } by new
                                       {
                                           en.FName,
                                           en.LName,
                                           en.PersonalCode,
                                           en.Gender
                                       }
                into grp
                                       orderby grp.Count() descending
                                       select new
                                       {
                                           Count = grp.Count(),
                                           FullName = grp.Key.FName + " " + grp.Key.LName,
                                           PersonelCode = grp.Key.PersonalCode,
                                           Gender = grp.Key.Gender
                                       }).ToList();


            foreach (var personel in personelCodesToSend)
            {

                string gender;
                if (personel.Gender == 1)
                {
                    gender = " جناب آقای ";
                }
                else if (personel.Gender == 2)
                {
                    gender = " سرکار خانم ";
                }
                else
                {
                    gender = " همکار گرامی ";
                }
                var content = "<p>"
                              + gender + personel.FullName + "</br> شما " + personel.Count +
                              @" درخواست اقدام نشده در سامانه مدیریت فرآیند های کسب و کار دارید 
                                      لطفا برای پاسخ به درخواست های خود از لینک زیر استفاده کنید.
                                      </p>
                                      <a target='_blank' href='/fa/site/user/bpmslogin'>https://bpms.tnc.ir</a>";
                var uri = new Uri("http://172.16.0.12/api/sendletter");
                var clients = new RestClient(uri);
                var requests = new RestRequest(uri, Method.Post) { Timeout = 10000 };
                requests.AddParameter("sender_pc", SystemConstant.SystemUser);
                requests.AddParameter("receivers_pc", personel.PersonelCode);
                requests.AddParameter("content", content);
                requests.AddParameter("subject", "گزارش وضعیت درخواست ها");
                requests.AddParameter("password",
                    HashPassword.EncodePasswordMd5("anx7gr").Replace("-", "").ToLower());
                requests.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                clients.Execute(requests);
            }
        }
        catch (Exception e)
        {
            var exceptions = new Exceptions();
            exceptions.HandleException(e);
        }
    }

    public void AutoAcceptAndReject()
    {
        var s2W = _unitOfWork.LookUps.GetAsQueryable().SingleOrDefault(d => d.Type == "WorkTime" && d.Code == 1);
        var thr = _unitOfWork.LookUps.GetAsQueryable().SingleOrDefault(d => d.Type == "WorkTime" && d.Code == 2);
        var holyday = _unitOfWork.Holydays.GetAsQueryable().ToList();

        var datetime = DateTime.Now;

        var query = from flows in _unitOfWork.Flows.GetAsQueryable()
                    join previousflows in _unitOfWork.Flows.GetAsQueryable() on flows.PreviousFlowId equals previousflows.Id into leftjoin
                    from previousflows in leftjoin.DefaultIfEmpty()
                    join requests in _unitOfWork.Request.GetAsQueryable() on flows.RequestId equals requests.Id
                    join workflowDetails in _unitOfWork.WorkflowDetails.GetAsQueryable() on flows.WorkFlowDetailId equals workflowDetails.Id
                    join status in _unitOfWork.LookUps.GetAsQueryable() on flows.FlowStatusId equals status.Id
                    where status.Code == 1 && workflowDetails.WaithingTImeForAct != null
                    select new
                    {
                        flows,
                        workflowDetails,
                        previousflows,
                        requests
                    };

        var queryResult = query.ToList();
        //query = query.ToList();

        foreach (var item in queryResult)
        {
            var reqDate = DateTime.ParseExact((item.previousflows != null ? item.previousflows.ResponseDate + item.previousflows.ResponseTime : item.requests.RegisterDate + item.requests.RegisterTime), "yyyyMMddHHmm", null, DateTimeStyles.None);
            //  var diff = datetime.Subtract(reqDate).TotalMinutes;
            var diff = _flowRepository.CalculateDelay(reqDate, datetime, s2W, thr, holyday);
            var minute = item.workflowDetails.WaithingTImeForAct * 60;
            if (!(diff > minute)) continue;

            var act = item.workflowDetails.Act == "A" ? "Accept" : "Reject";
            var apikey = _unitOfWork.LookUps.GetAll().FirstOrDefault(l => l.Type == "ApiKey" && l.Code == 1 && l.IsActive).Aux2;
            var client = new RestClient("https://bpms.tnc.ir/api/DynamicForm/" + act);
            var request = new RestRequest(new Uri("https://bpms.tnc.ir/api/DynamicForm/"), Method.Post) { Timeout = 10000 };
            request.AddParameter("StaffId", item.flows.StaffId);
            request.AddParameter("ApiKey", apikey);
            request.AddParameter("FlowId", item.flows.Id);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            client.Execute(request);
        }
    }

    public void ActivateInterruptingBoundaryTimer()
    {
        _flowService.CheckWorkFLowBoundaryForSchedule();
    }

    public void ActivateNonInterruptingBoundaryTimer(string webRootPath)
    {
        _flowService.CheckWorkFLowNonInterruptingBoundaryForSchedule(webRootPath);
    }

    public void ActivateIntermediateTimerNotation()
    {
        _flowService.CheckIntermediateTimerForSchedule();
    }

    public void CalculateAllRequestsDelay()
    {
        _unitOfWork.AllFlowsWithDelayLogs.UpdateTableLogSchedule();
    }

    public void SubPersonnelDelayedRequests()
    {
        try
        {
            var managers = _unitOfWork.Reports.GetManagers();

            foreach (var manager in managers)
            {
                var data = _unitOfWork.Reports.GetDelayedReport_AdminSubUsers(manager.Id);

                List<RequestDelayExcelViewModel> excelData = new List<RequestDelayExcelViewModel>();
                foreach (var item in data)
                {
                    excelData.Add(new RequestDelayExcelViewModel()
                    {
                        ApplicantName = item.ApplicantName,
                        SubprocessName = item.SubprocessName,
                        TimeToDo = item.TimeToDo,
                        FlowNameAndVersion = item.FlowNameAndVersion,
                        RequestDateTime = item.RequestDateTime,
                        DelayHour = item.DelayHour,
                        FlowLevelName = item.FlowLevelName,
                        PersonalName = item.PersonalName,
                        RequestNumber = item.RequestNumber
                    });
                }

                var fileBytes = HelperBs.ConvertToExcel(excelData.OrderByDescending(c => c.DelayHour));
                var client = new RestClient("http://172.16.0.12/webservice/letter");
                var request = new RestRequest(new Uri("http://172.16.0.12/webservice/letter"), Method.Post);
                request.AddHeader("Authorization", "Bearer tnc_automation_webservice_Letter_apikey");
                request.AddFile("letter_file", fileBytes, "گزارش درخواست های تاخیر دار.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                request.AddParameter("sender_pc", SystemConstant.SystemUser);
                request.AddParameter("receivers_pc", manager.PersonalCode);
                request.AddParameter("subject", "لیست درخواست های دارای تاخیر پرسنل زیرمجموعه من در سامانه مدیریت فرآیند های کسب و کار طرفه نگار ");
                request.AddParameter("content", "لیست درخواست های دارای تاخیر زیرمجموعه در سامانه مدیریت فرآیند های کسب و کار طرفه نگار به پیوست تقدیم می گردد.");
                RestResponse response = client.Execute(request);
            }
        }
        catch (Exception e)
        {
            var exceptions = new Exceptions();
            exceptions.HandleException(e);
        }
    }

    public void RunTimerStartEvent(string webRootPath)
    {
        var list = _unitOfWork.Flows.GetTimerStartEvents();

        foreach (var workflowId in list)
        {
            var requestTypeId = _unitOfWork.Workflows.Single(a => a.Id == workflowId).RequestTypeId;

            var staffId = _unitOfWork.Staffs.Single(c => c.PersonalCode == SystemConstant.SystemUser).Id;
            var organizationPostTitleId = _unitOfWork.LookUps.Single(c => c.Type == "OrganizationPostTitle" && c.Code == 1).Id;

            _dynamicForm.IsViewNameCreateProcess(requestTypeId, organizationPostTitleId, Guid.NewGuid(), staffId, out _, out _, null, SystemConstant.SystemUser);

            var model = HelperBs.EncodeUri("{}");

            // todo: uncomment later
            //_dynamicForm.Create(model, Guid.Empty, Guid.NewGuid(), "null", "[]", SystemConstant.SystemUser, null, null, webRootPath);

            _unitOfWork.Flows.SetTimerLastRunDate(workflowId);

            _unitOfWork.Complete();
        }
    }

    public void CalculateAverageProcessingLog()
    {
        _reportWareHouseService.SetAverageRequestProcessingTime();
    }

    public void SyncLdapUsers()
    {
        _ldapService.LDAPUserSyncTimerEnable();
    }

    public void ExecuteJob(ScheduleType scheduleType, string webRootPath)
    {
        var startTime = DateTime.Now.TimeOfDay;
        bool success;
        string errorMessage = "";
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        try
        {
            switch (scheduleType)
            {
                case ScheduleType.CalculateAverageProcessing:
                    CalculateAverageProcessingLog();
                    break;
                case ScheduleType.ActivateLdapSyncer:
                    SyncLdapUsers();
                    break;
                case ScheduleType.ActivateNonInterruptBoundary:
                    ActivateNonInterruptingBoundaryTimer(webRootPath);
                    break;
                case ScheduleType.ActivateInterruptBoundary:
                    ActivateInterruptingBoundaryTimer();
                    break;
                case ScheduleType.ActivateIntermediateTimerNotation:
                    ActivateIntermediateTimerNotation();
                    break;
                case ScheduleType.CalculateAllRequestsDelay:
                    CalculateAllRequestsDelay();
                    break;
                case ScheduleType.RunTimerStartEvent:
                    RunTimerStartEvent(webRootPath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scheduleType), scheduleType, null);
            }

            success = true;
        }
        catch (Exception e)
        {
            success = false;
            errorMessage = $"Exception: {e.Message}";
            if (e.InnerException != null)
            {
                errorMessage += $"\nInner Exception: {e.InnerException.Message}";
            }
        }
        finally
        {
            stopWatch.Stop();
        }

        var endTime = DateTime.Now.TimeOfDay;


        var log = ScheduleLifeTimeLog.CreateNew(scheduleType, startTime, endTime, stopWatch.Elapsed, success, errorMessage);
        _unitOfWork.ScheduleLifeTimeLog.Add(log);
        _unitOfWork.Complete();
    }
}