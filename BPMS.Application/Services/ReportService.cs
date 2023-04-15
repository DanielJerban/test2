using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;

namespace BPMS.Application.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCacheHelper _cacheHelper;

    public ReportService(IUnitOfWork unitOfWork, IDistributedCacheHelper cacheHelper)
    {
        _unitOfWork = unitOfWork;
        _cacheHelper = cacheHelper;
    }

    public List<Guid> GetAdminSubStaffs(string personalCode)
    {
        var thisStaff = _unitOfWork.Staffs.SingleOrDefault(c => c.PersonalCode == personalCode);
        if (thisStaff == null)
        {
            throw new ArgumentException("کاربری با این نام کاربری وجود ندارد!");
        }

        Guid staffId = thisStaff.Id;

        var query = _unitOfWork.OrganizationInfos.SingleOrDefault(w => w.StaffId == staffId);
        if (query == null)
        {
            throw new ArgumentException("پست سازمانی برای این کاربر یافت نشد.");
        }
        var checkIf = _unitOfWork.LookUps.SingleOrDefault(r => r.Id.ToString() == query.OrganiztionPost.Aux);
        if (checkIf.Aux2 != 1.ToString())
        {
            return new List<Guid>();
        }
        var chartIds = new List<Guid>();
        var staffs = new List<StaffDto>();
        chartIds.Add(query.Chart.Id);
        var loop = true;
        do
        {
            int count = 0;
            foreach (var item in chartIds)
            {
                foreach (var chart in _unitOfWork.Charts.GetAll())
                {
                    if (chart.ParentId == item)
                    {
                        if (!chartIds.Contains(chart.Id))
                        {
                            chartIds.Add(chart.Id);
                            count++;
                        }
                    }
                }
                if (count > 0)
                    break;
            }
            if (count == 0)
                loop = false;
        } while (loop);

        foreach (var item in chartIds)
        {

            foreach (var staff in _unitOfWork.OrganizationInfos.Find(w => w.ChartId == item))
            {
                staffs.Add(new StaffDto()
                {
                    Id = staff.Staff.Id,
                    FullName = staff.Staff.FullName
                });
            }
        }

        var adminSubStaffs = staffs.Select(c => c.Id).ToList();

        return adminSubStaffs;
    }

    public List<RequestDelayViewModel> GetIncompletedRequests()
    {
        // Incompleted preogress code
        int code = 1;

        var query1 = from w in _unitOfWork.WorkFlowConfermentAuthority.GetAll()
                     join wd in _unitOfWork.WorkFlowConfermentAuthorityDetail.GetAll() on w.Id equals wd.ConfermentAuthorityId
                     join workflows in _unitOfWork.Workflows.GetAll() on w.RequestTypeId equals workflows.RequestTypeId
                     join req in _unitOfWork.Request.GetAll() on workflows.Id equals req.WorkFlowId
                     join flow in _unitOfWork.Flows.GetAll() on req.Id equals flow.RequestId
                     where flow.StaffId == w.StaffId
                           && (req.RegisterDate >= wd.FromDate && req.RegisterDate <= wd.ToDate)
                           && flow.IsActive
                           && (code == 0 || flow.LookUpFlowStatus.Code == code)
                     select flow;

        var query2 = from f in _unitOfWork.Flows.GetAll()
                     where f.IsActive
                           && (code == 0 || f.LookUpFlowStatus.Code == code)
                     select f;

        var all = query1.Union(query2).Select(s => new RequestDelayViewModel()
        {
            PersonalName = s.Staff.FName + " " + s.Staff.LName,
            RequestNo = s.Request.RequestNo,
            FullName = s.Request.Staff.FName + " " + s.Request.Staff.LName,
            PersonalCode = s.Request.Staff.PersonalCode,
            CurrentStatus = s.LookUpFlowStatus.Title,
            RequestDate = s.Request.RegisterDate.ToString().Insert(4, "/").Insert(7, "/"),
            RequestTime = s.Request.RegisterTime.Insert(2, ":"),
            StepTitle = s.WorkFlowDetail.Title,
            FlowId = s.Id,
            RequestTypeTitle = s.WorkFlowDetail.WorkFlow.RequestType.Title + " / نسخه " + s.WorkFlowDetail.WorkFlow.OrginalVersion + "." + s.WorkFlowDetail.WorkFlow.SecondaryVersion,
            RequestId = s.RequestId,
            RequestTypeId = s.WorkFlowDetail.WorkFlow.RequestTypeId,
            WorkflowDetailId = s.WorkFlowDetailId,
            WorkflowId = s.WorkFlowDetail.WorkFlowId,
            Workflow = s.WorkFlowDetail.WorkFlow,
            StaffId = s.StaffId,
            ImagePath = s.Request.Staff.ImagePath,
            Name = s.WorkFlowDetail.WorkFlow.RequestType.Title,
            // for delay calculate
            DelayDate = s.DelayDate,
            DelayTime = s.DelayTime,
            WaitingTime = s.WorkFlowDetail.WaitingTime
        });

        var data = all.ToList();
        //if (data.Count > 0)
        //{
        //    data = data.Take(50).ToList();
        //}
        //var newData = SeperateWorkflowAndSubprocess(data);

        throw new NotImplementedException();
        //return newData;
    }

    public void SetPrintReportModelInCache(PrintReportViewModel model, Guid id)
    {
        _cacheHelper.SetObject(CacheKeyHelper.GetPrintReportCacheKey(id), model, TimeSpan.FromHours(8));
    }

    public PrintReportViewModel GetPrintReportModelFromCache(Guid id)
    {
        return _cacheHelper.GetObject<PrintReportViewModel>(CacheKeyHelper.GetPrintReportCacheKey(id));
    }

    public void ResetPrintReportCache(Guid id)
    {
        _cacheHelper.Remove(CacheKeyHelper.GetPrintReportCacheKey(id));
    }
}