using BPMS.Domain.Common.Constants;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Web;
using BPMS.Application.Hubs;

namespace BPMS.Application.Services;

public class DynamicFormService : IDynamicFormService
{
    private readonly CustomExceptionHandler _exceptions;
    private readonly IFlowParamService _flowParamService;
    private readonly IFlowService _flowService;
    private readonly IUnitOfWork _unitOfWork;

    public DynamicFormService(IUnitOfWork unitOfWork, IFlowParamService flowParamService, IFlowService flowService)
    {
        _unitOfWork = unitOfWork;
        _flowParamService = flowParamService;
        _flowService = flowService;
        _exceptions = new CustomExceptionHandler();
    }

    // TODO: uncomment later 
    //public object Create(string model, Guid unig, Guid requestId, string staffSelected, string gridList, string UserName, List<IFormFile> files, ControllerContext context, string webRootPath)
    //{
    //    var param = _flowParamService.GetFlowParamFromCache(UserName);


    //    param.RequestId = requestId;
    //    model = HttpUtility.UrlDecode(model, Encoding.UTF8);
    //    param.Work = model;
    //    if (!string.IsNullOrWhiteSpace(staffSelected) && staffSelected != "null")
    //    {
    //        var res = staffSelected.Split(',').ToList().ConvertAll(Guid.Parse);
    //        param.SelectedStaffIds = res;
    //    }

    //    try
    //    {
    //        _unitOfWork.WorkFlowFormLists.UpdateListInCartable(gridList, requestId);
    //        var result = _flowService.CreateWork(param);
    //        if (result.IsSelectAcceptor)
    //        {
    //            return new
    //            {
    //                isValid = true,
    //                status = "Create",
    //                partial = Util.RenderRazorViewToString(context, "_SelectStaff", new SelectAcceptorViewModel())
    //            };
    //        }

    //        _unitOfWork.WorkFlowForm.UploadFile(files, requestId, webRootPath);

    //        if (_unitOfWork.Request.Find(d => d.Id == result.Request.Id).Any())
    //        {
    //            _unitOfWork.Complete();

    //            _unitOfWork.Flows.SetDynamicWaitingTime(result.FlowId, param.Work);
    //            _flowService.AutoAcceptScriptTaskFlow(param.RequestId);
    //            _flowService.AutoAcceptServiceTaskFlow(param.RequestId, webRootPath);
    //            _flowService.AutoAcceptOrRejectManualTaskFlow(param.RequestId);

    //            try
    //            {
    //                _unitOfWork.Flows.ChangeBalloonStatus(result.Request);
    //                _unitOfWork.Complete();
    //                MainHub.UpdateNotificationCountOnMenuBar(UserName == SystemConstant.SystemUser
    //                    ? SystemConstant.SystemUser
    //                    : UserName);
    //            }
    //            catch (Exception e)
    //            {
    //                _exceptions.HandleException(e);
    //            }
    //        }

    //        _flowParamService.ResetFlowParamCache(UserName);

    //        return new { isValid = true, msg = "درخواست مورد نظر با موفقیت ثبت شد.", flowId = result.FlowId };

    //    }
    //    catch (Exception e)
    //    {
    //        _flowParamService.ResetFlowParamCache(UserName);

    //        return new { isValid = false, msg = e.Message, innerexp = e.InnerException?.InnerException?.Message };
    //    }
    //}

    public bool IsViewNameCreateProcess(Guid requestTypeId, Guid? organizationPostTitleId, Guid unig, Guid? staffId,
        out WorkFlowFormViewModel model, out string[] viewName
        , string userName, string webRootPath)
    {
        viewName = null;
        model = new WorkFlowFormViewModel();
        if (organizationPostTitleId == Guid.Empty || organizationPostTitleId == null)
        {
            throw new ArgumentException("پست سازمانی انتخاب نشده است");
        }

        var workFlowDetail = _unitOfWork.WorkflowDetails.GetWithRequestType(requestTypeId, 0);

        var workflowDetailId = workFlowDetail.Id;
        var workflowId = _unitOfWork.WorkflowDetails.SingleOrDefault(c => c.Id == workflowDetailId).WorkFlowId;
        workflowId = GetParentWorkFlowId(workflowId);
        var workflowBase = _unitOfWork.Workflows.Get(workflowId);
        string fileName = "";

        if (Directory.Exists(Path.Combine(webRootPath, "Images/upload/WorkflowTutorial/" + workflowId)))
        {
            var directoryFiles = Directory.GetFiles(Path.Combine(webRootPath, "Images/upload/WorkflowTutorial/" + workflowId));
            if (directoryFiles.Length > 0)
            {
                var filePath = Directory.GetFiles(Path.Combine(webRootPath, "Images/upload/WorkflowTutorial/" + workflowId + "/"))[0];
                fileName = Path.GetFileName(filePath);
            }
        }

        _flowParamService.SetFlowParamInCache(
            new FlowParam()
            {
                StaffId = staffId.Value,
                OrganizationPostTitleId = organizationPostTitleId.Value,
                RequestTypeId = requestTypeId,
                CurrentStep = workFlowDetail.Id
            }, userName);

        if (workFlowDetail.ViewName != null)
        {
            viewName = workFlowDetail.ViewName.Split('/');
            return true;
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

        var previousForm = _unitOfWork.WorkFlowForm.GetPreviousForm(json, null);

        model = new WorkFlowFormViewModel()
        {
            FullFormData = json,
            Id = workFlowDetail.WorkFlowForm.Id,
            PName = workFlowDetail.WorkFlowForm.PName,
            JsonFile = encoodeJson,
            uniqeid = unig,
            PreviousForm = previousForm,
            EditableCheck = workFlowDetail.EditableFields,
            HiddenFields = workFlowDetail.HiddenFields,
            Value = "{}",
            WorkFlowDetailId = workFlowDetail.Id,
            Jquery = jquery,
            AdditionalCssStyleCode = CssCode,
            PrintFileName = workFlowDetail.PrintFileName,
            Dsr = workFlowDetail.Dsr,
            DocumentCode = workFlowDetail.WorkFlowForm.DocumentCode,
            WorkFlowAbout = workflowBase.About,
            TutorialFileName = fileName
        };
        return false;
    }

    /// <summary>
    /// Gets subprocess first parent id
    /// </summary>
    /// <param name="id">workflowId</param>
    public Guid GetParentWorkFlowId(Guid? id)
    {

        var thisWorkFlow = _unitOfWork.Workflows.SingleOrDefault(c => c.Id == id);
        Guid workflowId = thisWorkFlow.Id;
        if (thisWorkFlow.SubProcessId == null)
        {
            return workflowId;
        }
        Guid? subprocessId = thisWorkFlow.SubProcessId;

        return GetParentWorkFlowId(subprocessId);
    }
}