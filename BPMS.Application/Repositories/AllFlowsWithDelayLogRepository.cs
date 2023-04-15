using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Common.ViewModels.Report;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS.Application.Repositories;

public class AllFlowsWithDelayLogRepository : Repository<AllFlowsWithDelayLog>, IAllFlowsWithDelayLogRepository
{
    private readonly IServiceProvider _serviceProvider;
    public AllFlowsWithDelayLogRepository(BpmsDbContext context, IServiceProvider serviceProvider) : base(context)
    {
        _serviceProvider = serviceProvider;
    }

    public ILookUpRepository LookUpRepository => _serviceProvider.GetRequiredService<ILookUpRepository>();
    public IChartRepository ChartRepository => _serviceProvider.GetRequiredService<IChartRepository>();
    public IFlowRepository FlowRepository => _serviceProvider.GetRequiredService<IFlowRepository>();


    public void UpdateTableLogSchedule()
    {
        var query = (from request in Context.Requests
            join flow in Context.Flows on request.Id equals flow.RequestId
            join lookUp in Context.LookUps on flow.FlowStatusId equals lookUp.Id
            where lookUp.Code == 1 && lookUp.Type == "FlowStatus" //اقدام نشده
            join delayLogs in Context.AllFlowsWithDelayLogs on flow.Id equals delayLogs.FlowId into p
            from log in p.DefaultIfEmpty()
            // where log.FlowStatus == "اقدام نشده" || log.FlowStatus == null
            select request).Distinct().ToList();

        //query = query.Take(5).ToList();

        var s2W = LookUpRepository.GetByTypeAndCode("WorkTime", 1);
        var thr = LookUpRepository.GetByTypeAndCode("WorkTime", 2);
        var holiday = Context.Holydays.ToList();

        var tempList = new List<AllFlowsWithDelayLog>();

        int counter = 0;
        foreach (var request in query)
        {
            counter++;
            var delayHourToThisStep = 0;
            var predictedTimeAllToThisStep = 0;
            var timeTodoAllToThisStep = 0;

            foreach (var flow in request.Flows.OrderBy(d => d.Order))
            {
                if (Context.AllFlowsWithDelayLogs.Any(c => c.FlowId == flow.Id))
                {
                    continue;
                }

                var workflowBaseId = GetParentWorkFlowId(flow.Request.WorkFlowId);
                var workflowBase = Context.Workflows.Find(workflowBaseId);

                var reqDate = DateTime.ParseExact(flow.DelayDate + flow.DelayTime, "yyyyMMddHHmm", null, DateTimeStyles.None);
                var datetime = flow.ResponseDate != null
                    ? DateTime.ParseExact(flow.ResponseDate + flow.ResponseTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                    : DateTime.Now;
                var diff = Math.Round(FlowRepository.CalculateDelay(reqDate, datetime, s2W, thr, holiday));

                var currentWorkFlowDetail = Context.WorkFlowDetails.Single(c => c.Id == flow.WorkFlowDetailId);

                var sediment = (diff > currentWorkFlowDetail.WaitingTime) ? (diff - currentWorkFlowDetail.WaitingTime) : 0;
                var deadline = FlowRepository.CalculateTimeToDo(reqDate, currentWorkFlowDetail.WaitingTime, s2W, thr, holiday);


                int? timeToDo;
                if (flow.ResponseDate != null)
                    timeToDo = Convert.ToInt32(diff);
                else
                    timeToDo = null;
                delayHourToThisStep += Convert.ToInt32(sediment);
                predictedTimeAllToThisStep += currentWorkFlowDetail.WaitingTime.Value;
                timeTodoAllToThisStep += timeToDo ?? 0;

                var registerDate = DateTime.ParseExact(flow.Request.RegisterDate + flow.Request.RegisterTime, "yyyyMMddHHmm", null, DateTimeStyles.None);

                var subName = request.Workflow.RequestType == null ? "" : request.Workflow.RequestType.Title;

                var flowStaff = Context.Staffs.Single(c => c.Id == flow.StaffId);
                string flowStaffName = $"{flowStaff.FName} {flowStaff.LName}";

                var requestStaff = Context.Staffs.Single(c => c.Id == request.StaffId);
                string requestStaffName = $"{requestStaff.FName} {requestStaff.LName}";

                var workflowBaseRequestType = Context.LookUps.SingleOrDefault(c => c.Id == workflowBase.RequestTypeId);

                int retry = 0;
                while (workflowBaseRequestType == null && retry < 3)
                {
                    workflowBaseRequestType = Context.LookUps.SingleOrDefault(c => c.Id == workflowBase.RequestTypeId);
                    retry++;
                }

                var log = new AllFlowsWithDelayLog
                {
                    FlowId = flow.Id,
                    FlowStaffId = flow.StaffId,
                    RequestStaffId = request.StaffId,
                    FlowStaff = flowStaffName,
                    FlowStatus = flow.LookUpFlowStatus.Title,
                    ProcessName = workflowBaseRequestType.Title + " / نسخه " + workflowBase?.OrginalVersion + "." + workflowBase?.SecondaryVersion,
                    RegisterDate = registerDate,
                    RequestNo = request.RequestNo,
                    RequestStatus = request.RequestStatus.Title,
                    RequestStaff = requestStaffName,
                    WorkflowDetailTitle = currentWorkFlowDetail.Title,
                    WorkflowId = request.WorkFlowId,
                    SubProcessName = subName,
                    Deadline = deadline ?? DateTime.MinValue,
                    PredictedTime = currentWorkFlowDetail.WaitingTime.Value < 0 ? 0 : currentWorkFlowDetail.WaitingTime.Value,
                    DelayHour = Convert.ToInt32(sediment) < 0 ? 0 : Convert.ToInt32(sediment),
                    TimeTodo = timeToDo < 0 ? 0 : timeToDo,
                    DelayHourToThisStep = delayHourToThisStep < 0 ? 0 : delayHourToThisStep,
                    PredictedTimeAllToThisStep = predictedTimeAllToThisStep < 0 ? 0 : predictedTimeAllToThisStep,
                    TimeTodoAllToThisStep = timeTodoAllToThisStep < 0 ? 0 : timeTodoAllToThisStep
                };

                if (!(tempList.Any(c => c.FlowId == log.FlowId)))
                {
                    tempList.Add(log);
                }
            }


            var deletQuery = from delayLogs in Context.AllFlowsWithDelayLogs
                join flowsleft in Context.Flows on delayLogs.FlowId equals flowsleft.Id into p
                from flows in p.DefaultIfEmpty()
                where flows.Id == null
                select delayLogs;

            Context.AllFlowsWithDelayLogs.RemoveRange(deletQuery);

        }

        Context.AllFlowsWithDelayLogs.AddRange(tempList);
        Context.SaveChanges();
    }

    public DataSourceResult GetAllLogs(DataSourceRequest request)
    {
        var result = Context.AllFlowsWithDelayLogs;
        return result.ToDataSourceResult(request);
    }
    private List<WorkFlowViewModel> GetWorkFlowList()
    {
        var allworkflows = from workflows in Context.Workflows
            join requesttypes in Context.LookUps on workflows.RequestTypeId equals
                requesttypes.Id
            where workflows.SubProcessId == null
            select new WorkFlowViewModel()
            {
                RequestType = workflows.RequestType.Title,
                Dsr = workflows.Dsr,
                Id = workflows.Id,
                IsActive = workflows.IsActive,
                StaffId = workflows.StaffId,
                RequestTypeId = workflows.RequestTypeId,
                Version = workflows.OrginalVersion + "." + workflows.SecondaryVersion,
                RegisterDateTime = workflows.RegisterDate != 0 ? workflows.RegisterDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflows.RegisterTime.Insert(2, ":") : "",
                ModifiedDateTime = workflows.ModifideDate != 0 && workflows.ModifideDate != null ? workflows.ModifideDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflows.ModifideTime.Insert(2, ":") : "",
                Staff = workflows.Staff.FName + " " + workflows.Staff.LName,
                Modifier = workflows.Modifier.FName + " " + workflows.Modifier.LName,
                FlowType = workflows.FlowType.Title,
                RequestGroupType = workflows.RequestGroupType.Title
            };
        return allworkflows.ToList();

    }

    private IEnumerable<Guid> GetWorkflowWithSubProcess(Workflow workflow)
    {
        var list = new List<Guid> { workflow.Id };
        var sub = Context.Workflows.Where(d => d.SubProcessId == workflow.Id);
        foreach (var workflow1 in sub)
        {
            list.AddRange(GetWorkflowWithSubProcess(workflow1));
        }

        return list;
    }

    private IEnumerable<WorkflowIndicatorWidgetViewModel> GetWorkflowIndicatorByWorkFlowId(Guid id, List<Holyday> holydays)
    {
        var today = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));

        var query = (from workflowIndicators in Context.WorkFlowIndicators
            join flows in Context.Flows on workflowIndicators.RequestTypeId equals flows.Request.Workflow.RequestTypeId
            join previousflows in Context.Flows on flows.PreviousFlowId equals previousflows.Id into leftjoin
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

    public WorkflowIndicatorDetailViewModel GetDetailsOfWorkflow(Guid workflowId, List<Request> relatedRequests)
    {
        var holyday = Context.Holydays.ToList();

        var workflow = Context.Workflows.Find(workflowId);

        var requestTypeVersion = Context.Workflows
            .Where(d => d.RequestTypeId == workflow.RequestTypeId)
            .OrderByDescending(d => d.OrginalVersion)
            .ThenByDescending(d => d.SecondaryVersion)
            .Select(s => new SelectListItem()
            {
                Text = s.OrginalVersion + "." + s.SecondaryVersion,
                Value = s.Id.ToString()
            }).ToList();


        var query = relatedRequests
            .Select(d => new
            {
                d.RegisterDate,
                d.RegisterTime,
                RequestStatusCode = d.RequestStatus.Code
            }).ToList();

        var firstRequest = query.OrderBy(d => d.RegisterDate).ThenBy(d => int.Parse(d.RegisterTime))
            .FirstOrDefault();
        var lastRequest = query.OrderByDescending(d => d.RegisterDate).ThenByDescending(d => int.Parse(d.RegisterTime))
            .FirstOrDefault();
        var requestFinishedAccepted = query.Count(d => d.RequestStatusCode == 3);
        var totalRequest = query.Count;


        var gauge = GetWorkflowIndicatorByWorkFlowId(workflowId, holyday);

        var workflows = GetWorkflowWithSubProcess(workflow);

        var q1 = (from workflowdetail in Context.WorkFlowDetails
            join w in Context.Workflows on workflowdetail.WorkFlowId equals w.Id
            join flow in Context.Flows on workflowdetail.Id equals flow.WorkFlowDetailId
            join previousflows in Context.Flows on flow.PreviousFlowId equals previousflows.Id into leftjoin
            from previousflows in leftjoin.DefaultIfEmpty()
            join request in Context.Requests on flow.RequestId equals request.Id
            join lookup in Context.LookUps on request.RequestStatusId equals lookup.Id
            where workflows.Contains(w.Id)
            select new
            {
                workflowdetailId = workflowdetail.Id,
                workflowdetail.Title,
                previousflows,
                flow,
                request,
                lookup.Code,
                workflowdetail.WaitingTime
            }).ToList();



        var s2W = LookUpRepository.GetByTypeAndCode("WorkTime", 1); ;
        var thr = LookUpRepository.GetByTypeAndCode("WorkTime", 2); ;

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


        var model = new WorkflowIndicatorDetailViewModel()
        {
            FirstRequestDate = HelperBs.MakeDate(firstRequest?.RegisterDate.ToString()),
            FirstRequestTime = HelperBs.MakeTime(firstRequest?.RegisterTime),
            LastRequestDate = HelperBs.MakeDate(lastRequest?.RegisterDate.ToString()),
            LastRequestTime = HelperBs.MakeTime(lastRequest?.RegisterTime),
            FinishedAcceptedRequestsCount = requestFinishedAccepted,
            TotalRequestsCount = totalRequest,
            Gauges = gauge,
            TotalActivity = totalActivity.OrderByDescending(d => d.Count),
            Versions = requestTypeVersion,
            CurrentVersion = workflow?.Id.ToString()
        };
        return model;

    }

    public ReportWorkflowIndicatorDetailViewModel GetWorkflowDetailForReport(Guid workflowId, List<Request> relatedRequests)
    {
        var holyday = Context.Holydays.ToList();

        var workflow = Context.Workflows.Find(workflowId);

        var requestTypeVersion = Context.Workflows
            .Where(d => d.RequestTypeId == workflow.RequestTypeId)
            .OrderByDescending(d => d.OrginalVersion)
            .ThenByDescending(d => d.SecondaryVersion)
            .Select(s => new SelectListItem()
            {
                Text = s.OrginalVersion + "." + s.SecondaryVersion,
                Value = s.Id.ToString()
            }).ToList();


        var query = relatedRequests
            .Select(d => new
            {
                d.RegisterDate,
                d.RegisterTime,
                RequestStatusCode = d.RequestStatus.Code
            }).ToList();

        var firstRequest = query.OrderBy(d => d.RegisterDate).ThenBy(d => int.Parse(d.RegisterTime))
            .FirstOrDefault();
        var lastRequest = query.OrderByDescending(d => d.RegisterDate).ThenByDescending(d => int.Parse(d.RegisterTime))
            .FirstOrDefault();
        var requestFinishedAccepted = query.Count(d => d.RequestStatusCode == 3);
        var totalRequest = query.Count;


        var workflows = GetWorkflowWithSubProcess(workflow);

        var q1 = (from workflowdetail in Context.WorkFlowDetails
            join w in Context.Workflows on workflowdetail.WorkFlowId equals w.Id
            join flow in Context.Flows on workflowdetail.Id equals flow.WorkFlowDetailId
            join previousflows in Context.Flows on flow.PreviousFlowId equals previousflows.Id into leftjoin
            from previousflows in leftjoin.DefaultIfEmpty()
            join request in Context.Requests on flow.RequestId equals request.Id
            join lookup in Context.LookUps on request.RequestStatusId equals lookup.Id
            where workflows.Contains(w.Id) && request.RequestStatus.Code == 3
            select new
            {
                workflowdetailId = workflowdetail.Id,
                workflowdetail.Title,
                previousflows,
                flow,
                request,
                lookup.Code,
                workflowdetail.WaitingTime
            }).ToList();


        var s2W = LookUpRepository.GetByTypeAndCode("WorkTime", 1);
        var thr = LookUpRepository.GetByTypeAndCode("WorkTime", 2);


        var totalActivity = (from result in q1

                let reqDate = DateTime.ParseExact(result.flow.DelayDate + result.flow.DelayTime, "yyyyMMddHHmm",
                    null, DateTimeStyles.None)
                let datetime = result.flow.ResponseDate != null
                    ? DateTime.ParseExact(result.flow.ResponseDate + result.flow.ResponseTime, "yyyyMMddHHmm", null,
                        DateTimeStyles.None)
                    : DateTime.Now
                let diff = FlowRepository.CalculateDoneTime(reqDate, datetime, s2W, thr, holyday)

                let sediment = (diff > result.WaitingTime) ? diff - result.WaitingTime : 0
                select new
                {
                    result.workflowdetailId,
                    result.Title,
                    result.previousflows,
                    result.request,
                    result.request.Id,
                    result.WaitingTime,
                    ResponseDate = diff,
                    sediment

                }).GroupBy(i => i.request.Id)
            .Select(s => new ReportActivityStatusViewModel()
            {
                Avg = Math.Round(value: s.Sum(d => d.ResponseDate), digits: 0, mode: MidpointRounding.AwayFromZero),
                WathingTime = Convert.ToInt32(s.Sum(d => d.WaitingTime))

            });
        var model = new ReportWorkflowIndicatorDetailViewModel()
        {
            FirstRequestDate = HelperBs.MakeDate(firstRequest?.RegisterDate.ToString()),
            FirstRequestTime = HelperBs.MakeTime(firstRequest?.RegisterTime),
            LastRequestDate = HelperBs.MakeDate(lastRequest?.RegisterDate.ToString()),
            LastRequestTime = HelperBs.MakeTime(lastRequest?.RegisterTime),
            FinishedAcceptedRequestsCount = requestFinishedAccepted,
            TotalRequestsCount = totalRequest,
            TotalActivity = totalActivity.OrderByDescending(d => d.Count),
            Versions = requestTypeVersion,
            CurrentVersion = workflow?.Id.ToString()
        };
        return model;

    }

    public DataSourceResult GetGeneralAllProcessStatus(DataSourceRequest request)
    {
        return Context.AverageRequestProcessingTimeLogs.ToDataSourceResult(request);
    }

    public List<RequestUponWorkflowDto> GetRequestUponWorkFlows()
    {
        var queryRequests1 = (from workflow in Context.Workflows
            join requestType in Context.LookUps on workflow.RequestTypeId equals requestType.Id
            join req in Context.Requests on workflow.Id equals req.WorkFlowId
            where req.RequestStatus.Code == 3 //خاتمه یافته تاببد شده
            select new RequestWorkFlowDto
            {
                RequestTypeId = requestType.Id,
                Request = req,
                RequestId = req.Id,
                WorkFlowTitle = requestType.Title,
                WorkflowId = workflow.Id,
                Workflow = workflow
            }).ToList().DistinctBy(r => r.RequestId);

        var queryRequests = queryRequests1.GroupBy(i => i.WorkflowId).Select(reqWorkflowDto => new RequestUponWorkflowDto()
        {
            WorkflowId = reqWorkflowDto.Key,
            RequestWorkFlowDto = reqWorkflowDto.ToList()
        });

        return queryRequests.ToList();
    }

    public List<GeneralProcessStatusViewModel> GetProcessStatusSelectedStep(Guid workflowId)
    {

        var currentProcess = Context.Workflows.FirstOrDefault(w => w.Id == workflowId);

        var subIds = new List<Guid?>();

        var subprocessesIds = GetListofSubprocessIds(workflowId, subIds);
        var subprocesses = Context.Workflows.Where(i => subprocessesIds.Contains(i.Id)).Select(i => i);

        var requests = (from eachEequest in Context.Requests
            join flow in Context.Flows on eachEequest.Id equals flow.RequestId
            join process in subprocesses on eachEequest.WorkFlowId equals process.Id
            where flow.LookUpFlowStatus.Code == 3 || flow.LookUpFlowStatus.Code == 4
            select eachEequest).Distinct().ToList();


        var result = new List<AverageTimeViewModel>();
        requests.ForEach(o =>
        {
            var reuestLastFlow = o.Flows.OrderByDescending(i => i.Order).FirstOrDefault();

            var startDate = DateTime.ParseExact(o.RegisterDate + o.RegisterTime, "yyyyMMddHHmm", null, DateTimeStyles.None);

            var endtDate = (reuestLastFlow?.ResponseDate != null && reuestLastFlow?.ResponseTime != null) ? (DateTime.ParseExact(reuestLastFlow?.ResponseDate + reuestLastFlow?.ResponseTime, "yyyyMMddHHmm", null, DateTimeStyles.None)) : startDate;
            var diff = endtDate - startDate;

            var workflowBase = Context.Workflows.Find(o.WorkFlowId);

            result.Add(new AverageTimeViewModel()
            {
                ProcessName = o.Flows.Select(i => i.WorkFlowDetail.Title).FirstOrDefault(),
                TimeTodo = (int)diff.TotalDays,
                WorkflowId = o.WorkFlowId

            });

        });

        var query = from item in result
            group new { item.TimeTodo, item.PredictedTime } by new { item.ProcessName, item.WorkflowId }
            into g
            select new GeneralProcessStatusViewModel
            {
                ProcessName = g.Key.ProcessName,
                AvgTimeToDo = (int?)g.Average(d => d.TimeTodo),
                Min = g.Min(p => p.TimeTodo),
                Max = g.Max(m => m.TimeTodo),
                Count = (int)g.Count(),
                Id = g.Key.WorkflowId
            };
        return query.ToList();
    }
    public DataSourceResult GetGeneralProcessStatus(DataSourceRequest request,
        string from, string to)
    {
        var start = string.IsNullOrWhiteSpace(from) ? DateTime.MinValue : DateTime.ParseExact(@from, "yyyy/MM/dd", null, DateTimeStyles.None);
        var end = string.IsNullOrWhiteSpace(to) ? DateTime.MaxValue : DateTime.ParseExact(to, "yyyy/MM/dd", null, DateTimeStyles.None);


        var query = from log in Context.AllFlowsWithDelayLogs
            where log.RegisterDate >= start && log.RegisterDate <= end
            group new { log.TimeTodo, log.PredictedTime } by new { log.ProcessName, log.WorkflowId }
            into g
            select new GeneralProcessStatusViewModel
            {
                ProcessName = g.Key.ProcessName,
                AvgPredictedTime = (int?)g.Average(d => d.PredictedTime),
                Min = g.Min(p => p.TimeTodo),
                Max = g.Max(m => m.TimeTodo),
                Count = g.Count(),
                AvgTimeToDo = (int?)g.Average(d => d.TimeTodo)
            };
        return query.ToDataSourceResult(request);
    }

    public DataSourceResult GetLogsByChartId(DataSourceRequest request, Guid chartId)
    {

        var thisChartSubCharts = ChartRepository.GetSubCharts(chartId);
        var subUsersId = new List<Guid>();
        foreach (var chart in thisChartSubCharts)
        {
            var chartUsers = ChartRepository.GetChartStaffsId(chart.Id);
            subUsersId.AddRange(chartUsers);
        }

        return Context.AllFlowsWithDelayLogs
            .Where(d => subUsersId.Contains(d.RequestStaffId) || subUsersId.Contains(d.FlowStaffId))
            .ToDataSourceResult(request);
    }

    public DataSourceResult GetLogsBySubUsers(DataSourceRequest request, Guid staffId, bool onlyDelay = false)
    {

        var subUsersId = GetAdminSubStaffs(staffId);
        return Context.AllFlowsWithDelayLogs
            .Where(d => /*subUsersId.Contains(d.RequestStaffId) ||*/ subUsersId.Contains(d.FlowStaffId))
            .Where(d => onlyDelay != true || d.DelayHour > 0)
            .ToDataSourceResult(request);
    }

    private Guid GetParentWorkFlowId(Guid? id)
    {
        int retry = 0;
        var thisWorkFlow = Context.Workflows.SingleOrDefault(c => c.Id == id);

        while (thisWorkFlow == null && retry < 3)
        {
            thisWorkFlow = Context.Workflows.SingleOrDefault(c => c.Id == id);
            retry++;
        }

        var workflowId = thisWorkFlow.Id;
        if (thisWorkFlow.SubProcessId == null)
        {
            return workflowId;
        }
        var subprocessId = thisWorkFlow.SubProcessId;

        return GetParentWorkFlowId(subprocessId);
    }

    private List<Guid?> GetListofSubprocessIds(Guid? id, List<Guid?> outputs)
    {

        var thisWorkFlow = Context.Workflows.SingleOrDefault(c => c.Id == id);
        var workflowId = thisWorkFlow?.Id;
        if (thisWorkFlow?.SubProcessId == null)
        {
            outputs.Add(workflowId);
            return outputs;
        }
        var subprocessId = thisWorkFlow?.SubProcessId;
        outputs.Add(subprocessId);
        return GetListofSubprocessIds(subprocessId, outputs);
    }

    public List<Guid> GetAdminSubStaffs(Guid staffId)
    {
        var query = Context.OrganiztionInfos.FirstOrDefault(w => w.StaffId == staffId);
        var checkIf = Context.LookUps.FirstOrDefault(r => r.Id.ToString() == query.OrganiztionPost.Aux);
        if (checkIf.Aux2 != 1.ToString())
        {
            return new List<Guid>();
        }

        var thisChartSubCharts = ChartRepository.GetSubCharts(query.Chart.Id);
        var subUsersId = new List<Guid>();
        foreach (var chart in thisChartSubCharts)
        {
            var chartUsers = ChartRepository.GetChartStaffsId(chart.Id);
            subUsersId.AddRange(chartUsers);
        }

        return subUsersId;
    }

}