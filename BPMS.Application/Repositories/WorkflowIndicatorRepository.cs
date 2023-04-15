using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Constants.PermissionStructure;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using Workflow = BPMS.Domain.Entities.Workflow;

namespace BPMS.Application.Repositories;

public class WorkflowIndicatorRepository : Repository<WorkFlowIndicator>, IWorkflowIndicatorRepository
{
    private readonly IServiceProvider _serviceProvider;
    public WorkflowIndicatorRepository(BpmsDbContext context, IServiceProvider serviceProvider) : base(context)
    {
        _serviceProvider = serviceProvider;
    }

    public ILookUpRepository LookUpRepository => _serviceProvider.GetRequiredService<ILookUpRepository>();
    public IFlowRepository FlowRepository => _serviceProvider.GetRequiredService<IFlowRepository>();


    public BpmsDbContext DbContext => Context;

    public IEnumerable<WorkFlowIndicatorViewModel> GetWorkflowIndicatorRecords()
    {
        var query = from workflowindicator in DbContext.WorkFlowIndicators
                    let groupType = DbContext.LookUps.FirstOrDefault(d => d.Id.ToString() == workflowindicator.WidgetType.Aux).Title
                    select new WorkFlowIndicatorViewModel()
                    {
                        Id = workflowindicator.Id,
                        ActivityId = workflowindicator.ActivityId,
                        CalcCriterionId = workflowindicator.CalcCriterionId,
                        RequestTypeId = workflowindicator.RequestTypeId,
                        DurationId = workflowindicator.DurationId,
                        Warning = workflowindicator.Warning,
                        Crisis = workflowindicator.Crisis,
                        FlowstatusId = workflowindicator.FlowstatusId,
                        RequestType = workflowindicator.RequestType.Title,
                        Flowstatus = workflowindicator.Flowstatus.Title,
                        Activity = workflowindicator.WorkFlowDetail.Title,
                        Duration = workflowindicator.Duration.Title,
                        CalcCriterion = workflowindicator.CalcCriterion.Title,
                        WidgetTypeId = workflowindicator.WidgetTypeId,
                        WidgetType = workflowindicator.WidgetType.Title,
                        WidgetGroupTypeId = workflowindicator.WidgetType.Aux,
                        WidgetGroupType = groupType,
                        RegisterDate = workflowindicator.RegisterDate,
                        RegisterTime = workflowindicator.RegisterTime
                    };
        return query.OrderByDescending(a => a.RegisterDate).ThenByDescending(a => a.RegisterTime).ToList();
    }

    public dynamic GetActivitiesByRequestType(Guid id)
    {
        var query = from workflows in DbContext.Workflows
                    join workflowdetails in DbContext.WorkFlowDetails on workflows.Id equals workflowdetails.WorkFlowId
                    where workflows.RequestTypeId == id && workflows.IsActive && workflowdetails.Step != 0 && workflowdetails.Step != int.MaxValue && workflowdetails.Step != int.MinValue
                    orderby workflowdetails.Title ascending
                    select new { workflowdetails.Id, workflowdetails.Title };
        return query;
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
    public IEnumerable<LookUpViewModel> GetRequestTypeByPolicy(string username)
    {
        var user = DbContext.Users.Single(c => c.UserName == username);
        var userId = user.Id;
        var staffId = user.StaffId;

        var roleAccessesId = from ra in DbContext.RoleAccesses
                             where ra.UserId == userId
                             select ra.RoleId;

        var roleMapChartAccessIds = from organizationInfo in DbContext.OrganiztionInfos
                                    join chart in DbContext.Charts on organizationInfo.ChartId equals chart.Id
                                    join roleMapChart in DbContext.RoleMapCharts on chart.Id equals roleMapChart.ChartId
                                    where organizationInfo.StaffId == staffId
                                    select roleMapChart.RoleId;

        var roleMapPostTypeAccessId = GetRoleMapPostTypeAccessId(staffId);

        var roleMapPostTitleAccessId = GetRoleMapPostTitleAccessId(staffId);

        var roleIds = new List<Guid>();
        roleIds.AddRange(roleMapPostTitleAccessId);
        roleIds.AddRange(roleMapChartAccessIds);
        roleIds.AddRange(roleAccessesId);
        roleIds.AddRange(roleMapPostTypeAccessId);
        roleIds = roleIds.Distinct().ToList();

        var query = from workflow in DbContext.Workflows.ToList()
                    join requestType in DbContext.LookUps.ToList() on workflow.RequestTypeId equals requestType.Id
                    join roleClaim in DbContext.RoleClaims on requestType.Id.ToString() equals roleClaim.ClaimValue
                    join roleId in roleIds on roleClaim.RoleId equals roleId
                    where workflow.IsActive && roleClaim.ClaimType == PermissionPolicyType.WorkFlowIndexPermission
                    select new LookUpViewModel()
                    {
                        Id = requestType.Id,
                        Title = requestType.Title
                    };
        return query.ToList().DistinctBy(d => d.Id);
    }


    public IEnumerable<LookUpViewModel> GetRequestTypeByActivityId(Guid? staffId, string currentUserName)
    {
        var currentUser = DbContext.Users.Single(c => c.UserName == currentUserName);
        var userId = DbContext.Users.Single(d => d.StaffId == staffId).Id;

        var access = from user in DbContext.Users
                     join roleAccess in DbContext.RoleAccesses on user.Id equals roleAccess.UserId
                     join role in DbContext.Roles on roleAccess.RoleId equals role.Id
                     where role.Name == "sysadmin" && user.Id == userId
                     select role;
        if (access.Any())
        {
            return (from workflowdetails in DbContext.WorkFlowDetails
                    join workflow in DbContext.Workflows on workflowdetails.WorkFlowId equals workflow.Id
                    join requestType in DbContext.LookUps on workflow.RequestTypeId equals requestType.Id
                    where workflow.IsActive
                    select new LookUpViewModel()
                    {
                        Id = requestType.Id,
                        Title = requestType.Title
                    }).ToList().DistinctBy(d => d.Id);
        }


        var roleAccessesId = from ra in DbContext.RoleAccesses
                             where ra.UserId == userId
                             select ra.RoleId;

        var rolemapchartaccessId = from organiztionInfo in DbContext.OrganiztionInfos
                                   join chart in DbContext.Charts on organiztionInfo.ChartId equals chart.Id
                                   join rolemapchar in DbContext.RoleMapCharts on chart.Id equals rolemapchar.ChartId
                                   where organiztionInfo.StaffId == staffId
                                   select rolemapchar.RoleId;

        var roleMapPostTypeAccessId = GetRoleMapPostTypeAccessId(currentUser.StaffId);

        var roleMapPostTitleAccessId = GetRoleMapPostTitleAccessId(currentUser.StaffId);

        var roleIds = new List<Guid>();
        roleIds.AddRange(roleMapPostTitleAccessId);
        roleIds.AddRange(rolemapchartaccessId);
        roleIds.AddRange(roleAccessesId);
        roleIds.AddRange(roleMapPostTypeAccessId);
        roleIds = roleIds.Distinct().ToList();

        var query = from workflows in DbContext.Workflows.ToList()
                    join requesttypes in DbContext.LookUps.ToList() on workflows.RequestTypeId equals requesttypes.Id
                    join roleClaim in DbContext.RoleClaims on requesttypes.Id.ToString() equals roleClaim.ClaimValue
                    join roleId in roleIds on roleClaim.RoleId equals roleId
                    where workflows.IsActive && roleClaim.ClaimType == PermissionPolicyType.WidgetPermission
                    select new LookUpViewModel()
                    {

                        Id = requesttypes.Id,
                        Title = requesttypes.Title
                    };

        return query.ToList().DistinctBy(d => d.Id);
    }

    public IEnumerable<WorkflowIndicatorWidgetViewModel> GetWorkflowIndicatorRecordsForReport()
    {
        var query = (from workflowIndicators in DbContext.WorkFlowIndicators
                     join flows in DbContext.Flows on workflowIndicators.RequestTypeId equals flows.Request.Workflow.RequestTypeId
                     join previousflows in DbContext.Flows on flows.PreviousFlowId equals previousflows.Id into leftjoin
                     from previousflows in leftjoin.DefaultIfEmpty()
                     where workflowIndicators.ActivityId == flows.WorkFlowDetailId && flows.FlowStatusId == workflowIndicators.FlowstatusId
                     select new
                     {
                         workflowIndicators.Id,
                         responseDate = flows.ResponseDate != null ? flows.ResponseDate : (previousflows != null ? previousflows.ResponseDate : flows.Request.RegisterDate),
                         responseTime = flows.ResponseTime != null ? flows.ResponseTime : (previousflows != null ? previousflows.ResponseTime : flows.Request.RegisterTime),
                         workflowDetailTitle = workflowIndicators.WorkFlowDetail.Title,
                         requestType = flows.Request.Workflow.RequestType.Title,
                         warninglevel = workflowIndicators.Warning,
                         crisislevel = workflowIndicators.Crisis,
                         durationAux = workflowIndicators.Duration.Aux2,
                         workflowId = workflowIndicators.WorkFlowDetail.WorkFlowId,
                         widgetType = workflowIndicators.WidgetType.Title

                     }).ToList();

        var mymodel = from q in query
                          // let sub = (q.durationCode == 2) ? -30 : ((q.durationCode == 3) ? -90 : ((q.durationCode == 4) ? -180 : ((q.durationCode == 5) ? -365 : 0)))
                      let sub = int.Parse(q.durationAux) * -1
                      where q.responseDate >= Convert.ToInt32(DateTime.Now.AddDays(sub).ToString("yyyyMMdd"))
                      group q by new
                      {
                          sub,
                          q.Id,
                          q.requestType,
                          q.workflowDetailTitle,
                          q.crisislevel,
                          q.warninglevel,
                          q.widgetType,
                          q.workflowId
                      }
            into grp
                      where grp.Count() >= grp.Key.warninglevel
                      select new WorkflowIndicatorWidgetViewModel()
                      {
                          Count = grp.Count(),
                          Duration = grp.Key.sub,
                          Activity = grp.Key.workflowDetailTitle,
                          RequestTypeTitle = grp.Key.requestType,
                          WorkflowIndicatorId = grp.Key.Id,
                          FlowStatus = (grp.Count() > grp.Key.warninglevel && grp.Count() <= grp.Key.crisislevel) ? "هشدار" : "بحران",
                          WidgetType = grp.Key.widgetType,
                          WorkflowId = grp.Key.workflowId
                      };
        return mymodel;
    }

    public WorkFlowRequestsDetailByRequestTypeDto GetDetailsOfWorkFlowByRequestTypeId(Guid requestTypeId)
    {
        var workFlows = DbContext.Workflows.Where(c => c.RequestTypeId == requestTypeId).ToList();

        //تعداد کل درخواست های اجرا شده
        int allRequests = 0;

        //تاریخ وساعت اولین درخواست ثبت شده
        string firstRequestDateTime;

        // تاریخ و ساعت آخرین درخواست پایان یافته
        string lastRequestDateTime;

        // تعداد درخواست های اقدام نشده
        int totalNoActionRequests = 0;

        // تعداد درخواست های در حال اقدام
        int totalInActionRequests = 0;

        // تعداد درخواستهای خاتمه یافته رد شده
        int totalRejectedDoneRequests = 0;

        // تعداد درخواست های خاتمه یافته تایید شده
        int totalAcceptedDoneRequests = 0;

        var firstRequestDateTimes = new Dictionary<int, string>();
        var lastRequestDateTimes = new Dictionary<int, string>();

        foreach (var item in workFlows)
        {
            var query = DbContext.Requests.Where(r => r.WorkFlowId == item.Id)
                .Select(d => new
                {
                    d.RegisterDate,
                    d.RegisterTime,
                    RequestStatusCode = d.RequestStatus.Code
                }).ToList();

            //تاریخ وساعت اولین فرآیند ثبت شده
            var firstRequest = query.OrderBy(d => d.RegisterDate).ThenBy(d => int.Parse(d.RegisterTime))
                .FirstOrDefault();

            if (firstRequest != null)
            {
                firstRequestDateTimes.Add(firstRequest.RegisterDate, firstRequest.RegisterTime);
            }

            // تاریخ و ساعت آخرین فرآیند پایان یافته
            var lastRequest = query.OrderByDescending(d => d.RegisterDate).ThenByDescending(d => int.Parse(d.RegisterTime))
                .FirstOrDefault();
            if (lastRequest != null)
            {
                lastRequestDateTimes.Add(lastRequest.RegisterDate, lastRequest.RegisterTime);
            }

            //تعداد فرآیندهای اقدام نشده
            var requestNotStarted = query.Count(d => d.RequestStatusCode == 1);
            totalNoActionRequests += requestNotStarted;

            // تعداد فرآیندهای در حال اقدام
            var requestInProgress = query.Count(d => d.RequestStatusCode == 2);
            totalInActionRequests += requestInProgress;

            // تعداد فرآیندهای خاتمه یافته تایید شده
            var requestFinishedAccepted = query.Count(d => d.RequestStatusCode == 3);
            totalAcceptedDoneRequests += requestFinishedAccepted;

            // تعداد فرآیندهای خاتمه یافته رد شده
            var requestFinishedRejected = query.Count(d => d.RequestStatusCode == 4);
            totalRejectedDoneRequests += requestFinishedRejected;

            // تعداد کل فرآیند های اجرا شده
            var totalRequest = query.Count;
            allRequests += totalRequest;
        }

        var theFirstRequest = firstRequestDateTimes.OrderBy(c => c.Key).FirstOrDefault();
        firstRequestDateTime = HelperBs.MakeDate(theFirstRequest.Key.ToString()) + " " + HelperBs.MakeTime(theFirstRequest.Value);

        var theLastRequest = lastRequestDateTimes.OrderByDescending(c => c.Key).FirstOrDefault();
        lastRequestDateTime = HelperBs.MakeDate(theLastRequest.Key.ToString()) + " " + HelperBs.MakeTime(theLastRequest.Value);

        return new WorkFlowRequestsDetailByRequestTypeDto
        {
            FinishedAcceptedRequestsCount = totalAcceptedDoneRequests,
            FinishedRejectedRequestsCount = totalRejectedDoneRequests,
            FirstRequestDateTime = firstRequestDateTime,
            LastRequestDateTime = lastRequestDateTime,
            InProgressRequestsCount = totalInActionRequests,
            NotStartedRequestsCount = totalNoActionRequests,
            TotalRequestsCount = allRequests
        };
    }

    public WorkflowIndicatorDetailViewModel GetDetailsOfWorkflow(Guid workflowId)
    {
        var holyday = DbContext.Holydays.ToList();
        // _dbContext.Database.Log = log => System.IO.File.AppendAllText(@"C:\Users\s.razavi\Desktop\logbpms\logChart.txt", log);

        // ورژن
        var workflow = DbContext.Workflows.Find(workflowId);
        var requestTypeVersion = DbContext.Workflows
            .Where(d => d.RequestTypeId == workflow.RequestTypeId)
            .OrderByDescending(d => d.OrginalVersion)
            .ThenByDescending(d => d.SecondaryVersion)
            .Select(s => new SelectListItem()
            {
                Text = s.OrginalVersion + "." + s.SecondaryVersion,
                Value = s.Id.ToString()
            }).ToList();


        var query = DbContext.Requests.Where(r => r.WorkFlowId == workflowId)
            .Select(d => new
            {
                d.RegisterDate,
                d.RegisterTime,
                RequestStatusCode = d.RequestStatus.Code
            }).ToList();
        //تاریخ وساعت اولین فرآیند ثبت شده
        var firstRequest = query.OrderBy(d => d.RegisterDate).ThenBy(d => int.Parse(d.RegisterTime))
            .FirstOrDefault();
        // تاریخ و ساعت آخرین فرآیند پایان یافته
        var lastRequest = query.OrderByDescending(d => d.RegisterDate).ThenByDescending(d => int.Parse(d.RegisterTime))
            .FirstOrDefault();
        //تعداد فرآیندهای اقدام نشده
        var requestNotStarted = query.Count(d => d.RequestStatusCode == 1);
        // تعداد فرآیندهای در حال اقدام
        var requestInProgress = query.Count(d => d.RequestStatusCode == 2);
        // تعداد فرآیندهای خاتمه یافته تایید شده
        var requestFinishedAccepted = query.Count(d => d.RequestStatusCode == 3);
        // تعداد فرآیندهای خاتمه یافته رد شده
        var requestFinishedRejected = query.Count(d => d.RequestStatusCode == 4);
        // تعداد کل فرآیند های اجرا شده
        var totalRequest = query.Count;

        // شاخص ها
        //var gauge = GetWorkflowIndicatorByWorkFlowId(workflowId, holyday);

        var workflows = GetWorkflowWithSubProcess(workflow);

        var q1 = (from workflowdetail in DbContext.WorkFlowDetails
                  join w in DbContext.Workflows on workflowdetail.WorkFlowId equals w.Id
                  join flow in DbContext.Flows on workflowdetail.Id equals flow.WorkFlowDetailId
                  join previousflows in DbContext.Flows on flow.PreviousFlowId equals previousflows.Id into leftjoin
                  from previousflows in leftjoin.DefaultIfEmpty()
                  join request in DbContext.Requests on flow.RequestId equals request.Id
                  join lookup in DbContext.LookUps on request.RequestStatusId equals lookup.Id
                  where workflows.Contains(w.Id)
                  select new
                  {
                      workflowdetailId = workflowdetail.Id,
                      workflowdetail.Title,
                      previousflows,
                      flow,
                      flowId = flow.Id,
                      request,
                      lookup.Code,
                      workflowdetail.WaitingTime
                  }).ToList();


        // تایم کاری
        var s2W = LookUpRepository.GetByTypeAndCode("WorkTime", 1);
        var thr = LookUpRepository.GetByTypeAndCode("WorkTime", 2);

        // وضعیت کل فعالیت ها
        var totalActivity = (from result in q1
                             let reqDate = DateTime.ParseExact(result.flow.DelayDate + result.flow.DelayTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                             let datetime = result.flow.ResponseDate != null ? DateTime.ParseExact(result.flow.ResponseDate + result.flow.ResponseTime, "yyyyMMddHHmm", null, DateTimeStyles.None) : DateTime.Now
                             let diff = Math.Round(FlowRepository.CalculateDelay(reqDate, datetime, s2W, thr, holyday))
                             let sediment = (diff > result.WaitingTime) ? diff - result.WaitingTime : 0
                             select new
                             {
                                 result.workflowdetailId,
                                 result.Title,
                                 result.previousflows,
                                 result.request,
                                 result.WaitingTime,
                                 ResponseDate = diff,
                                 sediment,
                                 result.flowId

                             })
            .GroupBy(d => d.workflowdetailId)
            .Select(s => new ActivityStatusViewModel()
            {
                WorkflowDetailId = s.FirstOrDefault().workflowdetailId,
                FlowId = s.FirstOrDefault().flowId,
                Title = s.FirstOrDefault().Title,
                Count = s.Count(),
                Max = s.Max(d => (int)d.ResponseDate) < 0 ? 0 : s.Max(d => (int)d.ResponseDate),
                Min = s.Min(d => (int)d.ResponseDate) < 0 ? 0 : s.Min(d => (int)d.ResponseDate),
                Avg = (int)s.Average(d => d.ResponseDate) < 0 ? 0 : (int)s.Average(d => d.ResponseDate),
                MaxSediment = (int)s.Max(d => d.sediment),
                MinSediment = (int)s.Min(d => d.sediment),
                AvgSediment = (int)s.Average(d => d.sediment),
                WathingTime = s.FirstOrDefault().WaitingTime

            });

        // وضعیت فعالیت های اقدام نشده و در حال اقدام
        var inprogress = (from result in q1
                          where result.Code == 1 || result.Code == 2
                          // محاسبه زمان انجام
                          //   let reqDate = DateTime.ParseExact(result.previousflows != null ? result.previousflows.ResponseDate + result.previousflows.ResponseTime : result.request.RegisterDate + result.request.RegisterTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                          let reqDate = DateTime.ParseExact(result.flow.DelayDate + result.flow.DelayTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                          let datetime = result.flow.ResponseDate != null ? DateTime.ParseExact(result.flow.ResponseDate + result.flow.ResponseTime, "yyyyMMddHHmm", null, DateTimeStyles.None) : DateTime.Now
                          //let diff = datetime.Subtract(reqDate).TotalHours
                          let diff = Math.Round(FlowRepository.CalculateDelay(reqDate, datetime, s2W, thr, holyday))
                          // محاسبه رسوب
                          let sediment = (diff > result.WaitingTime) ? diff - result.WaitingTime : 0
                          select new
                          {
                              result.workflowdetailId,
                              result.Title,
                              result.previousflows,
                              result.request,
                              result.WaitingTime,
                              ResponseDate = diff,
                              sediment
                          })
            .GroupBy(d => d.workflowdetailId)
            .Select(s => new ActivityStatusViewModel()
            {
                WorkflowDetailId = s.FirstOrDefault().workflowdetailId,
                Title = s.FirstOrDefault().Title,
                Count = s.Count(),
                Max = s.Max(d => (int)d.ResponseDate) < 0 ? 0 : s.Max(d => (int)d.ResponseDate),
                Min = s.Min(d => (int)d.ResponseDate) < 0 ? 0 : s.Min(d => (int)d.ResponseDate),
                Avg = (int)s.Average(d => d.ResponseDate) < 0 ? 0 : (int)s.Average(d => d.ResponseDate),
                MaxSediment = (int)s.Max(d => d.sediment),
                MinSediment = (int)s.Min(d => d.sediment),
                AvgSediment = (int)s.Average(d => d.sediment),
                WathingTime = s.FirstOrDefault().WaitingTime
            });

        // وضعیت فعالیت های خاتمه یافته تایید شده
        var finishedAccepted = (from result in q1
                                where result.Code == 3
                                //  let reqDate = DateTime.ParseExact(result.previousflows != null ? result.previousflows.ResponseDate + result.previousflows.ResponseTime : result.request.RegisterDate + result.request.RegisterTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                                let reqDate = DateTime.ParseExact(result.flow.DelayDate + result.flow.DelayTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                                let datetime = result.flow.ResponseDate == null ? throw new ArgumentException("تاریخ پاسخ شماره درخواست " + result.request.Id + " ثبت نشده است.") : DateTime.ParseExact(result.flow.ResponseDate + result.flow.ResponseTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                                // let diff = datetime.Subtract(reqDate).TotalHours
                                let diff = Math.Round(FlowRepository.CalculateDelay(reqDate, datetime, s2W, thr, holyday))
                                let sediment = (diff > result.WaitingTime) ? diff - result.WaitingTime : 0

                                select new
                                {
                                    result.workflowdetailId,
                                    result.Title,
                                    result.previousflows,
                                    result.request,
                                    ResponseDate = diff,
                                    sediment,
                                    result.WaitingTime
                                })
            .GroupBy(d => d.workflowdetailId)
            .Select(s => new ActivityStatusViewModel()
            {
                WorkflowDetailId = s.FirstOrDefault().workflowdetailId,
                Title = s.FirstOrDefault().Title,
                Count = s.Count(),
                Max = s.Max(d => (int)d.ResponseDate) < 0 ? 0 : s.Max(d => (int)d.ResponseDate),
                Min = s.Min(d => (int)d.ResponseDate) < 0 ? 0 : s.Min(d => (int)d.ResponseDate),
                Avg = (int)s.Average(d => d.ResponseDate) < 0 ? 0 : (int)s.Average(d => d.ResponseDate),
                MaxSediment = (int)s.Max(d => d.sediment),
                MinSediment = (int)s.Min(d => d.sediment),
                AvgSediment = (int)s.Average(d => d.sediment),
                WathingTime = s.FirstOrDefault().WaitingTime
            });

        // وضعیت فعالیت های خاتمه یافته رد شده
        var finishedReject = (from result in q1
                              where result.Code == 4
                              //  let reqDate = DateTime.ParseExact(result.previousflows != null ? result.previousflows.ResponseDate + result.previousflows.ResponseTime : result.request.RegisterDate + result.request.RegisterTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                              let reqDate = DateTime.ParseExact(result.flow.DelayDate + result.flow.DelayTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                              let datetime = result.flow.ResponseDate == null ? throw new ArgumentException("تاریخ پاسخ شماره درخواست " + result.request.Id + " ثبت نشده است.") : DateTime.ParseExact(result.flow.ResponseDate + result.flow.ResponseTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                              // let diff = datetime.Subtract(reqDate).TotalHours
                              let diff = Math.Round(FlowRepository.CalculateDelay(reqDate, datetime, s2W, thr, holyday))
                              let sediment = (diff > result.WaitingTime) ? diff - result.WaitingTime : 0
                              select new
                              {
                                  result.workflowdetailId,
                                  result.Title,
                                  result.previousflows,
                                  result.request,
                                  ResponseDate = diff,
                                  sediment,
                                  result.WaitingTime
                              })
            .GroupBy(d => d.workflowdetailId)
            .Select(s => new ActivityStatusViewModel()
            {
                WorkflowDetailId = s.FirstOrDefault().workflowdetailId,
                Title = s.FirstOrDefault().Title,
                Count = s.Count(),
                Max = s.Max(d => (int)d.ResponseDate) < 0 ? 0 : s.Max(d => (int)d.ResponseDate),
                Min = s.Min(d => (int)d.ResponseDate) < 0 ? 0 : s.Min(d => (int)d.ResponseDate),
                Avg = (int)s.Average(d => d.ResponseDate) < 0 ? 0 : (int)s.Average(d => d.ResponseDate),
                MaxSediment = (int)s.Max(d => d.sediment),
                MinSediment = (int)s.Min(d => d.sediment),
                AvgSediment = (int)s.Average(d => d.sediment),
                WathingTime = s.FirstOrDefault().WaitingTime
            });

        var model = new WorkflowIndicatorDetailViewModel()
        {
            FirstRequestDate = HelperBs.MakeDate(firstRequest?.RegisterDate.ToString()),
            FirstRequestTime = HelperBs.MakeTime(firstRequest?.RegisterTime),
            LastRequestDate = HelperBs.MakeDate(lastRequest?.RegisterDate.ToString()),
            LastRequestTime = HelperBs.MakeTime(lastRequest?.RegisterTime),
            InProgressRequestsCount = requestInProgress,
            NotStartedRequestsCount = requestNotStarted,
            FinishedAcceptedRequestsCount = requestFinishedAccepted,
            FinishedRejectedRequestsCount = requestFinishedRejected,
            TotalRequestsCount = totalRequest,
            //Gauges = gauge,
            InprogressActivity = inprogress.OrderByDescending(d => d.Count),
            FinishdAcceptActivity = finishedAccepted.OrderByDescending(d => d.Count),
            FinishedRejectsActivity = finishedReject.OrderByDescending(d => d.Count),
            TotalActivity = totalActivity.OrderByDescending(d => d.Count),
            Versions = requestTypeVersion,
            CurrentVersion = workflow.Id.ToString()
        };
        return model;

    }

    private IEnumerable<Guid> GetWorkflowWithSubProcess(Workflow workflow)
    {
        var list = new List<Guid> { workflow.Id };
        var sub = DbContext.Workflows.Where(d => d.SubProcessId == workflow.Id);
        foreach (var workflow1 in sub)
        {
            list.AddRange(GetWorkflowWithSubProcess(workflow1));
        }

        return list;
    }

    private IEnumerable<WorkflowIndicatorWidgetViewModel> GetWorkflowIndicatorByWorkFlowId(Guid id, List<Holyday> holydays)
    {
        var today = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));

        var query = (from workflowIndicators in DbContext.WorkFlowIndicators
                     join flows in DbContext.Flows on workflowIndicators.RequestTypeId equals flows.Request.Workflow.RequestTypeId
                     join previousflows in DbContext.Flows on flows.PreviousFlowId equals previousflows.Id into leftjoin
                     from previousflows in leftjoin.DefaultIfEmpty()
                     where workflowIndicators.ActivityId == flows.WorkFlowDetailId
                           && flows.FlowStatusId == workflowIndicators.FlowstatusId
                           // چک کردن اینکه آیدی از نوع گردش کار هست یا شاخص
                           && (workflowIndicators.WorkFlowDetail.WorkFlowId == id || workflowIndicators.WidgetTypeId == id)
                     select new
                     {
                         workflowIndicators.Id,
                         responseDate = flows.ResponseDate != null ? flows.ResponseDate : (previousflows != null ? previousflows.ResponseDate : flows.Request.RegisterDate),
                         responseTime = flows.ResponseTime != null ? flows.ResponseTime : (previousflows != null ? previousflows.ResponseTime : flows.Request.RegisterTime),
                         workflowDetailTitle = workflowIndicators.WorkFlowDetail.Title,
                         requestType = flows.Request.Workflow.RequestType.Title,
                         warninglevel = workflowIndicators.Warning,
                         crisislevel = workflowIndicators.Crisis,
                         durationAux = workflowIndicators.Duration.Aux2,
                         workflowId = workflowIndicators.WorkFlowDetail.WorkFlowId,
                         WidgetType = workflowIndicators.WidgetType.Title,
                         workflowRegisterdate = flows.Request.Workflow.RegisterDate,
                         CalcCriterionCode = workflowIndicators.CalcCriterion.Code,
                         requestTypeId = workflowIndicators.RequestTypeId,
                         workflowDetailId = workflowIndicators.WorkFlowDetail.Id
                     }).ToList();

        var mymodel = from q in query
                          // let sub = (q.durationCode == 2) ? -30 : ((q.durationCode == 3) ? -90 : ((q.durationCode == 4) ? -180 : ((q.durationCode == 5) ? -365 : 0)))
                      let sub = (q.durationAux == "0") ? q.workflowRegisterdate - today : int.Parse(q.durationAux) * -1
                      where q.responseDate >= Convert.ToInt32(DateTime.Now.AddDays(sub).ToString("yyyyMMdd"))

                      group q by new
                      {
                          sub,
                          q.Id,
                          q.requestType,
                          q.workflowDetailTitle,
                          q.crisislevel,
                          q.warninglevel,
                          q.WidgetType,
                          q.workflowId,
                          q.CalcCriterionCode,
                          q.requestTypeId,
                          q.workflowDetailId
                      }
            into grp
                      select new WorkflowIndicatorWidgetViewModel()
                      {
                          Count = grp.Count(),
                          Duration = grp.Key.sub,
                          Activity = grp.Key.workflowDetailTitle,
                          RequestTypeTitle = grp.Key.requestType,
                          WorkflowIndicatorId = grp.Key.Id,
                          FlowStatus = (grp.Count() > grp.Key.warninglevel && grp.Count() <= grp.Key.crisislevel) ? "هشدار" : "بحران",
                          Warning = grp.Key.warninglevel,
                          Crisis = grp.Key.crisislevel,
                          WidgetType = grp.Key.WidgetType,
                          WorkflowId = grp.Key.workflowId,
                          CalcCriterionCode = grp.Key.CalcCriterionCode,
                          RequestTypeId = grp.Key.requestTypeId,
                          WorkflowDetailId = grp.Key.workflowDetailId
                      };

        var takhir = mymodel.Where(d => d.CalcCriterionCode == 2);
        var tedadi = mymodel.Where(d => d.CalcCriterionCode == 1);

        if (!takhir.Any()) return mymodel;

        var ta = from t in takhir
                 join s in FlowRepository.CalculateSediment(1, holydays) on t.WorkflowDetailId equals s.WorkflowDetailId
                 where t.WorkflowDetailId == s.WorkflowDetailId
                 group t by new
                 {
                     t.WorkflowIndicatorId,
                     t.RequestTypeTitle,
                     t.Activity,
                     t.Crisis,
                     t.Warning,
                     t.WidgetType,
                     t.WorkflowId,
                     t.CalcCriterionCode,
                     t.RequestTypeId,
                     t.WorkflowDetailId,
                     t.Duration
                 }
            into grp
                 select new WorkflowIndicatorWidgetViewModel()
                 {
                     Count = grp.Count(),
                     Duration = grp.Key.Duration,
                     Activity = grp.Key.Activity,
                     RequestTypeTitle = grp.Key.RequestTypeTitle,
                     WorkflowIndicatorId = grp.Key.WorkflowIndicatorId,
                     FlowStatus = (grp.Count() > grp.Key.Warning && grp.Count() <= grp.Key.Crisis) ? "هشدار" : "بحران",
                     Warning = grp.Key.Warning,
                     Crisis = grp.Key.Crisis,
                     WidgetType = grp.Key.WidgetType,
                     WorkflowId = grp.Key.WorkflowId,
                     CalcCriterionCode = grp.Key.CalcCriterionCode,
                     RequestTypeId = grp.Key.RequestTypeId,
                     WorkflowDetailId = grp.Key.WorkflowDetailId
                 };

        return tedadi.Union(ta);
    }


    public WorkflowIndicatorWidgetViewModel GetByWidgetId(Guid id)
    {
        var workflowindicator = DbContext.WorkFlowIndicators.FirstOrDefault(d => d.WidgetTypeId == id);
        var today = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));

        var activeWorkFlow = DbContext.Workflows.Where(w => w.RequestTypeId == workflowindicator.RequestTypeId && w.IsActive).SingleOrDefault();

        var intervalDays = (workflowindicator.Duration.Aux2 == "0") ? activeWorkFlow.RegisterDate - today : int.Parse(workflowindicator.Duration.Aux2) * -1;
        var startDate = Convert.ToInt32(DateTime.Now.AddDays(intervalDays).ToString("yyyyMMdd"));

        var requestWithFlowStatus = from flow in DbContext.Flows
                                    join request in DbContext.Requests on flow.RequestId equals request.Id
                                    join workflow in DbContext.Workflows on request.WorkFlowId equals workflow.Id
                                    where flow.IsActive && flow.WorkFlowDetailId == workflowindicator.ActivityId
                                                        && (flow.LookUpFlowStatus.Id == workflowindicator.FlowstatusId)
                                                        && (flow.DelayDate >= startDate)
                                    select flow;


        if (workflowindicator.CalcCriterion.Code == 1)
        {

            return new WorkflowIndicatorWidgetViewModel()
            {
                Count = requestWithFlowStatus.Count(),
                WidgetType = workflowindicator.WidgetType.Title,
                WorkflowIndicatorId = workflowindicator.Id,
                Activity = workflowindicator.WorkFlowDetail.Title,
                RequestTypeTitle = workflowindicator.RequestType.Title,
                Warning = workflowindicator.Warning,
                Crisis = workflowindicator.Crisis
            };
        }

        else
        {
            var delayCount = 0;
            var s2W = LookUpRepository.GetByTypeAndCode("WorkTime", 1);
            var thr = LookUpRepository.GetByTypeAndCode("WorkTime", 2);
            var holyday = Context.Holydays.ToList();

            foreach (var flow in requestWithFlowStatus.ToList())
            {
                var reqDate = DateTime.ParseExact(flow.DelayDate + flow.DelayTime, "yyyyMMddHHmm", null, DateTimeStyles.None);
                var datetime = flow.ResponseDate != null
                    ? DateTime.ParseExact(flow.ResponseDate + flow.ResponseTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                    : DateTime.Now;
                var diff = Math.Round(FlowRepository.CalculateDelay(reqDate, datetime, s2W, thr, holyday));
                var sediment = (diff > flow.WorkFlowDetail.WaitingTime) ? (diff - flow.WorkFlowDetail.WaitingTime) : 0;
                if (sediment > 0)
                {
                    delayCount += 1;
                }
            }

            return new WorkflowIndicatorWidgetViewModel()
            {
                Count = delayCount,
                WidgetType = workflowindicator.WidgetType.Title,
                WorkflowIndicatorId = workflowindicator.Id,
                Activity = workflowindicator.WorkFlowDetail.Title,
                RequestTypeTitle = workflowindicator.RequestType.Title,
                Warning = workflowindicator.Warning,
                Crisis = workflowindicator.Crisis
            };

        }

    }

    public void CreateWorkflowIndicator(WorkFlowIndicatorViewModel model)
    {
        var workFlowIndicator = DbContext.WorkFlowIndicators.Find(model.Id);
        if (workFlowIndicator == null)
        {
            int maxcode;
            var code = DbContext.LookUps.Where(l => l.Type == "Widget").Select(c => c.Code);
            if (code.Count() != 0)
            {
                maxcode = code.Max() + 1;
            }
            else
            {
                maxcode = 1;
            }
            var look = new LookUp()
            {
                Aux = model.WidgetGroupTypeId.ToString(),
                Code = maxcode,
                Title = model.WidgetType.Trim(),
                IsActive = true,
                Type = "Widget",
                Aux2 = "Indicator"
            };
            DbContext.LookUps.Add(look);

            var wfi = new WorkFlowIndicator()
            {

                RequestTypeId = model.RequestTypeId,
                ActivityId = model.ActivityId,
                FlowstatusId = model.FlowstatusId,
                DurationId = model.DurationId,
                CalcCriterionId = model.CalcCriterionId,
                Crisis = model.Crisis,
                Warning = model.Warning,
                WidgetTypeId = look.Id,
                RegisterDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")),
                RegisterTime = DateTime.Now.ToString("HHmm")
            };
            DbContext.WorkFlowIndicators.Add(wfi);

        }
        else
        {
            var widget = DbContext.LookUps.Find(workFlowIndicator.WidgetTypeId);
            widget.Title = model.WidgetType.Trim();
            widget.Aux = model.WidgetGroupTypeId.ToString();
            var wfi = new WorkFlowIndicator()
            {
                Id = model.Id,
                RequestTypeId = model.RequestTypeId,
                ActivityId = model.ActivityId,
                FlowstatusId = model.FlowstatusId,
                DurationId = model.DurationId,
                CalcCriterionId = model.CalcCriterionId,
                Crisis = model.Crisis,
                Warning = model.Warning,
                WidgetTypeId = widget.Id,
                RegisterDate = workFlowIndicator.RegisterDate,
                RegisterTime = workFlowIndicator.RegisterTime
            };
            DbContext.WorkFlowIndicators.Update(wfi);
        }
    }

    public void DeleteIndicator(Guid id)
    {

        var workflowIndicatorToRemove = DbContext.WorkFlowIndicators.Find(id);
        if (workflowIndicatorToRemove == null)
        {
            throw new ArgumentException("رکورد مورد نظر یافت نشد.");
        }
        var widget = DbContext.LookUps.FirstOrDefault(l => l.Id == workflowIndicatorToRemove.WidgetTypeId);
        if (widget != null)
        {
            DbContext.LookUps.Remove(widget);
        }
        DbContext.WorkFlowIndicators.Remove(workflowIndicatorToRemove);

    }
}