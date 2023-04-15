using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS.Application.Repositories;

public class WorkflowDetailsRepository : Repository<WorkFlowDetail>, IWorkflowDetailsRepository
{
    private readonly IServiceProvider _serviceProvider;
    public WorkflowDetailsRepository(BpmsDbContext context, IServiceProvider serviceProvider) : base(context)
    {
        _serviceProvider = serviceProvider;
    }

    public ILookUpRepository LookUpRepository => _serviceProvider.GetRequiredService<ILookUpRepository>();
    public IFlowRepository FlowRepository => _serviceProvider.GetRequiredService<IFlowRepository>();

    public WorkFlowDetail GetWithRequestType(Guid requestTypeId, Guid id)
    {
        return Context.WorkFlowDetails.Include(w => w.WorkFlow).Single(p => p.Id == id && p.WorkFlow.RequestTypeId == requestTypeId);
    }
    public WorkFlowDetail GetWithRequestType(Guid requestTypeId, int step)
    {
        var workFlow = Context.Workflows.SingleOrDefault(c => c.RequestTypeId == requestTypeId && c.IsActive);
        var workFlowDetail = Context.WorkFlowDetails.FirstOrDefault(p => p.Step == step &&
                                                                         p.WorkFlowId == workFlow.Id);

        return workFlowDetail;
    }
    public WorkFlowDetail GetWithRequestId(Guid requestId, int step)
    {
        var d = from workflowDetail in Context.WorkFlowDetails
                join workflow in Context.Workflows on workflowDetail.WorkFlowId equals workflow.Id
                join request in Context.Requests on workflow.Id equals request.WorkFlowId
                where request.Id == requestId && workflowDetail.Step == step
                select workflowDetail;
        return d.FirstOrDefault();
    }
    public WorkFlowDetail GetWithWorkFlow(Guid workflowId, int step)
    {
        return Context.WorkFlowDetails.Include(p => p.WorkFlow).Single(p => p.Step == step && p.WorkFlowId == workflowId);
    }

    public WorkFlowViewModel ShowDiagram(Guid workflowDetailId, Guid requestId)
    {
        XNamespace workflow = "http://magic";
        var encoding = new UnicodeEncoding();
        var query = Context.WorkFlowDetails.Find(workflowDetailId);

        var str = encoding.GetString(query.WorkFlow.Content);
        XDocument xml = XDocument.Parse(str);
        var request = Context.Requests.Find(requestId);
        var waitingTime = Context.WorkFlowDetails.Where(w => w.Id == workflowDetailId).Select(o => o.WaitingTime)
            .FirstOrDefault();
        if (request == null)
        {
            var id = xml.Descendants()
                .Where(d => (string)d.Attribute(workflow + "WorkFlowDetailId") == query.Id.ToString()).ToList()
                .Select(d => d.Attribute("id")).Single();
            var elements = new List<string> { id.Value };
            var workFlow = new WorkFlowViewModel()
            {
                RequestType = query.WorkFlow.RequestType.Title,
                XmlFile = HelperBs.EncodeUri(str),
                DataElementId = elements,
                WaitingTime = waitingTime
            };
            return workFlow;
        }

        var status = request.RequestStatus.Code;
        if (status == 1 || status == 2)
        {
            var flows = Context.Flows.Where(f => f.RequestId == requestId && f.LookUpFlowStatus.Code == 1)
                .ToList();
            var elements = new List<string>();
            foreach (var flow in flows)
            {
                if (flow.IsActive)
                {

                    var id = xml.Descendants()
                        .Where(d => (string)d.Attribute(workflow + "WorkFlowDetailId") ==
                                    flow.WorkFlowDetailId.ToString()).ToList().Select(d => d.Attribute("id")).FirstOrDefault();
                    if (id != null)
                        elements.Add(id.Value);
                    else
                    {
                        id = xml.Descendants()
                            .Where(d => (string)d.Attribute(workflow + "WorkFlowDetailId") ==
                                        flow.CallActivityId.ToString()).ToList().Select(d => d.Attribute("id")).FirstOrDefault();
                    }
                    elements.Add(id?.Value);
                }
                else
                {
                    var flowEvent = Context.FlowEvents.OrderBy(d => d.Order)
                        .FirstOrDefault(f => f.FlowId == flow.Id && f.IsActive == false);
                    if (flowEvent != null)
                    {
                        elements.Add(flowEvent.WorkflowEsb.EventId);
                    }
                }
            }

            var workFlow = new WorkFlowViewModel()
            {
                RequestType = query.WorkFlow.RequestType.Title,
                XmlFile = HelperBs.EncodeUri(str),
                DataElementId = elements,
                WaitingTime = waitingTime
            };
            return workFlow;
        }
        else
        {
            var wfdid = Context.Flows.Where(f => f.RequestId == requestId).OrderByDescending(o => o.Order)
                .Select(s => s.WorkFlowDetailId).FirstOrDefault();
            var id = xml.Descendants()
                .Where(d => (string)d.Attribute(workflow + "WorkFlowDetailId") == wfdid.ToString()).ToList()
                .Select(d => d.Attribute("id")).SingleOrDefault();
            var elements = new List<string>();
            if (id != null)
                elements.Add(id.Value);
            var workFlow = new WorkFlowViewModel()
            {
                RequestType = query.WorkFlow.RequestType.Title,
                XmlFile = HelperBs.EncodeUri(str),
                DataElementId = elements
            };
            return workFlow;
        }
    }
    public IEnumerable<WorkFlowDetailViewModel> GetActiveWorkFlowDetailsByRequestType(Guid typeId)
    {
        if (typeId == Guid.Empty)
        {
            return Context.WorkFlowDetails.Where(l => l.WorkFlow.IsActive && l.Step != 0 && l.Step != int.MaxValue && l.Step != int.MinValue).Select(p => new WorkFlowDetailViewModel()
            {
                Title = p.Title,
                Id = p.Id,

            });
        }
        return Context.WorkFlowDetails.Where(l => l.WorkFlow.RequestTypeId == typeId && l.WorkFlow.IsActive && l.Step != 0 && l.Step != int.MaxValue && l.Step != int.MinValue).Select(p => new WorkFlowDetailViewModel()
        {
            Title = p.Title,
            Id = p.Id,

        });


    }

    public IEnumerable<WorkFlowDetailViewModel> GetWorkflowDetail(Guid workflowDetailId, int code)
    {
        // تایم کاری
        var s2W = LookUpRepository.GetByTypeAndCode("WorkTime", 1); ;
        var thr = LookUpRepository.GetByTypeAndCode("WorkTime", 2); ;
        var holyday = Context.Holydays.ToList();
        IEnumerable<WorkFlowDetailViewModel> result = null;
        var query = (from wfd in Context.WorkFlowDetails
                     join flow in Context.Flows on wfd.Id equals flow.WorkFlowDetailId
                     join previousFlow in Context.Flows on flow.PreviousFlowId equals previousFlow.Id into leftjoin
                     from previousFlow in leftjoin.DefaultIfEmpty()
                     join request in Context.Requests on flow.RequestId equals request.Id
                     where wfd.Id == workflowDetailId

                     select new
                     {
                         wfd,
                         previousFlow,
                         flow,
                         request

                     }).ToList();
        switch (code)
        {
            case 100:
                result = from p in query
                             // let reqDate = DateTime.ParseExact(p.previousFlow != null ? p.previousFlow.ResponseDate + p.previousFlow.ResponseTime : p.request.RegisterDate + p.request.RegisterTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                         let reqDate = DateTime.ParseExact(p.flow.DelayDate + p.flow.DelayTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                         let datetime = p.flow.ResponseDate != null ? DateTime.ParseExact(p.flow.ResponseDate + p.flow.ResponseTime, "yyyyMMddHHmm", null, DateTimeStyles.None) : DateTime.Now
                         // let diff = datetime.Subtract(reqDate).TotalHours
                         let diff = Math.Round(FlowRepository.CalculateDelay(reqDate, datetime, s2W, thr, holyday))
                         let sediment = (diff > p.wfd.WaitingTime) ? diff - p.wfd.WaitingTime : 0
                         select new WorkFlowDetailViewModel
                         {
                             Id = p.wfd.Id,
                             FlowId = p.flow.Id,
                             Title = p.wfd.Title,
                             FullName = p.flow.Staff.FName + " " + p.flow.Staff.LName,
                             ReceiveDate = p.previousFlow?.ResponseDate ?? p.request.RegisterDate,
                             ResponseDate = p.flow.ResponseDate,
                             ResponseTime = p.flow.ResponseTime,
                             Building = p.flow.Staff.BuildingId != null ? p.flow.Staff.Building.Title + "-" + p.flow.Staff.Building.Aux2 : null,
                             RequestNo = p.request.RequestNo,
                             FlowStatus = p.flow.LookUpFlowStatus.Title,
                             Phone = p.flow.Staff.LocalPhone,
                             Answer = (int)diff,
                             Sediment = (int)sediment,
                             WaitingTime = p.wfd.WaitingTime,
                             RequestId = p.request.Id
                         };
                break;
            case 12:
                result = from p in query
                         where p.request.RequestStatus.Code == 1 || p.request.RequestStatus.Code == 2
                         //let reqDate = DateTime.ParseExact(p.previousFlow != null ? p.previousFlow.ResponseDate + p.previousFlow.ResponseTime : p.request.RegisterDate + p.request.RegisterTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                         let reqDate = DateTime.ParseExact(p.flow.DelayDate + p.flow.DelayTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                         let datetime = p.flow.ResponseDate != null ? DateTime.ParseExact(p.flow.ResponseDate + p.flow.ResponseTime, "yyyyMMddHHmm", null, DateTimeStyles.None) : DateTime.Now
                         // let diff = datetime.Subtract(reqDate).TotalHours
                         let diff = Math.Round(FlowRepository.CalculateDelay(reqDate, datetime, s2W, thr, holyday))
                         let sediment = (diff > p.wfd.WaitingTime) ? diff - p.wfd.WaitingTime : 0
                         select new WorkFlowDetailViewModel
                         {
                             Id = p.wfd.Id,
                             FlowId = p.flow.Id,
                             Title = p.wfd.Title,
                             FullName = p.flow.Staff.FName + " " + p.flow.Staff.LName,
                             ReceiveDate = p.previousFlow?.ResponseDate ?? p.request.RegisterDate,
                             ResponseDate = p.flow.ResponseDate,
                             ResponseTime = p.flow.ResponseTime,
                             Building = p.flow.Staff.BuildingId != null ? p.flow.Staff.Building.Title + "-" + p.flow.Staff.Building.Aux2 : null,
                             RequestNo = p.request.RequestNo,
                             FlowStatus = p.flow.LookUpFlowStatus.Title,
                             Phone = p.flow.Staff.LocalPhone,
                             Answer = (int)diff,
                             Sediment = (int)sediment,
                             WaitingTime = p.wfd.WaitingTime,
                             RequestId = p.request.Id
                         };
                break;
            case 3:
                result = from p in query
                         where p.request.RequestStatus.Code == 3
                         //let reqDate = DateTime.ParseExact(p.previousFlow != null ? p.previousFlow.ResponseDate + p.previousFlow.ResponseTime : p.request.RegisterDate + p.request.RegisterTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                         let reqDate = DateTime.ParseExact(p.flow.DelayDate + p.flow.DelayTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                         let datetime = DateTime.ParseExact(p.flow.ResponseDate + p.flow.ResponseTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                         // let diff = datetime.Subtract(reqDate).TotalHours
                         let diff = Math.Round(FlowRepository.CalculateDelay(reqDate, datetime, s2W, thr, holyday))
                         let sediment = (diff > p.wfd.WaitingTime) ? diff - p.wfd.WaitingTime : 0

                         select new WorkFlowDetailViewModel
                         {
                             Id = p.wfd.Id,
                             FlowId = p.flow.Id,
                             Title = p.wfd.Title,
                             FullName = p.flow.Staff.FName + " " + p.flow.Staff.LName,
                             ReceiveDate = p.previousFlow?.ResponseDate ?? p.request.RegisterDate,
                             ResponseDate = p.flow.ResponseDate,
                             ResponseTime = p.flow.ResponseTime,
                             Building = p.flow.Staff.BuildingId != null ? p.flow.Staff.Building.Title + "-" + p.flow.Staff.Building.Aux2 : null,
                             RequestNo = p.request.RequestNo,
                             FlowStatus = p.flow.LookUpFlowStatus.Title,
                             Phone = p.flow.Staff.LocalPhone,
                             Answer = (int)diff,
                             Sediment = (int)sediment,
                             WaitingTime = p.wfd.WaitingTime,
                             RequestId = p.request.Id
                         };
                break;
            case 4:
                result = from p in query
                         where p.request.RequestStatus.Code == 4
                         //let reqDate = DateTime.ParseExact(p.previousFlow != null ? p.previousFlow.ResponseDate + p.previousFlow.ResponseTime : p.request.RegisterDate + p.request.RegisterTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                         let reqDate = DateTime.ParseExact(p.flow.DelayDate + p.flow.DelayTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                         let datetime = DateTime.ParseExact(p.flow.ResponseDate + p.flow.ResponseTime, "yyyyMMddHHmm", null, DateTimeStyles.None)
                         // let diff = datetime.Subtract(reqDate).TotalHours
                         let diff = Math.Round(FlowRepository.CalculateDelay(reqDate, datetime, s2W, thr, holyday))
                         let sediment = (diff > p.wfd.WaitingTime) ? diff - p.wfd.WaitingTime : 0
                         select new WorkFlowDetailViewModel
                         {
                             Id = p.wfd.Id,
                             FlowId = p.flow.Id,
                             Title = p.wfd.Title,
                             FullName = p.flow.Staff.FName + " " + p.flow.Staff.LName,
                             ReceiveDate = p.previousFlow?.ResponseDate ?? p.request.RegisterDate,
                             ResponseDate = p.flow.ResponseDate,
                             ResponseTime = p.flow.ResponseTime,
                             Building = p.flow.Staff.BuildingId != null ? p.flow.Staff.Building.Title + "-" + p.flow.Staff.Building.Aux2 : null,
                             RequestNo = p.request.RequestNo,
                             FlowStatus = p.flow.LookUpFlowStatus.Title,
                             Phone = p.flow.Staff.LocalPhone,
                             Answer = (int)diff,
                             Sediment = (int)sediment,
                             WaitingTime = p.wfd.WaitingTime,
                             RequestId = p.request.Id
                         };
                break;
        }

        return result;
    }

    public void UploadStimullFile(IFormFile? file, string filePath, string webRootPath)
    {
        if (file != null)
        {
            if (file.Length > 0)
            {
                if (file.Length > 50 * 1024 * 1024)
                    throw new ArgumentException("حجم فایل آپلود شده نمیتوان بیش از 50 مگابایت باشد.");

                var fileName = file.FileName;
                var extension = Path.GetExtension(fileName).ToLower();
                if (extension != ".mrt")
                    throw new ArgumentException("فرمت فایل غیر مجاز است.");
                var targetFolder = Path.Combine(webRootPath, filePath);

                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                var path = Path.Combine(targetFolder, fileName);
                using Stream fileStream = new FileStream(path, FileMode.Create);
                file.CopyTo(fileStream);
            }
            else
                throw new ArgumentException("فایل مورد نظر آپلود نشد");
        }
    }

    public IEnumerable<DynamicViewModel> GetAllStimulInBpmn(string webRootPath)
    {
        var path = Path.Combine(webRootPath, "Images/upload/bpmn");
        var files = Directory.GetFiles(path);
        var list = new List<DynamicViewModel>();
        foreach (var file in files)
        {
            list.Add(new DynamicViewModel() { Title = Path.GetFileName(file) });
        }

        return list;
    }

    public IEnumerable<WorkFlowDetailViewModel> GetWorkflowDetainForAdHoc(Guid id)
    {
        var list = new List<WorkFlowDetailViewModel>();
        var workflowDetail = Context.WorkFlowDetails
            .Where(d => d.WorkFlowId == id && d.Step != int.MaxValue && d.Step != int.MinValue && d.CallProcessId == null).Select(s => new WorkFlowDetailViewModel()
            {
                Title = s.Title,
                Id = s.Id,
                RequestType = s.WorkFlow.RequestType.Title
            });
        list.AddRange(workflowDetail);
        var workflows = Context.Workflows.Where(d => d.SubProcessId == id).ToList();
        foreach (var workflow in workflows)
        {
            list.AddRange(GetWorkflowDetainForAdHoc(workflow.Id));
        }
        return list;
    }

    public IEnumerable<SelectListItem> GetAdHocWorkflowDetails(Request request, Workflow workflowBase)
    {
        var list = new List<SelectListItem>();
        var workFlowDetails = Context.Flows.Where(d =>
            d.LookUpFlowStatus.Code == 1 && d.IsActive && d.RequestId == request.Id).Select(s => s.WorkFlowDetail).ToList();

        var adHocWorkflowAcess =
            Context.WorkFlowDetails.Where(d => d.IsAdHoc && d.WorkFlowId == workflowBase.Id);

        foreach (var adHoc in adHocWorkflowAcess)
        {
            var workflowDetailAccess = JsonConvert.DeserializeObject<List<Guid>>(adHoc.AdHocWorkflowDetails);
            foreach (var item in workFlowDetails)
            {
                if (workflowDetailAccess.Any(d => d == item.Id))
                {
                    list.Add(new SelectListItem()
                    {
                        Value = adHoc.CallProcessId.ToString(),
                        Text = adHoc.Title
                    });
                }
            }
        }

        return list.Distinct();
    }

    public WorkFlowDetail FindWorkFlowDetailById(Guid workFlowDetailId)
    {
        return Context.WorkFlowDetails.Find(workFlowDetailId);
    }

    public WorkFlowDetail GetWorkFlow(Guid? workFlowDetailId)
    {
        return (from wd in Context.WorkFlowDetails
                join wf in Context.Workflows on wd.WorkFlowId equals wf.Id
                where /*wf.RequestTypeId == param.RequestTypeId &&*/ wd.Id == workFlowDetailId
                select wd).Single();
    }
}