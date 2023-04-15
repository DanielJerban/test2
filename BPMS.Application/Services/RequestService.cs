using BPMS.Domain.Common.Constants;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.Services;
using System.Globalization;

namespace BPMS.Application.Services;

public class RequestService : IRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFlowService _flowService;

    public RequestService(IUnitOfWork unitOfWork, IFlowService flowService)
    {
        _unitOfWork = unitOfWork;
        _flowService = flowService;
    }
    public string CreateRequest(CreateRequestDto dto, string webRootPath)
    {
        var requestId = Guid.NewGuid();

        var param = GetRequestFlowParam(dto);
        param.Work = dto.FormData;
        param.RequestId = requestId;

        _unitOfWork.WorkFlowFormLists.UpdateListInCartable(dto.GridList, requestId);
        var createWork = _flowService.CreateWork(param);
        if (createWork.IsSelectAcceptor)
        {
            throw new Exception("امکان شروع این فرآیند به دلیل قابل انتخاب بودن پاسخ دهنده مرحله بعد وجود ندارد.");
        }

        // TODO: must integrate with formData later
        //_unitOfWork.WorkFlowForm.UploadFile(files, requestId);

        _unitOfWork.Complete();

        _unitOfWork.Flows.SetDynamicWaitingTime(createWork.FlowId, param.Work);

        _flowService.AutoAcceptServiceTaskFlow(param.RequestId,webRootPath);

        _flowService.AutoAcceptOrRejectManualTaskFlow(param.RequestId);

        _unitOfWork.Flows.ChangeBalloonStatus(createWork.Request);

        _unitOfWork.Complete();

        var requestNo = _unitOfWork.Request.Single(c => c.Id == requestId).RequestNo;

        return requestNo.ToString();
    }

    public ShowFlowDetailsViewModel GetFlowDetailsByRequestId(Guid requestId, Guid? workFlowDetailId)
    {
        var flowsDetail = GetRequestWorkFlowDetails(requestId);
        var infos = _unitOfWork.Request.Single(r => r.Id == requestId);
        var org = _unitOfWork.OrganizationInfos.FirstOrDefault(i => i.OrganiztionPostId == infos.OrganizationPostTitleId);

        var activeFlow = _unitOfWork.Flows.FirstOrDefault(f => f.RequestId == requestId && f.LookUpFlowStatus.Code == 1);

        var activeWorkflowDetailId = (activeFlow != null ? activeFlow.WorkFlowDetailId : workFlowDetailId);

        if (activeFlow != null && activeFlow.CallActivityId != null && activeFlow.WorkFlowDetail.Step == 0)
        {
            var activeFlowEvent = activeFlow.FlowEvents.FirstOrDefault();
            if (activeFlowEvent is { IsActive: false })
            {
                activeWorkflowDetailId = activeFlow.PreviousFlow.WorkFlowDetailId;
            }
        }

        var staff = _unitOfWork.Staffs.Single(c => c.Id == infos.StaffId);
        var info = new FlowDetailsInfo()
        {
            RequestNo = infos.RequestNo.ToString(),
            ReqDate = HelperBs.MakeDate(infos.RegisterDate.ToString()),
            ReqTime = HelperBs.MakeTime(infos.RegisterTime),
            Requestor = staff.FName + " " + staff.LName,
            ReqStatus = infos.RequestStatus.Title,
            WorkflowDetailId = workFlowDetailId,
            ActiveWorkflowDetailId = activeWorkflowDetailId,
            RequestId = infos.Id,
            ReqStatusCode = infos.RequestStatus.Code,
            LocalPhone = staff.LocalPhone ?? "",
            Building = staff.BuildingId != null ? staff.Building.Title + "-" + staff.Building.Aux2 : null,
            CompanyName = org == null ? "" : _unitOfWork.Staffs.GetCompanyName(org.ChartId),
            PostTitle = infos.OrganizationPostTitle.Title
        };

        return new ShowFlowDetailsViewModel()
        {
            FlowDetails = flowsDetail,
            DetailsInfo = info
        };
    }

    public IEnumerable<FlowDetailsViewModel> GetRequestWorkFlowDetails(Guid requestId)
    {
        var data = (from flow in _unitOfWork.Flows.GetAsQueryable()
                    join request in _unitOfWork.Request.GetAsQueryable() on flow.RequestId equals request.Id
                    where flow.RequestId == requestId && flow.IsActive
                    orderby flow.Order
                    select new FlowDetailsViewModel()
                    {
                        FullName = flow.Staff.FName + " " + flow.Staff.LName,
                        Status = flow.LookUpFlowStatus.Title,
                        ResponeDate = flow.ResponseDate.ToString(),
                        ResponeTime = flow.ResponseTime,
                        RequestNo = request.RequestNo,
                        Dsr = flow.Dsr,
                        StepTitle = flow.WorkFlowDetail.Title,
                        LocalPhone = flow.Staff.LocalPhone,
                        Building = flow.Staff.BuildingId != null ? flow.Staff.Building.Title + "-" + flow.Staff.Building.Aux2 : null,
                        DelayDate = flow.DelayDate,
                        DelayTime = flow.DelayTime,
                        WaitingTime = flow.WorkFlowDetail.WaitingTime
                    }).ToList();

        long row = 0;

        var s2W = _unitOfWork.LookUps.GetByTypeAndCode("WorkTime", 1);
        var thr = _unitOfWork.LookUps.GetByTypeAndCode("WorkTime", 2);
        var holiday = _unitOfWork.Holydays.GetAll().ToList();
        foreach (var item in data)
        {
            row++;
            item.Row = row.ToString();

            var reqDate = DateTime.ParseExact(item.DelayDate + item.DelayTime, "yyyyMMddHHmm", null, DateTimeStyles.None);
            var timeToDo = _unitOfWork.Flows.CalculateTimeToDo(reqDate, item.WaitingTime, s2W, thr, holiday);
            var acceptTime = string.IsNullOrWhiteSpace(item.ResponeDate) ? DateTime.Now : DateTime.ParseExact(item.ResponeDate + item.ResponeTime, "yyyyMMddHHmm", null);
            var diff = Math.Round(_unitOfWork.Flows.CalculateDelay(reqDate, acceptTime, s2W, thr, holiday));
            var sediment = diff > item.WaitingTime ? diff - item.WaitingTime : 0;

            item.TimeToDo = timeToDo?.ToString("yyyy/MM/dd HH:mm");
            item.Delay = sediment.ToString();

        }

        return data;
    }

    private FlowParam GetRequestFlowParam(CreateRequestDto dto)
    {
        var workflow = CheckWorkflowExistence(dto.WorkflowExternalId);

        string personalCode = dto.PersonalCode;
        if (string.IsNullOrEmpty(dto.PersonalCode))
        {
            personalCode = SystemConstant.SystemUser;
        }

        var staff = GetStaff(personalCode);
        var staffOrganizationPostTitleId = GetStaffOrganizationPostId(staff.Id, personalCode);

        var workFlowDetail = _unitOfWork.WorkflowDetails.GetWithRequestType(workflow.RequestTypeId, 0);

        return new FlowParam()
        {
            StaffId = staff.Id,
            OrganizationPostTitleId = staffOrganizationPostTitleId,
            RequestTypeId = workflow.RequestTypeId,
            CurrentStep = workFlowDetail.Id
        };
    }

    private Guid GetStaffOrganizationPostId(Guid staffId, string personalCode)
    {
        var staffOrganizationPostTitleId = _unitOfWork.Staffs.GetStaffMainOrganizationInfoId(staffId);
        if (staffOrganizationPostTitleId == null)
        {
            throw new Exception($"کاربری با نام کاربری {personalCode} سمتی تعریف نشده است.");
        }

        return (Guid)staffOrganizationPostTitleId;
    }

    private Staff GetStaff(string personalCode)
    {
        var staff = _unitOfWork.Staffs.SingleOrDefault(c => c.PersonalCode == personalCode);
        if (staff == null)
        {
            throw new Exception($"کاربری با نام کاربری {personalCode} یافت نشد.");
        }

        return staff;
    }

    private Workflow CheckWorkflowExistence(Guid workflowExternalId)
    {
        var workflow = _unitOfWork.Workflows.SingleOrDefault(c => c.ExternalId == workflowExternalId && c.IsActive);
        if (workflow == null)
        {
            throw new Exception("هیچ فرآیند فعالی یافت نشد.");
        }

        return workflow;
    }
}