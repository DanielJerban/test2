using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using BPMS.Application.Hubs;
using BPMS.Infrastructure.Services.SMS;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS.Application.Repositories;

public class RequestRepository : Repository<Request>, IRequestRepository
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISendingSmsService _sendingSmsService;
    public RequestRepository(BpmsDbContext context, IServiceProvider serviceProvider) : base(context)
    {
        _serviceProvider = serviceProvider;
        _sendingSmsService = serviceProvider.GetRequiredService<ISendingSmsService>();
    }
    public BpmsDbContext DbContext => Context;

    public ILookUpRepository LookUpRepository => _serviceProvider.GetRequiredService<ILookUpRepository>();
    public IFlowRepository FlowRepository => _serviceProvider.GetRequiredService<IFlowRepository>();

    public IEnumerable<RequestViewModel> GetRequestForManagment(string condition)
    {
        var code = @"SELECT 
                        [Requests].[RegisterDate], 
                        [Requests].[Id] AS [Id], 
                        [Staffs].[FName] + N' ' + [Staffs].[LName] as 'Staff' , 
                        [Requests].[RequestNo] , 
                        [RequestStatus].[Title] AS 'RequestStatus', 
                        [Requests].[RegisterTime] AS [RegisterTime], 
                        [RequestType].[Title] AS 'RequestType', 
                         CAST( [Workflows].[OrginalVersion] AS nvarchar(max)) + N'.' +  CAST( [Workflows].[SecondaryVersion] AS nvarchar(max)) AS 'Version', 
                        [OrganizationPostTitle].[Title] AS 'OrganizationPostTitle'
                        FROM      [dbo].[Requests] 
                        INNER JOIN [dbo].[Staffs]  ON [Requests].[StaffId] = [Staffs].[Id]
                        --INNER JOIN [dbo].[Flows]  ON [Requests].[Id] = [Flows].[RequestId]
                        INNER JOIN [dbo].[LookUps] AS [RequestStatus] ON [Requests].[RequestStatusId] = [RequestStatus].[Id]
                        INNER JOIN [dbo].[Workflows]  ON [Requests].[WorkFlowId] = [Workflows].[Id]
                        INNER JOIN [dbo].[LookUps] AS [RequestType] ON [Workflows].[RequestTypeId] = [RequestType].[Id]
                        INNER JOIN [dbo].[LookUps] AS [OrganizationPostTitle] ON [Requests].[OrganizationPostTitleId] = [OrganizationPostTitle].[Id] " + condition;

        var re = DbContext.Database.SqlQuery<RequestViewModel>($"{code}");

        var status = DbContext.LookUps.FirstOrDefault(i => i.Code == 1 && i.Type == "FlowStatus");
        var result = new List<RequestViewModel>();
        foreach (var item in re)
        {
            if (item.Id == Guid.Parse("83711FB3-E4A9-417A-9069-8A5206730867"))
            {

            }
            var flowId = status != null
                ? (DbContext.Flows.Where(i => i.RequestId == item.Id && i.FlowStatusId == status.Id)
                    .OrderByDescending(i => i.DelayDate).ThenByDescending(i => i.DelayTime).Select(i => i.Id)
                    .FirstOrDefault())
                : Guid.Empty;

            result.Add(new RequestViewModel()
            {
                FlowId = flowId.ToString(),
                RequestNo = item.RequestNo,
                RegisterDate = item.RegisterDate,
                Id = item.Id,
                Staff = item.Staff,
                RequestStatus = item.RequestStatus,
                RegisterTime = item.RegisterTime,
                RequestType = item.RequestType,
                OrganizationPostTitle = item.OrganizationPostTitle,
                Version = item.Version
            });

        }

        return result;
    }

    public List<PieChartViewModel> GetFlowPieChartData(string username)
    {
        List<PieChartViewModel> pieChartData = new List<PieChartViewModel>();
        var currentUser = DbContext.Users.Single(c => c.UserName == username);
        var query1 = from w in DbContext.WorkFlowConfermentAuthority
                     join wd in DbContext.WorkFlowConfermentAuthorityDetails on w.Id equals wd.ConfermentAuthorityId
                     join workflows in DbContext.Workflows on w.RequestTypeId equals workflows.RequestTypeId
                     join request in DbContext.Requests on workflows.Id equals request.WorkFlowId
                     join flow in DbContext.Flows on request.Id equals flow.RequestId
                     where wd.StaffId == currentUser.StaffId && flow.LookUpFlowStatus.Code == 1 && flow.StaffId == w.StaffId && (request.RegisterDate >= wd.FromDate && request.RegisterDate <= wd.ToDate)
                           && (wd.OnlyOwnRequest ? request.StaffId == currentUser.StaffId : true)
                     select flow;
        var query2 = from f in DbContext.Flows
                     where f.StaffId == currentUser.StaffId && f.LookUpFlowStatus.Code == 1
                     select f;
        var requestsNotInProgress = query1.Union(query2).ToList().Count;
        var accQuery1 = from f in DbContext.Flows
                        where f.StaffId == currentUser.StaffId && f.LookUpFlowStatus.Code == 2
                        select f;
        var accQuery2 = from w in DbContext.WorkFlowConfermentAuthority
                        join wd in DbContext.WorkFlowConfermentAuthorityDetails on w.Id equals wd.ConfermentAuthorityId
                        join workflows in DbContext.Workflows on w.RequestTypeId equals workflows.RequestTypeId
                        join request in DbContext.Requests on workflows.Id equals request.WorkFlowId
                        join flow in DbContext.Flows on request.Id equals flow.RequestId
                        where wd.StaffId == currentUser.StaffId && flow.LookUpFlowStatus.Code == 2 && flow.StaffId == w.StaffId && (request.RegisterDate >= wd.FromDate && request.RegisterDate <= wd.ToDate)
                              && (wd.OnlyOwnRequest ? request.StaffId == currentUser.StaffId : true)
                        select flow;
        var acceptedFlows = accQuery1.Union(accQuery2).ToList().Count;
        var rejQuery1 = from f in DbContext.Flows
                        where f.StaffId == currentUser.StaffId && f.LookUpFlowStatus.Code == 3
                        select f;
        var rejQuery2 = from w in DbContext.WorkFlowConfermentAuthority
                        join wd in DbContext.WorkFlowConfermentAuthorityDetails on w.Id equals wd.ConfermentAuthorityId
                        join workflows in DbContext.Workflows on w.RequestTypeId equals workflows.RequestTypeId
                        join request in DbContext.Requests on workflows.Id equals request.WorkFlowId
                        join flow in DbContext.Flows on request.Id equals flow.RequestId
                        where wd.StaffId == currentUser.StaffId && flow.LookUpFlowStatus.Code == 3 && flow.StaffId == w.StaffId && (request.RegisterDate >= wd.FromDate && request.RegisterDate <= wd.ToDate)
                              && (wd.OnlyOwnRequest ? request.StaffId == currentUser.StaffId : true)
                        select flow;
        var rejectedFlows = rejQuery1.Union(rejQuery2).ToList().Count;
        pieChartData.Add(new PieChartViewModel() { category = "دریافت شده اقدام نشده", value = requestsNotInProgress, Tooltip = requestsNotInProgress, color = "#FF9F1C" });

        pieChartData.Add(new PieChartViewModel() { category = "تایید شده", value = acceptedFlows, Tooltip = acceptedFlows, color = "#04A777" });

        pieChartData.Add(new PieChartViewModel() { category = "رد شده", value = rejectedFlows, Tooltip = rejectedFlows, color = "#D90429" });
        if (requestsNotInProgress == 0 && acceptedFlows == 0 && rejectedFlows == 0)
        {
            List<PieChartViewModel> emptyPieChart = new List<PieChartViewModel>() { new PieChartViewModel() { category = "", value = 100, color = "#CED6E9", Tooltip = 0 } };
            return emptyPieChart;
        }
        else
        {
            return pieChartData;
        }
    }

    public List<PieChartViewModel> GetRequestPieChartData(string username)
    {
        List<PieChartViewModel> pieChartData = new List<PieChartViewModel>();
        var sendnotInProgressIdCount = GetDatasByRequestStatus(DbContext.LookUps.Single(c => c.Code == 1 && c.Type == "RequestStatus").Id, username);
        pieChartData.Add(new PieChartViewModel() { category = "ارسال شده اقدام نشده", value = sendnotInProgressIdCount, color = "#FF9F1C", Tooltip = sendnotInProgressIdCount });
        var inProgressCount = GetDatasByRequestStatus(DbContext.LookUps.Single(c => c.Code == 2 && c.Type == "RequestStatus").Id, username);
        pieChartData.Add(new PieChartViewModel() { category = "ارسال شده در حال اقدام", value = inProgressCount, color = "#E9C46A", Tooltip = inProgressCount });
        var finishedAccepted = GetDatasByRequestStatus(DbContext.LookUps.Single(c => c.Code == 3 && c.Type == "RequestStatus").Id, username);
        pieChartData.Add(new PieChartViewModel() { category = "خاتمه یافته تایید شده", value = finishedAccepted, color = "#04A777", Tooltip = finishedAccepted });
        var finishedRejected = GetDatasByRequestStatus(DbContext.LookUps.Single(c => c.Code == 4 && c.Type == "RequestStatus").Id, username);
        pieChartData.Add(new PieChartViewModel() { category = "خاتمه یافته رد شده", value = finishedRejected, color = "#D90429", Tooltip = finishedRejected });
        if (sendnotInProgressIdCount == 0 && inProgressCount == 0 && finishedAccepted == 0 && finishedRejected == 0)
        {
            List<PieChartViewModel> emptyPieChart = new List<PieChartViewModel>() { new PieChartViewModel() { category = "", value = 100, color = "#CED6E9", Tooltip = 0 } };
            return emptyPieChart;
        }

        return pieChartData;
    }

    /// <summary>
    /// Information for charts in the main page
    /// </summary>
    /// <returns></returns>
    public ChartsViewModel GetChartsData(string username)
    {
        var flowPieChartData = GetFlowPieChartData(username);
        var requestPieChartData = GetRequestPieChartData(username);
        var viewModel = new ChartsViewModel() { FlowPieChart = flowPieChartData, RequestPieChart = requestPieChartData };
        return viewModel;
    }

    /// <summary>
    /// تعداد درخواست های دریافت شده اقدام نشده، ارسال شده در حال اقدام، ارسال شده خاتمه یافته تایید شده خاتمه یافته رد شده را به صورت تعداد در بالای سمت راست صفحه نشان می دهد.
    /// </summary>
    /// <returns></returns>
    public int GetCountOfAllNotification(Guid staffId)
    {
        //تعداد درخواست های دریافت شده اقدام نشده را محاسبه می کند 
        //تفویض اختیار هم لحاظ شده است
        var query1 = from w in DbContext.WorkFlowConfermentAuthority
                     join wd in DbContext.WorkFlowConfermentAuthorityDetails on w.Id equals wd.ConfermentAuthorityId
                     join workflows in DbContext.Workflows on w.RequestTypeId equals workflows.RequestTypeId
                     join request in DbContext.Requests on workflows.Id equals request.WorkFlowId
                     join flow in DbContext.Flows on request.Id equals flow.RequestId
                     where wd.StaffId == staffId && flow.StaffId == w.StaffId && (request.RegisterDate >= wd.FromDate && request.RegisterDate <= wd.ToDate)
                           && (!wd.OnlyOwnRequest || request.StaffId == staffId) && flow.IsActive && flow.IsRead == false
                     select flow;
        var query2 = from f in DbContext.Flows
                     where f.StaffId == staffId && f.IsActive && f.IsRead == false
                     select f;
        var flowsTotalCount = query1.Union(query2).Count();
        ////تعداد درخواست های ارسال شده درحال اقدام، خاتمه یافته تایید شده و خاتمه یافته رد شده را محاسبه می کند 
        //var requestsCount = from req in _dbContext.Requests
        //    join flow in _dbContext.Flows
        //    on req.Id equals flow.RequestId
        //    where !flow.IsBalloon && req.RequestStatus.Code != 1 && req.RequestStatus.Code != 2 && req.StaffId == GlobalVariables.User.StaffId
        //    select req;
        //var totalrequestsCount = requestsCount.DistinctBy(r => r.RequestNo).Count();
        return flowsTotalCount;
    }

    public void ChangeIsBalloonStatusWhanMessagesRead(Guid requestId)
    {
        var flowsToChangeBallonStatus = DbContext.Flows.Where(f => f.RequestId == requestId).ToList();
        foreach (var item in flowsToChangeBallonStatus)
        {
            item.IsBalloon = true;
            DbContext.Flows.Update(item);
        }
    }

    public float GetDatasByRequestStatus(Guid requestStatusId, string username)
    {
        var staffId = DbContext.Users.Single(c => c.UserName == username).StaffId;
        var count = DbContext.Requests.Count(f => f.RequestStatusId == requestStatusId && f.StaffId == staffId);
        return count;
    }

    public IEnumerable<Flow> GetFlowsRequest(Guid? statusId, string usernamne)
    {
        var user = DbContext.Users.Single(c => c.UserName == usernamne);
        if (statusId != Guid.Empty)
        {
            var query1 = from w in DbContext.WorkFlowConfermentAuthority
                         join wd in DbContext.WorkFlowConfermentAuthorityDetails on w.Id equals wd.ConfermentAuthorityId
                         join workflows in DbContext.Workflows on w.RequestTypeId equals workflows.RequestTypeId
                         join request in DbContext.Requests on workflows.Id equals request.WorkFlowId
                         join flow in DbContext.Flows on request.Id equals flow.RequestId
                         where wd.StaffId == user.StaffId && flow.FlowStatusId == statusId && flow.StaffId == w.StaffId && (request.RegisterDate >= wd.FromDate && request.RegisterDate <= wd.ToDate)
                               && (!wd.OnlyOwnRequest || request.StaffId == user.StaffId) && flow.IsActive
                         select flow;
            var query2 = from f in DbContext.Flows

                         where f.StaffId == user.StaffId && f.FlowStatusId == statusId && f.IsActive
                         select f;
            return query1.Union(query2).ToList();
        }
        else
        {
            var query1 = from w in DbContext.WorkFlowConfermentAuthority
                         join wd in DbContext.WorkFlowConfermentAuthorityDetails on w.Id equals wd.ConfermentAuthorityId
                         join workflows in DbContext.Workflows on w.RequestTypeId equals workflows.RequestTypeId
                         join request in DbContext.Requests on workflows.Id equals request.WorkFlowId
                         join flow in DbContext.Flows on request.Id equals flow.RequestId
                         where wd.StaffId == user.StaffId && (request.RegisterDate >= wd.FromDate && request.RegisterDate <= wd.ToDate)
                                                          && (!wd.OnlyOwnRequest || request.StaffId == user.StaffId) && flow.IsActive
                         select flow;
            var query2 = from f in DbContext.Flows
                         where f.StaffId == user.StaffId && f.IsActive
                         select f;
            var all = query1.Union(query2).ToList();
            return all;
        }
    }

    public IEnumerable<Flow> ExternalGetFlowsRequest(Guid? statusId, Guid staffId)
    {
        if (statusId != Guid.Empty)
        {
            var query1 = from w in DbContext.WorkFlowConfermentAuthority
                         join wd in DbContext.WorkFlowConfermentAuthorityDetails on w.Id equals wd.ConfermentAuthorityId
                         join workflows in DbContext.Workflows on w.RequestTypeId equals workflows.RequestTypeId
                         join request in DbContext.Requests on workflows.Id equals request.WorkFlowId
                         join flow in DbContext.Flows on request.Id equals flow.RequestId
                         where wd.StaffId == staffId && flow.FlowStatusId == statusId && flow.StaffId == w.StaffId && (request.RegisterDate >= wd.FromDate && request.RegisterDate <= wd.ToDate)
                               && (!wd.OnlyOwnRequest || request.StaffId == staffId) && flow.IsActive
                         select flow;
            var query2 = from f in DbContext.Flows
                         where f.StaffId == staffId && f.FlowStatusId == statusId && f.IsActive
                         select f;
            return query1.Union(query2).ToList();
        }
        else
        {
            var query1 = from w in DbContext.WorkFlowConfermentAuthority
                         join wd in DbContext.WorkFlowConfermentAuthorityDetails on w.Id equals wd.ConfermentAuthorityId
                         join workflows in DbContext.Workflows on w.RequestTypeId equals workflows.RequestTypeId
                         join request in DbContext.Requests on workflows.Id equals request.WorkFlowId
                         join flow in DbContext.Flows on request.Id equals flow.RequestId
                         where wd.StaffId == staffId && (request.RegisterDate >= wd.FromDate && request.RegisterDate <= wd.ToDate)
                                                     && (!wd.OnlyOwnRequest || request.StaffId == staffId) && flow.IsActive
                         select flow;
            var query2 = from f in DbContext.Flows
                         where f.StaffId == staffId && f.IsActive
                         select f;
            var all = query1.Union(query2).ToList();
            return all;
        }
    }

    public IEnumerable<FlowsInCartbotViewModel> GetFlows(Guid id, string username)
    {

        var datalist = new List<FlowsInCartbotViewModel>();
        var s2w = LookUpRepository.GetByTypeAndCode("WorkTime", 1);
        var thr = LookUpRepository.GetByTypeAndCode("WorkTime", 2);
        var holyday = DbContext.Holydays.ToList();

        if (id == Guid.Parse("6464A047-285C-4340-A4B3-21BC662726C8"))
        {
            var recievedData = GetFlowsRequest(Guid.Empty, username);

            datalist.AddRange(
                from data in recievedData
                select new FlowsInCartbotViewModel()
                {
                    RequestNo = data.Request.RequestNo,
                    FullName = data.Request.Staff.FullName,
                    PersonalCode = data.Request.Staff.PersonalCode,
                    CurrentStatus = data.LookUpFlowStatus.Title,
                    RequestDate = HelperBs.MakeDate(data.Request.RegisterDate.ToString()),
                    RequestTime = HelperBs.MakeTime(data.Request.RegisterTime),
                    StepTitle = data.WorkFlowDetail.Title,
                    FlowId = data.Id,
                    RequestTypeTitle = data.WorkFlowDetail.WorkFlow.RequestType.Title + " / نسخه " + data.WorkFlowDetail.WorkFlow.OrginalVersion + "." + data.WorkFlowDetail.WorkFlow.SecondaryVersion,
                    RequestId = data.RequestId,
                    RequestTypeId = data.WorkFlowDetail.WorkFlow.RequestTypeId,
                    IsBalloon = data.IsBalloon,
                    WorkflowDetailId = data.WorkFlowDetailId,
                    IsMultiConfirmReject = data.WorkFlowDetail.IsMultiConfirmReject,
                    Message = data.PreviousFlow?.Dsr,
                    StaffId = data.StaffId,
                    IsRead = data.IsRead
                });
            // return datalist;
        }
        else
        {
            var recievedData = GetFlowsRequest(id, username);
            datalist.AddRange(
                from data in recievedData
                let reqDate = DateTime.ParseExact(data.DelayDate + data.DelayTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                let timeToDo = FlowRepository.CalculateTimeToDo(reqDate, data.WorkFlowDetail.WaitingTime, s2w, thr, holyday)
                let diff = Math.Round(FlowRepository.CalculateDelay(reqDate, DateTime.Now, s2w, thr, holyday))
                let sediment = (diff > data.WorkFlowDetail.WaitingTime) ? diff - data.WorkFlowDetail.WaitingTime : 0
                select new FlowsInCartbotViewModel()
                {
                    RequestNo = data.Request.RequestNo,
                    FullName = data.Request.Staff.FullName,
                    PersonalCode = data.Request.Staff.PersonalCode,
                    CurrentStatus = data.LookUpFlowStatus.Title,
                    RequestDate = HelperBs.MakeDate(data.Request.RegisterDate.ToString()),
                    RequestTime = HelperBs.MakeTime(data.Request.RegisterTime),
                    StepTitle = data.WorkFlowDetail.Title,
                    FlowId = data.Id,
                    RequestTypeTitle = data.WorkFlowDetail.WorkFlow.RequestType.Title + " / نسخه " + data.WorkFlowDetail.WorkFlow.OrginalVersion + "." + data.WorkFlowDetail.WorkFlow.SecondaryVersion,
                    RequestId = data.RequestId,
                    RequestTypeId = data.WorkFlowDetail.WorkFlow.RequestTypeId,
                    IsBalloon = data.IsBalloon,
                    WorkflowDetailId = data.WorkFlowDetailId,
                    IsMultiConfirmReject = data.WorkFlowDetail.IsMultiConfirmReject,
                    Message = data.PreviousFlow?.Dsr,
                    StaffId = data.StaffId,
                    TimeToDo = timeToDo?.ToString("yyyy/MM/dd HH:mm"),
                    IsRed = timeToDo < DateTime.Now,
                    Delay = sediment.ToString(),
                    IsRead = data.IsRead
                });

        }
        return datalist;
    }

    public IEnumerable<ExternalFlowsInCartbotViewModel> ExternalGetFlows(Guid id, Guid staffId)
    {
        var datalist = new List<ExternalFlowsInCartbotViewModel>();
        var isOnline = "f";

        if (id == Guid.Parse("6464A047-285C-4340-A4B3-21BC662726C8"))
        {
            var recievedData = ExternalGetFlowsRequest(Guid.Empty, staffId);
            foreach (var data in recievedData)
            {
                var onlineStatus = MainHub.CheckStaffIsOnline(data.Request.Staff.PersonalCode);
                isOnline = onlineStatus ? "o" : "f";
                datalist.Add(new ExternalFlowsInCartbotViewModel()
                {
                    RequestNo = data.Request.RequestNo,
                    FullName = data.Request.Staff.FullName,
                    PersonalCode = data.Request.Staff.PersonalCode,
                    CurrentStatus = data.LookUpFlowStatus.Title,
                    RequestDate = HelperBs.MakeDate(data.Request.RegisterDate.ToString()),
                    RequestTime = HelperBs.MakeTime(data.Request.RegisterTime),
                    StepTitle = data.WorkFlowDetail.Title,
                    FlowId = data.Id,
                    RequestTypeTitle = data.WorkFlowDetail.WorkFlow.RequestType.Title,
                    RequestId = data.RequestId,
                    RequestTypeId = data.WorkFlowDetail.WorkFlow.RequestTypeId,
                    IsBalloon = data.IsBalloon,
                    IsOnline = isOnline,
                    WorkflowDetailId = data.WorkFlowDetailId,
                    // ImagePath = ExternalGetImagePath(data.Request.Staff.PersonalCode)
                    ImagePath = "/images" + data.Request.Staff.ImagePath

                });
            }
            return datalist;
        }
        else
        {
            var recievedData = ExternalGetFlowsRequest(id, staffId);
            foreach (var data in recievedData)
            {
                var onlineStatus = MainHub.CheckStaffIsOnline(data.Request.Staff.PersonalCode);
                isOnline = onlineStatus ? "o" : "f";
                datalist.Add(new ExternalFlowsInCartbotViewModel()
                {
                    RequestNo = data.Request.RequestNo,
                    FullName = data.Request.Staff.FullName,
                    PersonalCode = data.Request.Staff.PersonalCode,
                    CurrentStatus = data.LookUpFlowStatus.Title,
                    RequestDate = HelperBs.MakeDate(data.Request.RegisterDate.ToString()),
                    RequestTime = HelperBs.MakeTime(data.Request.RegisterTime),
                    StepTitle = data.WorkFlowDetail.Title,
                    FlowId = data.Id,
                    RequestTypeTitle = data.WorkFlowDetail.WorkFlow.RequestType.Title,
                    RequestId = data.RequestId,
                    RequestTypeId = data.WorkFlowDetail.WorkFlow.RequestTypeId,
                    IsBalloon = data.IsBalloon,
                    IsOnline = isOnline,
                    WorkflowDetailId = data.WorkFlowDetailId,
                    // ImagePath = ExternalGetImagePath(data.Request.Staff.ImagePath)
                    ImagePath = "/images" + data.Request.Staff.ImagePath

                });
            }
            return datalist;
        }
    }


    public IEnumerable<RequestsInCartbotViewModel> GetRequests(Guid id, string username)
    {
        var user = DbContext.Users.Single(c => c.UserName == username);

        IEnumerable<RequestsInCartbotViewModel> datas;
        if ((id == Guid.Parse("3BEA47CB-1F38-4277-8EE5-C42D944D5441")))
        {
            datas = from request in DbContext.Requests
                    join flow in DbContext.Flows
                        on request.Id equals flow.RequestId
                    where request.StaffId == user.StaffId
                    select new RequestsInCartbotViewModel()
                    {
                        RequestNo = request.RequestNo,
                        RequestType = request.Workflow.RequestType.Title + " / نسخه " + request.Workflow.OrginalVersion + "." + request.Workflow.SecondaryVersion,
                        RequestDate = request.RegisterTime.Insert(2, ":"),
                        RequestTime = request.RegisterDate.ToString().Insert(4, "/").Insert(7, "/"),
                        CurrentStatus = request.RequestStatus.Title,
                        RequestId = request.Id,
                        FlowId = flow.Id,
                        RequestTypeId = request.Workflow.RequestTypeId,
                        WorkflowDetailId = flow.WorkFlowDetailId
                    };
        }
        else
        {
            datas = from request in DbContext.Requests
                    join flow in DbContext.Flows
                        on request.Id equals flow.RequestId
                    where request.RequestStatusId == id && request.StaffId == user.StaffId
                    select new RequestsInCartbotViewModel()
                    {
                        RequestNo = request.RequestNo,
                        RequestType = request.Workflow.RequestType.Title + " / نسخه " + request.Workflow.OrginalVersion + "." + request.Workflow.SecondaryVersion,
                        RequestDate = request.RegisterTime.Insert(2, ":"),
                        RequestTime = request.RegisterDate.ToString().Insert(4, "/").Insert(7, "/"),
                        CurrentStatus = request.RequestStatus.Title,
                        RequestId = request.Id,
                        FlowId = flow.Id,
                        RequestTypeId = request.Workflow.RequestTypeId,
                        WorkflowDetailId = flow.WorkFlowDetailId

                    };

        }
        return datas;
    }
    public DataSourceResult GetRequests(Guid userStaffId, DataSourceRequest request, int code)
    {
        var datas = from req in DbContext.Requests

                    where (code == 0 || req.RequestStatus.Code == code)
                          && req.StaffId == userStaffId
                    select new RequestsInCartbotViewModel()
                    {
                        RequestNo = req.RequestNo,
                        RequestType = req.Workflow.RequestType.Title + " / نسخه " + req.Workflow.OrginalVersion + "." + req.Workflow.SecondaryVersion,
                        RequestDate = req.RegisterDate.ToString().Insert(4, "/").Insert(7, "/"),
                        RequestTime = req.RegisterTime.Insert(2, ":"),
                        CurrentStatus = req.RequestStatus.Title,
                        RequestId = req.Id,
                        RequestTypeId = req.Workflow.RequestTypeId,
                        // WorkflowDetailId = flow.WorkFlowDetailId,

                    };
        // datas = datas.GroupBy(d => d.RequestNo).Select(g => g.FirstOrDefault()); // not support ef core 3.1
        return datas.ToDataSourceResult(request);
    }

    public object GetRequestsByUserId(Guid userStaffId)
    {
        var status = DbContext.LookUps.FirstOrDefault(i => i.Code == 1 && i.Type == "FlowStatus");
        var re = (from req in DbContext.Requests
                  where (req.RequestStatus.Code == 1 || req.RequestStatus.Code == 2) && req.StaffId == userStaffId
                  select new StaffRequestsViewModel()
                  {
                      RequestNo = req.RequestNo,
                      RequestType = req.Workflow.RequestType.Title + " / نسخه " + req.Workflow.OrginalVersion + "." + req.Workflow.SecondaryVersion,
                      RequestDate = req.RegisterDate.ToString().Insert(4, "/").Insert(7, "/"),
                      RequestTime = req.RegisterTime.Insert(2, ":"),
                      CurrentStatus = req.RequestStatus.Title,
                      RequestId = req.Id,
                      RequestTypeId = req.Workflow.RequestTypeId,
                      IgnoreOrgInfChange = req.IgnoreOrgInfChange ? "بله" : "خیر",
                      StaffDropDown = new StaffDropDownViewModel()
                      {
                          text = "انتخاب کاربر جایگزین",
                          value = ""
                      }
                  }).OrderByDescending(r => r.RequestDate).ThenByDescending(r => r.RequestTime);
        var result = new List<StaffRequestsViewModel>();
        foreach (var item in re)
        {
            var flowId = status != null
                ? (DbContext.Flows.Where(i => i.RequestId == item.RequestId && i.FlowStatusId == status.Id)
                    .OrderByDescending(i => i.DelayDate).ThenByDescending(i => i.DelayTime).Select(i => i.Id)
                    .FirstOrDefault())
                : Guid.Empty;

            result.Add(new StaffRequestsViewModel()
            {
                FlowId = flowId,
                RequestNo = item.RequestNo,
                RequestType = item.RequestType,
                RequestDate = item.RequestDate,
                RequestTime = item.RequestTime,
                CurrentStatus = item.CurrentStatus,
                RequestId = item.RequestId,
                RequestTypeId = item.RequestTypeId,
                IgnoreOrgInfChange = item.IgnoreOrgInfChange,
                StaffDropDown = item.StaffDropDown

            });

        }


        return result.OrderByDescending(r => r.RequestDate).ThenByDescending(r => r.RequestTime);
    }

    public void ChangeRequestsStaff(ChangeFlowStaffsViewModel vm)
    {


        Guid currentStaffId = Guid.Parse(vm.currentStaffId);

        List<string> usernames = new List<string>();
        usernames.Add(DbContext.Users.Single(c => c.StaffId == currentStaffId).UserName);

        foreach (var model in vm.model)
        {
            var request = DbContext.Requests.Find(model.Id);
            if (request == null || request.StaffId != currentStaffId) continue;
            {
                string username = DbContext.Users.Single(c => c.StaffId == model.StaffId).UserName;
                usernames.Add(username);

                request.StaffId = model.StaffId;
                request.IgnoreOrgInfChange = false;
                DbContext.Requests.Update(request);
            }
        }

        MainHub.RefreshCartbotGridOnChangingFlow(usernames);
    }

    public void SubmitChangeRequest(Guid currentStaffId, List<SubmitChangeRequestViewModel> model, string username)
    {
        List<string> usernames = new List<string>();
        List<string> staffs = new List<string>();

        foreach (var item in model)
        {
            var requestId = Guid.Parse(item.RequestId);

            if (item.IsRemoved)
            {
                // Remove record
                staffs.AddRange(DeleteRequests(requestId, username));
            }
            else if (!string.IsNullOrEmpty(item.StaffId))
            {
                // Change Staff
                usernames.Add(DbContext.Users.Single(c => c.StaffId == currentStaffId).UserName);


                var request = DbContext.Requests.Find(requestId);
                if (request == null || request.StaffId != currentStaffId)
                    continue;

                var staffId = Guid.Parse(item.StaffId);
                string theUsername = DbContext.Users.Single(c => c.StaffId == staffId).UserName;
                usernames.Add(theUsername);

                var staffOrgTitleId = DbContext.OrganiztionInfos.FirstOrDefault(c => c.Priority == true && c.StaffId == staffId).OrganiztionPostId;

                request.StaffId = staffId;
                request.OrganizationPostTitleId = staffOrgTitleId;
                request.IgnoreOrgInfChange = false;
                DbContext.Requests.Update(request);
            }
            else
            {
                // Update Ignore State
                ChangeStatus(requestId, item.IsIgnored);
            }
        }

        DbContext.SaveChanges();

        var notifiedStaffs = (from s in staffs select s).Distinct().ToList();
        var notifiedUserName = (from u in usernames select u).Distinct().ToList();
        MainHub.UpdateNotificationCount(notifiedStaffs);
        MainHub.UpdateDashboardCharts();
        MainHub.RefreshCartbotGridOnChangingFlow(notifiedUserName);
    }

    public IEnumerable<RequestsInCartbotViewModel> ExternalGetRequests(Guid id, Guid staffId)
    {
        if (id == Guid.Parse("3BEA47CB-1F38-4277-8EE5-C42D944D5441"))
        {
            var datas = from request in DbContext.Requests
                        join flow in DbContext.Flows
                            on request.Id equals flow.RequestId
                        where request.StaffId == staffId && flow.IsActive
                        select new RequestsInCartbotViewModel()
                        {
                            RequestNo = request.RequestNo,
                            RequestType = request.Workflow.RequestType.Title,
                            RequestDate = request.RegisterDate.ToString().Insert(4, "/").Insert(7, "/"),
                            RequestTime = request.RegisterTime.Insert(2, ":"),
                            CurrentStatus = request.RequestStatus.Title,
                            RequestId = request.Id,
                            FlowId = flow.Id,
                            RequestTypeId = request.Workflow.RequestTypeId,
                            WorkflowDetailId = flow.WorkFlowDetailId
                        }
                ;
            return datas;
        }
        else
        {

            var datas = from request in DbContext.Requests
                        join flow in DbContext.Flows
                            on request.Id equals flow.RequestId
                        where request.RequestStatusId == id && request.StaffId == staffId && flow.IsActive
                        select new RequestsInCartbotViewModel()
                        {
                            RequestNo = request.RequestNo,
                            RequestType = request.Workflow.RequestType.Title,
                            RequestDate = request.RegisterDate.ToString().Insert(4, "/").Insert(7, "/"),
                            RequestTime = request.RegisterTime.Insert(2, ":"),
                            CurrentStatus = request.RequestStatus.Title,
                            RequestId = request.Id,
                            FlowId = flow.Id,
                            RequestTypeId = request.Workflow.RequestTypeId,
                            WorkflowDetailId = flow.WorkFlowDetailId


                        };
            return datas;
        }
    }

    public List<Flow> GetNotificationBarData(string username)
    {
        var user = DbContext.Users.Single(c => c.UserName == username);
        //تعداد درخواست های دریافت شده اقدام نشده را محاسبه می کند 
        var query1 = from w in DbContext.WorkFlowConfermentAuthority
                     join wd in DbContext.WorkFlowConfermentAuthorityDetails on w.Id equals wd.ConfermentAuthorityId
                     join workflows in DbContext.Workflows on w.RequestTypeId equals workflows.RequestTypeId
                     join request in DbContext.Requests on workflows.Id equals request.WorkFlowId
                     join flow in DbContext.Flows on request.Id equals flow.RequestId
                     where wd.StaffId == user.StaffId && flow.StaffId == w.StaffId && (request.RegisterDate >= wd.FromDate && request.RegisterDate <= wd.ToDate)
                           && (!wd.OnlyOwnRequest || request.StaffId == user.StaffId) && flow.IsActive && flow.IsRead == false
                     select flow;

        var query2 = from f in DbContext.Flows
                     where f.StaffId == user.StaffId && f.IsActive && f.IsRead == false
                     select f;

        var flowsList = query1.Union(query2).ToList();
        return flowsList;
    }


    public Tuple<double, int> GetCountOfRequestsWithStatusCode1()
    {
        float reqCount = DbContext.Requests.Count(r => r.RequestStatus.Code == 1);
        float totalrequestCount = DbContext.Requests.Where(r => r.RequestStatus.Code != 3 && r.RequestStatus.Code != 4).ToList().Count();
        double percent = 0;
        if (reqCount > 0)
        {
            float result = reqCount / totalrequestCount * 100;
            percent = Math.Round(result, 2);
        }
        return new Tuple<double, int>(percent, (int)reqCount);
    }
    public Tuple<double, int> GetCountOfRequestsWithStatusCode2()
    {
        float reqCount = DbContext.Requests.Count(r => r.RequestStatus.Code == 2);
        float totalrequestCount = DbContext.Requests.Where(r => r.RequestStatus.Code != 3 && r.RequestStatus.Code != 4).ToList().Count();
        double percent = 0;
        if (reqCount > 0)
        {
            float result = reqCount / totalrequestCount * 100;
            percent = Math.Round(result, 2);
        }
        return new Tuple<double, int>(percent, (int)reqCount);
    }

    public IEnumerable<ThirdChartViewModel> GetSediment(int a, List<Holyday> holydays)
    {
        var requestsToShow = new List<RequestsWithDelayViewModel>();
        var cartbotsWithStatusCode1 = (from flows in DbContext.Flows
                                       join previousflows in DbContext.Flows on flows.PreviousFlowId equals previousflows.Id into leftjoin
                                       from previousflows in leftjoin.DefaultIfEmpty()

                                       join requests in DbContext.Requests on flows.RequestId equals requests.Id
                                       join workflowDetails in DbContext.WorkFlowDetails on flows.WorkFlowDetailId equals workflowDetails.Id
                                       where (a == 0) || flows.LookUpFlowStatus.Code == a
                                       select new { flows, workflowDetails, previousflows, requests }).ToList();


        var s2W = LookUpRepository.GetByTypeAndCode("WorkTime", 1);
        var thr = LookUpRepository.GetByTypeAndCode("WorkTime", 2);

        foreach (var item in cartbotsWithStatusCode1)
        {
            if (item.workflowDetails.WaitingTime == null) continue;
            var reqDate = DateTime.ParseExact((item.previousflows != null ? item.previousflows.ResponseDate + item.previousflows.ResponseTime : item.requests.RegisterDate + item.requests.RegisterTime), "yyyyMMddHHmm", null, DateTimeStyles.None);
            var diferentTime = FlowRepository.CalculateDelay(reqDate, DateTime.Now, s2W, thr, holydays);



            //  var diferentTime = DateTime.Now.Subtract(reqDate).TotalMinutes;
            var minute = item.workflowDetails.WaitingTime * 60;
            if (diferentTime > minute)
            {
                var delayTime = diferentTime - minute;
                requestsToShow.Add(new RequestsWithDelayViewModel()
                {
                    DelayTime = delayTime.Value,
                    RequestDetail = item.requests,
                    ConfirmPerson = item.flows.Staff.FName + " " + item.flows.Staff.LName,
                    WorkflowDetailTitle = item.workflowDetails.Title,
                    WorkflowDetailId = item.workflowDetails.Id
                });
            }

        }
        var model = requestsToShow.OrderByDescending(p => p.DelayTime).Select(s => new ThirdChartViewModel()
        {

            Title = s.RequestDetail.Workflow.RequestType.Title,
            ReqNo = s.RequestDetail.RequestNo,
            ReqTypeId = s.RequestDetail.Workflow.RequestTypeId,
            DelayTime = Math.Round(s.DelayTime),
            StaffFullName = s.ConfirmPerson,
            WorkflowDetailTitle = s.WorkflowDetailTitle,
            WorkflowDetailId = s.WorkflowDetailId


        });
        return model;
    }
    public IEnumerable<ThirdChartViewModel> GetThirdChartData()
    {
        var model = FlowRepository.CalculateSediment(1, DbContext.Holydays.ToList());

        return model;
    }

    public IEnumerable<ThirdChartViewModel> GetDataForSecondChart()
    {
        var requests = (from request in DbContext.Requests
                        where request.RequestStatus.Code == 1 || request.RequestStatus.Code == 2
                        group request by new { reqstatus = request.RequestStatus.Title, request.Workflow.RequestType.Title } into g

                        select new ThirdChartViewModel()
                        {
                            Count = g.Count(),
                            Title = g.Key.Title,
                            RequestStatus = g.Key.reqstatus
                        }).ToList();
        return requests;
    }

    public IEnumerable<DashboardColumnChartViewModel> FindRequestsWithDelay()
    {
        var q = (from requests in DbContext.Requests
                 join flows in DbContext.Flows on requests.Id equals flows.RequestId
                 group requests by new
                 {
                     flows.StaffId,
                     flows.Staff.FName,
                     flows.Staff.LName,
                     flows.Staff.PersonalCode,
                     flows.LookUpFlowStatus.Code
                 }
            into g
                 where g.Key.Code == 1

                 select new
                 {
                     Count = g.Count(),
                     FName = g.Key.FName,
                     LName = g.Key.LName,
                     PersonelCode = g.Key.PersonalCode

                 }).ToList();

        var result = q.OrderByDescending(p => p.Count).Take(10).Select(s => new DashboardColumnChartViewModel()
        {
            FName = s.FName,
            LName = s.LName,
            Count = s.Count,
            PersonelCode = s.PersonelCode,
            FullName = s.FName + " " + s.LName
        }).ToList();
        return result;
    }
    public IEnumerable<DashboardColumnChartViewModel> SupportPersonelActivitiesCount()
    {
        var supportPersonelCodes = new List<string>()
        {
            "395553", "511555", "511622" ,"511655", "455007", "511707"
        };

        var requestTypeId = Guid.Parse("52D2457D-22EA-4307-B3DF-27F05FB36367");


        var result = (from flow in DbContext.Flows
                      join req in DbContext.Requests on flow.RequestId equals req.Id
                      where req.Workflow.RequestTypeId == requestTypeId
                            && supportPersonelCodes.Contains(flow.Staff.PersonalCode) && flow.LookUpFlowStatus.Code != 1
                      select new
                      {
                          req.Id,
                          flow.Staff.FName,
                          flow.Staff.LName,
                          flow.Staff.PersonalCode
                      }).Distinct();

        var final = from en in result
                    group new { en.Id } by new { en.FName, en.LName, en.PersonalCode }
            into grp
                    orderby grp.Count() descending
                    select new DashboardColumnChartViewModel
                    {
                        Count = grp.Count(),
                        FullName = grp.Key.FName + " " + grp.Key.LName,
                        PersonelCode = grp.Key.PersonalCode,
                    };

        return final;

    }
    public IEnumerable<DashboardColumnChartViewModel> ITPersonelActivitiesCount()
    {
        var supportPersonelCodes = new List<string>()
        {
            "455007","511622","511655","395553","511555","395430","435045","511313","511515", "511707"
        };
        var requestTypeId = Guid.Parse("52D2457D-22EA-4307-B3DF-27F05FB36367");


        var result = (from flow in DbContext.Flows
                      join req in DbContext.Requests on flow.RequestId equals req.Id
                      where req.Workflow.RequestTypeId == requestTypeId
                            && supportPersonelCodes.Contains(flow.Staff.PersonalCode) && flow.LookUpFlowStatus.Code != 1
                      select new
                      {
                          req.Id,
                          flow.Staff.FName,
                          flow.Staff.LName,
                          flow.Staff.PersonalCode
                      }).Distinct();

        var final = from en in result
                    group new { en.Id } by new { en.FName, en.LName, en.PersonalCode }
            into grp
                    orderby grp.Count() descending
                    select new DashboardColumnChartViewModel
                    {
                        Count = grp.Count(),
                        FullName = grp.Key.FName + " " + grp.Key.LName,
                        PersonelCode = grp.Key.PersonalCode,
                    };

        return final;
    }

    public List<Flow> GetWorkFlowDiagramDatas(Guid requestId)
    {
        var flowsDetails = from flows in DbContext.Flows
                           join previousflows in DbContext.Flows on flows.PreviousFlowId equals previousflows.Id into leftjoin
                           from previousflows in leftjoin.DefaultIfEmpty()
                           join requests in DbContext.Requests on flows.RequestId equals requests.Id
                           join staffs in DbContext.Staffs on flows.StaffId equals staffs.Id
                           where requests.Id == requestId && flows.IsActive
                           select flows;

        return flowsDetails.OrderBy(s => s.Order).ToList();
    }

    public List<string> DeleteRequests(Guid requestid, string username)
    {
        var staffslist = new List<string>() { username };
        var requestToDelete = DbContext.Requests.FirstOrDefault(r => r.Id == requestid);
        //افرادی که باید درخواست حذف شده را از شمارش درخواست های دریافتی شان کم کنیم.
        var staffsToReduceRequests = (from flows in DbContext.Flows
                                      join staffs in DbContext.Staffs on flows.StaffId equals staffs.Id
                                      where flows.RequestId == requestid
                                      select staffs.Users.FirstOrDefault().UserName).ToList();
        foreach (var id in staffsToReduceRequests)
        {
            staffslist.AddRange((from w in DbContext.WorkFlowConfermentAuthority
                                 join wd in DbContext.WorkFlowConfermentAuthorityDetails on w.Id equals wd.ConfermentAuthorityId
                                 join workflows in DbContext.Workflows on w.RequestTypeId equals workflows.RequestTypeId
                                 join request in DbContext.Requests on workflows.Id equals request.WorkFlowId
                                 join flow in DbContext.Flows on request.Id equals flow.RequestId
                                 where w.Staff.PersonalCode == id && request.Id == requestid
                                 select wd.Staffs.Users.FirstOrDefault().UserName).ToList());
        }

        if (requestToDelete != null) DbContext.Requests.Remove(requestToDelete);
        SendMessageForDeleteRequest(requestid);
        return staffsToReduceRequests.Union(staffslist).ToList();
    }

    private void SendMessageForDeleteRequest(Guid requestId)
    {
        var result = (from flows in DbContext.Flows
                      join staffs in DbContext.Staffs on flows.StaffId equals staffs.Id
                      join requests in DbContext.Requests on flows.RequestId equals requests.Id
                      join requester in DbContext.Staffs on requests.StaffId equals requester.Id
                      join workFlows in DbContext.Workflows on requests.WorkFlowId equals workFlows.Id
                      join lookUps in DbContext.LookUps on workFlows.RequestTypeId equals lookUps.Id
                      join StatusRequest in DbContext.LookUps on flows.FlowStatusId equals StatusRequest.Id
                      where flows.RequestId == requestId
                      where StatusRequest.Code == 1
                      select new
                      {
                          staffs.PhoneNumber,
                          requests.RequestNo,
                          requester.FName,
                          requester.LName,
                          lookUps.Title
                      }).ToList();
        if (result.Count() > 0)
        {
            var fullName = result[0].FName + " " + result[0].LName;
            List<string> phoneNumber = new List<string>();
            foreach (var p in result)
            {
                phoneNumber.Add(p.PhoneNumber);
            }
            var text = @"همکار گرامی، درخواست با عنوان """ + result[0].Title +
                       @""" به شماره """ + result[0].RequestNo + @""" از طرف """ + fullName +
                       @""" حذف شد." + Environment.NewLine +
                       @"سامانه مدیریت فرآیند های کسب و کار طرفه نگار";
            _sendingSmsService.SendSms(phoneNumber, text);
        }
    }

    public void ChangeStatus(Guid requestId, bool isIgnored)
    {
        var request = DbContext.Requests.FirstOrDefault(r => r.Id == requestId);
        request.IgnoreOrgInfChange = isIgnored;
        DbContext.Requests.Update(request);
    }

    public List<string> DeleteListRequests(List<Guid> requestsId, string username)
    {
        if (requestsId.Count < 1)
        {
            throw new ArgumentException("هیچ درخواستی انتخاب نشده است!");
        }

        var staffslist = new List<string>() { username };
        var requestsToDelete = DbContext.Requests.Where(c => requestsId.Contains(c.Id)).ToList();

        var staffsToUpdateCartbot = (from flows in DbContext.Flows
                                     join staffs in DbContext.Staffs on flows.StaffId equals staffs.Id
                                     where requestsId.Contains(flows.RequestId)
                                     select staffs.Users.FirstOrDefault().UserName).ToList();


        foreach (var id in staffsToUpdateCartbot)
        {
            staffslist.AddRange(
                (from w in DbContext.WorkFlowConfermentAuthority
                 join wd in DbContext.WorkFlowConfermentAuthorityDetails on w.Id equals wd.ConfermentAuthorityId
                 join workflows in DbContext.Workflows on w.RequestTypeId equals workflows.RequestTypeId
                 join request in DbContext.Requests on workflows.Id equals request.WorkFlowId
                 join flow in DbContext.Flows on request.Id equals flow.RequestId
                 where w.Staff.PersonalCode == id && requestsId.Contains(request.Id)
                 select wd.Staffs.Users.FirstOrDefault().UserName)
                .ToList());
        }

        DbContext.Requests.RemoveRange(requestsToDelete);

        return staffsToUpdateCartbot.Union(staffslist).ToList();
    }




    public void CreateEmployementCertificate(Guid requestId, RequestEmployementCertificationViewModel model)
    {
        var EmpCertificate = new EmployementCertificate()
        {
            Id = Guid.NewGuid(),
            Dsr = model.Dsr,
            RequestId = requestId,
            RequestIntention = model.RequestIntention
        };
        DbContext.EmployementCertificate.Add(EmpCertificate);
    }

    public void UpdateEmployementCertificate(Guid currentFlowId, string dsr)
    {
        var currentFlow = DbContext.Flows.Where(f => f.Id == currentFlowId).FirstOrDefault();
        currentFlow.Dsr = dsr;
        DbContext.Flows.Update(currentFlow);
    }
    public List<OnlineUsersViewModel> GetOnlineUsersList()
    {
        var onlineUsers = MainHub.OnlineUsersList.DistinctBy(s => s.Value);
        var onlineUsersList = new List<OnlineUsersViewModel>();
        foreach (var entry in onlineUsers)
        {
            var userViewModel = (from staffs in DbContext.Staffs
                                 join users in DbContext.Users on staffs.Id equals users.StaffId
                                 where users.UserName == entry.Value
                                 select new OnlineUsersViewModel()
                                 {
                                     UserName = staffs.PersonalCode,
                                     FullName = staffs.FName + " " + staffs.LName
                                 }).FirstOrDefault();
            onlineUsersList.Add(userViewModel);
        }

        return onlineUsersList;
    }

    public IEnumerable<RequestViewModel> GetRequestsByRequestType(Guid requestTypeId)
    {
        var query = (from request in DbContext.Requests
                     join staff in DbContext.Staffs on request.StaffId equals staff.Id
                     join organization in DbContext.OrganiztionInfos on request.OrganizationPostTitleId equals organization.OrganiztionPostId
                     join workflow in DbContext.Workflows on request.WorkFlowId equals workflow.Id
                     where workflow.RequestTypeId == requestTypeId && organization.IsActive
                     select new RequestViewModel()
                     {
                         Id = request.Id,
                         Staff = staff.FName + " " + staff.LName,
                         RegisterDate = request.RegisterDate,
                         RegisterTime = request.RegisterTime,
                         RequestNo = request.RequestNo,
                         RequestType = workflow.RequestType.Title,
                         Version = workflow.OrginalVersion + "." + workflow.SecondaryVersion,
                         RequestStatus = request.RequestStatus.Title,
                         OrganizationPostTitle = organization.OrganiztionPost.Title
                     }).DistinctBy(r => r.Id).ToList();
        var status = DbContext.LookUps.FirstOrDefault(i => i.Code == 1 && i.Type == "FlowStatus");
        var result = new List<RequestViewModel>();
        foreach (var item in query)
        {
            var flowId = status != null
                ? (DbContext.Flows.Where(i => i.RequestId == item.Id && i.FlowStatusId == status.Id)
                    .OrderByDescending(i => i.DelayDate).ThenByDescending(i => i.DelayTime).Select(i => i.Id)
                    .FirstOrDefault())
                : Guid.Empty;

            result.Add(new RequestViewModel()
            {
                FlowId = flowId.ToString(),
                Id = item.Id,
                Staff = item.Staff,
                RegisterDate = item.RegisterDate,
                RegisterTime = item.RegisterTime,
                RequestNo = item.RequestNo,
                RequestType = item.RequestType,
                Version = item.Version,
                RequestStatus = item.RequestStatus,
                OrganizationPostTitle = item.OrganizationPostTitle

            });

        }



        return result.DistinctBy(r => r.Id).ToList();
    }

    public Request GetRequestById(Guid requestId)
    {
        return DbContext.Requests.Find(requestId);
    }

    public Request GetRequestsByRequestNo(long requestNo)
    {
        return DbContext.Requests.Where(x => x.RequestNo == requestNo).FirstOrDefault();
    }

    public Request FindById(Guid Id)
    {
        return DbContext.Requests.Find(Id);
    }
}