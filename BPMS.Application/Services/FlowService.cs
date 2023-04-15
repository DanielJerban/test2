using BPMS.Domain.Common.Constants;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using BPMS.Application.Hubs;
using BPMS.Infrastructure.Services.SMS;
using Microsoft.EntityFrameworkCore;

namespace BPMS.Application.Services;

public class FlowService : IFlowService
{
    private SelectAcceptorViewModel selectAcceptor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExternalApiService _externalApiService;
    private readonly ISendingSmsService _sendingSmsService;
    public FlowService(IUnitOfWork unitOfWork, IExternalApiService externalApiService, ISendingSmsService sendingSmsService)
    {
        _unitOfWork = unitOfWork;
        _externalApiService = externalApiService;
        _sendingSmsService = sendingSmsService;
    }


    public void SendMessageForRejectRequest(FlowParam param, Request currentRequest)
    {
        if (_unitOfWork.LookUps.Single(d => d.Type == "SmsRejectFlow").IsActive)
        {
            var workflowDetail = _unitOfWork.WorkflowDetails.FindWorkFlowDetailById(param.WorkFlowDetailId);
            var text = @"همکار گرامی، درخواست شما با عنوان """ + workflowDetail?.WorkFlow.RequestType.Title +
                       @""" به شماره """ + currentRequest.RequestNo + @""" در مرحله """ + workflowDetail?.Title +
                       @""" رد شد." + Environment.NewLine +
                       @"سامانه مدیریت فرآیند های کسب و کار طرفه نگار";

            _sendingSmsService.SendSms(new List<string> { currentRequest.Staff.PhoneNumber }, text);
        }

        if (_unitOfWork.LookUps.Single(d => d.Type == "EmailRejectFlow").IsActive)
        {
            var workflowDetail = _unitOfWork.WorkflowDetails.FindWorkFlowDetailById(param.WorkFlowDetailId);
            var text = @"همکار گرامی، درخواست شما با عنوان """ + workflowDetail?.WorkFlow.RequestType.Title +
                       @""" به شماره """ + currentRequest.RequestNo + @""" در مرحله @""" + workflowDetail?.Title +
                       @""" رد شد." + Environment.NewLine +
                       @"سامانه مدیریت فرآیند های کسب و کار طرفه نگار";

            // todo: uncomment when moved this method to service
            //if (currentRequest.Staff.Email != null)
            //    new EmailSender().Send(new List<string>() { currentRequest.Staff.Email }, new MessageContent() { Subject = "BPMS", Body = text });
        }
    }

    public void ChangeRequestStatus(Guid requestId, Guid statusId, FlowParam param, Guid? callprocessId)
    {
        var currentRequest = _unitOfWork.Request.GetRequestById(requestId);
        if (currentRequest != null)
        {
            currentRequest.RequestStatusId = statusId;
            var acceptId = _unitOfWork.LookUps.Single(c => c.Code == 3 && c.Type == "RequestStatus").Id;
            var rejectId = _unitOfWork.LookUps.Single(c => c.Code == 4 && c.Type == "RequestStatus").Id;
            // خاتمه یافته
            if (statusId == acceptId || statusId == rejectId)
            {
                if (statusId == rejectId)
                {
                    SendMessageForRejectRequest(param, currentRequest);
                }
                else
                {
                    var flow = _unitOfWork.Flows.Find(i => i.Id == param.CurrentFlowId).FirstOrDefault();
                    if (flow.CallActivityId != null && param.BoundaryName == null)
                    {
                        param.CurrentStep = flow.CallActivityId;
                        if (callprocessId != null)
                        {

                            // var currentWfd = _dbContext.WorkFlowDetails.Single(d => d.Id == flow.WorkFlowDetailId);
                            var currentWfd = _unitOfWork.WorkflowDetails.Single(d => d.Id == callprocessId);
                            var sefrStep = currentWfd.WorkFlow.WorkflowDetails.Single(d => d.Step == 0);
                            var firstFlow = _unitOfWork.Flows.GetFlowByRequestIdAndWorkFlowDetailId(flow.RequestId, sefrStep.Id);

                            if (firstFlow != null)
                            {
                                param.CurrentStep = firstFlow.CallActivityId;
                                GotoNextStepInCallProcess(param);
                            }

                        }
                        else
                        {
                            GotoNextStepInCallProcess(param);
                        }

                    }

                }

            }

        }
        _unitOfWork.Request.AddOrUpdate(currentRequest);
        _unitOfWork.Complete();
        MainHub.UpdateDashboardCharts();
    }
    public SelectAcceptorViewModel RejectFlow(FlowParam param)
    {
        selectAcceptor = new SelectAcceptorViewModel();
        var work = param.Work;
        var nextflowstaffresult = GetNextFlowStaff(param);
        if (nextflowstaffresult.IsSelectAcceptor)
        {
            selectAcceptor.IsSelectAcceptor = true;
            return selectAcceptor;
        }

        param.Work = work;
        if (nextflowstaffresult.End == "Terminate" || nextflowstaffresult.Evt == "A") param.IsEnd = true;
        var currentFlow = _unitOfWork.Flows.ChangeFlowStatus(param, _unitOfWork.LookUps.Single(c => c.Code == 3 && c.Type == "FlowStatus").Id);
        if (nextflowstaffresult.Evt == "A")
        {
            var currentFlowList = _unitOfWork.Flows.GetFlowList(currentFlow.RequestId,
                param.CurrentFlowId, currentFlow.WorkFlowDetailId);
            if (currentFlow.WorkFlowDetail.IsOrLogic ||
                !currentFlowList.Any(p => p.FlowStatusId != _unitOfWork.LookUps.Single(c => c.Code == 2 && c.Type == "FlowStatus").Id))
            {
                if (currentFlow.WorkFlowDetail.IsOrLogic)
                    _unitOfWork.Flows.RemoveRange(currentFlowList);
            }


            HandleEndEvent(nextflowstaffresult.WorkflowEsbs, param);

            var noActionFlowsExists = _unitOfWork.Flows.Include(c =>
                c.LookUpFlowStatus).Any(c => c.RequestId == param.RequestId && c.LookUpFlowStatus.Code == 1 &&
                                             c.LookUpFlowStatus.Type == "FlowStatus" && c.Id != currentFlow.Id);
            // if there is no flows with noAction state change the request status 
            if (!noActionFlowsExists)
            {
                // خاتمه یافته رد شده
                ChangeRequestStatus(currentFlow.RequestId, _unitOfWork.LookUps.Single(c => c.Code == 4 && c.Type == "RequestStatus").Id, param, null);
            }
            else
            {
                // در حال اقدام
                ChangeRequestStatus(param.RequestId, _unitOfWork.LookUps.Single(c => c.Code == 2 && c.Type == "RequestStatus").Id, param, null);
            }

            //signalR

            var rejectorpersonelCode = _unitOfWork.Flows.GetRejectorPersonalCode(param.ConfirmStaffId, param.RequestId);
            var requestorPerson = _unitOfWork.Staffs.Where(s => s.Id == param.StaffId).Select(t => t.PersonalCode)
                .ToList();

            var totalPersonelCodes = rejectorpersonelCode.Union(requestorPerson).ToList();
            MainHub.UpdateNotificationCount(totalPersonelCodes);

            //  SendSmsAndEmail(nextflowstaffresult.EsbList);

        }
        else
        {
            AcceptRejectSimilarCode(currentFlow.WorkFlowDetail, param, nextflowstaffresult);
        }
        selectAcceptor.FlowId = _unitOfWork.Flows.NextFlowIdIfAcepptorExist(param.ConfirmStaffId);
        return selectAcceptor;
    }
    public SelectAcceptorViewModel AcceptFlow(FlowParam param)
    {
        selectAcceptor = new SelectAcceptorViewModel();
        var work = param.Work;
        var nextflowstaffresult = GetNextFlowStaff(param);
        if (nextflowstaffresult.IsSelectAcceptor)
        {
            selectAcceptor.IsSelectAcceptor = true;
            return selectAcceptor;
        }

        param.Work = work;
        //var serialize = JsonConvert.SerializeObject(nextflowstaffresult);
        if (nextflowstaffresult.End == "Terminate") param.IsEnd = true;
        if (param.IsAdHok)
        {
            byte[] bytes = null;
            if (param.Work != null)
            {
                Type typeWork = param.Work.GetType();
                if (typeWork.Name.ToLower() == "string")
                {
                    var content = HttpUtility.UrlDecode(param.Work, System.Text.Encoding.UTF8);
                    var encoding = new UnicodeEncoding();
                    bytes = encoding.GetBytes(content);
                }
            }
            //  var request = _unitOfWork.Request.Find(param.RequestId);
            var request = _unitOfWork.Request.GetRequestById(param.RequestId);
            request.Value = bytes;


            request.Flows.Add(new Flow()
            {
                Dsr = param.Dsr,
                FlowStatusId = _unitOfWork.LookUps.Single(c => c.Code == 2 && c.Type == "FlowStatus").Id,
                IsActive = true,
                DelayDate = int.Parse(DateTime.Now.ToString("yyyyMMdd")),
                DelayTime = DateTime.Now.ToString("HHmm"),
                WorkFlowDetailId = param.WorkFlowDetailId,
                IsEnd = param.IsEnd,
                StaffId = param.StaffId,
                ResponseDate = int.Parse(DateTime.Now.ToString("yyyyMMdd")),
                ResponseTime = DateTime.Now.ToString("HHmm"),
                PreviousWorkFlowDetailId = param.CurrentStep
            });
            // var wfd = _unitOfWork.WorkflowDetails.Find(param.WorkFlowDetailId);
            var wfd = _unitOfWork.WorkflowDetails.FindWorkFlowDetailById(param.WorkFlowDetailId);
            AcceptRejectSimilarCode(wfd, param, nextflowstaffresult);
        }
        else
        {
            var currentFlow = _unitOfWork.Flows.ChangeFlowStatus(param, _unitOfWork.LookUps.Single(c => c.Code == 2 && c.Type == "FlowStatus").Id);
            AcceptRejectSimilarCode(currentFlow.WorkFlowDetail, param, nextflowstaffresult);
        }

        selectAcceptor.FlowId = _unitOfWork.Flows.NextFlowIdIfAcepptorExist(param.ConfirmStaffId);
        _unitOfWork.Complete();
        return selectAcceptor;
    }

    public void AcceptRejectSimilarCode(WorkFlowDetail workflowDetail, FlowParam param, MainFlowStaff nextflowstaffresult)
    {
        var nextFlows = new List<Flow>();
        var currentFlowList = _unitOfWork.Flows
            .Where(f => f.RequestId == param.RequestId && f.Id != param.CurrentFlowId &&
                        f.WorkFlowDetailId == workflowDetail.Id && f.LookUpFlowStatus.Code == 1).ToList();
        if (workflowDetail.IsOrLogic ||
            currentFlowList.All(p => p.FlowStatusId == _unitOfWork.LookUps.Single(c => c.Code == 2 && c.Type == "FlowStatus").Id))
        {
            List<Flow> flowNotProgress;
            if (workflowDetail.IsOrLogic)
            {
                flowNotProgress = _unitOfWork.Flows
                    .Where(f => f.RequestId == param.RequestId && f.LookUpFlowStatus.Code == 1 && f.WorkFlowDetailId != param.WorkFlowDetailId).ToList();
                _unitOfWork.Flows.RemoveRange(currentFlowList);
            }
            else
            {
                flowNotProgress = _unitOfWork.Flows
                    .Where(f => f.RequestId == param.RequestId && f.LookUpFlowStatus.Code == 1 && f.Id != param.CurrentFlowId).ToList();

            }

            foreach (var item in nextflowstaffresult.Gateways/*.Distinct()*/)
            {
                if (item.Contains("inclusive"))
                {
                    // baraye mahdood kardane workflowdetail ha ke taeed shode ya rad shode nabashand
                    var b = new List<WorkFlowDetail>();

                    foreach (var itemFlow in flowNotProgress)
                    {
                        var wfns = itemFlow.WorkFlowDetail.WorkFlowNextStepsFrom.ToList();
                        foreach (var workFlowNextStep in wfns)
                        {
                            var allLaterWfns = _unitOfWork.Flows.GetAllWorkFlowNextSteps(workFlowNextStep.FromWfdId);
                            allLaterWfns.Add(workFlowNextStep.NextStepFromWfd);

                            b.AddRange(allLaterWfns);
                            b = b.Distinct().ToList();
                        }
                    }

                    foreach (var wfd in b)
                    {
                        var wfns = wfd.WorkFlowNextStepsFrom.ToList();
                        foreach (var workFlowNextStep in wfns)
                        {
                            if (workFlowNextStep.Gateway == null) continue;
                            var gateways = workFlowNextStep.Gateway.Split('@');
                            if (workFlowNextStep.Gateway.Contains(item))
                            {

                                var index = Array.FindIndex(gateways, d => d == item);
                                if (index > 0)
                                {
                                    var w = gateways[index - 1];
                                    if (w.Contains("inclusive") || w.Contains("exclusive"))
                                    {
                                        if (!_unitOfWork.Flows.CheckInclusivePath(workflowDetail, param, w))
                                        {
                                            ChangeRequestStatus(param.RequestId,
                                                _unitOfWork.LookUps.Single(c => c.Code == 2 && c.Type == "RequestStatus").Id, param, null);
                                            return;
                                        };
                                    }

                                }
                                else
                                {
                                    ChangeRequestStatus(param.RequestId,
                                        _unitOfWork.LookUps.Single(c => c.Code == 2 && c.Type == "RequestStatus").Id, param, null);
                                    return;
                                }

                            }
                        }
                    }

                }
                if (item.Contains("parallel"))
                {
                    // tamame workflowdetail ha
                    var a = _unitOfWork.WorkflowDetails
                        .Where(f => f.WorkFlowId == workflowDetail.WorkFlowId
                                    && f.Step == null
                                    && f.Id != param.WorkFlowDetailId).ToList();

                    // baraye mahdood kardane workflowdetail ha ke taeed shode ya rad shode nabashand
                    var b = new List<WorkFlowDetail>();
                    foreach (var wfd in a)
                    {
                        // if (wfd.Flows.All(d => param.RequestId != d.RequestId) || wfd.Flows.Any(d => d.LookUpFlowStatus.Code == 1 && param.RequestId == d.RequestId))
                        // {
                        //     b.Add(wfd);
                        // }

                        if (wfd.Flows.Any(d => d.LookUpFlowStatus.Code == 1 && param.RequestId == d.RequestId))
                        {
                            b.Add(wfd);
                        }

                    }


                    foreach (var wfd in b)
                    {
                        var wfns = wfd.WorkFlowNextStepsFrom.ToList();
                        foreach (var workFlowNextStep in wfns)
                        {
                            if (workFlowNextStep.Gateway == null) continue;
                            var gateways = workFlowNextStep.Gateway.Split('@');
                            if (workFlowNextStep.Gateway.Contains(item))
                            {

                                var index = Array.FindIndex(gateways, d => d == item);
                                if (index > 0)
                                {
                                    var w = gateways[index - 1];
                                    if (w.Contains("inclusive") || w.Contains("exclusive"))
                                    {
                                        if (!_unitOfWork.Flows.CheckInclusivePath(workflowDetail, param, w))
                                        {
                                            ChangeRequestStatus(param.RequestId,
                                                _unitOfWork.LookUps.Single(c => c.Code == 2 && c.Type == "RequestStatus").Id, param, null);
                                            return;
                                        };
                                    }


                                }
                                else
                                {
                                    ChangeRequestStatus(param.RequestId,
                                        _unitOfWork.LookUps.Single(c => c.Code == 2 && c.Type == "RequestStatus").Id, param, null);
                                    return;
                                }


                            }
                        }
                    }
                }
            }

            DetectedRequestStatus(param, nextflowstaffresult, nextFlows, flowNotProgress, null);
        }

        //signalR
        _unitOfWork.Flows.SendNotification(param.RequestId, nextflowstaffresult);

    }

    public void DetectedRequestStatus(FlowParam param, MainFlowStaff nextflowstaffresult, List<Flow> nextFlows, List<Flow> flowNotProgress, Guid? callprocessId)
    {
        if (nextflowstaffresult.FlowStaves != null)
        {
            foreach (var flowStaff in nextflowstaffresult.FlowStaves)
            {
                var flows = FlowStaffToFlow(flowStaff, param.CurrentFlowId, param.CurrentStep);
                foreach (var flow in flows)
                {
                    if (nextFlows.Any(c =>
                            c.RequestId == flow.RequestId && c.StaffId == flow.StaffId &&
                            c.WorkFlowDetailId == flow.WorkFlowDetailId) == false)
                    {
                        flow.FlowEvents = flow.FlowEvents.ToList();
                        nextFlows.Add(flow);
                    }
                }

            }

            _unitOfWork.Flows.AddRange(nextFlows);
            // در حال اقدام
            ChangeRequestStatus(param.RequestId, _unitOfWork.LookUps.Single(c => c.Code == 2 && c.Type == "RequestStatus").Id, param, null);
        }
        else if (nextflowstaffresult.End == "Terminate")
        {
            HandleEndEvent(nextflowstaffresult.WorkflowEsbs, param);
            // خاتمه یافته تایید شده
            ChangeRequestStatus(param.RequestId, _unitOfWork.LookUps.Single(c => c.Code == 3 && c.Type == "RequestStatus").Id, param, callprocessId);
            //  SendSmsAndEmail(nextflowstaffresult.EsbList);

        }
        else
        {

            HandleEndEvent(nextflowstaffresult.WorkflowEsbs, param);
            // در حال اقدام
            if (flowNotProgress.Any() || (param.BoundaryName != null && param.BoundaryName.Contains("ErrorEvent")))
            {
                ChangeRequestStatus(param.RequestId, _unitOfWork.LookUps.Single(c => c.Code == 2 && c.Type == "RequestStatus").Id, param, null);
            }
            else
            {
                // خاتمه یافته تایید شده
                ChangeRequestStatus(param.RequestId, _unitOfWork.LookUps.Single(c => c.Code == 3 && c.Type == "RequestStatus").Id, param, callprocessId);
            }

            //    SendSmsAndEmail(nextflowstaffresult.EsbList);

        }
    }

    public void GotoNextStepInCallProcess(FlowParam param)
    {
        var nextflowstaffresult = GetNextFlowStaff(param);
        if (nextflowstaffresult.IsSelectAcceptor)
        {
            selectAcceptor.IsSelectAcceptor = true;
        }

        var flowNotProgress = _unitOfWork.Flows
            .Where(f => f.RequestId == param.RequestId && f.LookUpFlowStatus.Code == 1 && f.Id != param.CurrentFlowId)
            .ToList();
        //if (param.BoundaryName == null)
        //{
        DetectedRequestStatus(param, nextflowstaffresult, new List<Flow>(), flowNotProgress, param.CurrentStep);
        _unitOfWork.Flows.SendNotification(param.RequestId, nextflowstaffresult);
        //}
    }

    public void HandleEndEvent(List<WorkflowEsb> nextflowstaffresult, FlowParam param)
    {
        if (nextflowstaffresult != null)
        {
            foreach (var workflowEsb in nextflowstaffresult)
            {

                if (workflowEsb != null && workflowEsb.EventId.Contains("intermediateThrowEventMessage"))
                {
                    var esbjson = JsonConvert.DeserializeObject<EsbViewModel>(workflowEsb.Info);
                    esbjson.Param = param;
                    //SendSmsAndEmail(esbjson);
                    SendSmsAndEmail(esbjson, esbjson.Param.Work);
                }


                if (workflowEsb != null && workflowEsb.EventId.Contains("intermediateThrowEventSignal"))
                {
                    var obj = JObject.Parse(workflowEsb.Info);
                    var signal = new SignalViewModel()
                    {
                        SendRemoteId = obj.GetValue("SendRemoteId").ToString(),
                        Param = param
                    };
                    SendSignalMessage(signal);
                }
                if (workflowEsb != null && workflowEsb.EventId.Contains("errorEvent"))
                {
                    var obj = JObject.Parse(workflowEsb.Info);
                    var errorId = obj.GetValue("ErrorId").ToString();

                    var currentWorkflowDetail = _unitOfWork.WorkflowDetails.FindWorkFlowDetailById(param.WorkFlowDetailId);

                    if (currentWorkflowDetail != null)
                    {
                        var parentWorkflow = currentWorkflowDetail.WorkFlow.SubProcessId;
                        // var boundary = _dbContext.WorkFlowBoundaries.FirstOrDefault(d => d.WorkflowDetailId == wfd.FirstOrDefault(c => c.Id == d.WorkflowDetailId).Id);
                        var boundaries = _unitOfWork.Flows.GetWorkflowBoundaries(parentWorkflow.Value);
                        var parentWorkflowDetail = _unitOfWork.Flows.Single(d => d.Id == param.CurrentFlowId).CallActivityId;
                        var boundary = new WorkFlowBoundary();

                        foreach (var workFlowBoundary in boundaries)
                        {
                            var recriveErrorId = JObject.Parse(workFlowBoundary.Info).GetValue("RecriveErrorId").ToString();
                            if (errorId == recriveErrorId && workFlowBoundary.WorkflowDetailId == parentWorkflowDetail)
                                boundary = workFlowBoundary;
                        }

                        param.CurrentStep = boundary.WorkflowDetailId;
                        param.BoundaryName = boundary.BoundaryId;

                        var nerx = GetNextFlowStaff(param);
                        if (nerx.IsSelectAcceptor)
                        {
                            selectAcceptor.IsSelectAcceptor = true;
                        }
                        var flowNotProgress = _unitOfWork.Flows
                            .Where(f => f.RequestId == param.RequestId && f.LookUpFlowStatus.Code == 1 && f.Id != param.CurrentFlowId)
                            .ToList();
                        DetectedRequestStatus(param, nerx, new List<Flow>(), flowNotProgress, param.CurrentStep);
                        _unitOfWork.Flows.SendNotification(param.RequestId, nerx);
                    }
                }
            }
        }
    }


    public void AutoAcceptScriptTaskFlow(Guid requestId)
    {
        var userDefult = _unitOfWork.Staffs.Single(c => c.PersonalCode == "555556");
        var newQuery = _unitOfWork.Flows.GetFlowAndWorkFlowDetailByRequestIdAndStaffId(requestId, userDefult.Id);

        foreach (var item in newQuery)
        {
            try
            {
                var encoding = new UnicodeEncoding();
                var value = encoding.GetString(item.Flow.Request.Value);
                var param = new FlowParam
                {
                    StaffId = item.Flow.Request.StaffId,
                    OrganizationPostTitleId = item.Flow.Request.OrganizationPostTitleId,
                    RequestTypeId = item.Flow.Request.Workflow.RequestTypeId,
                    CurrentStep = item.Flow.WorkFlowDetailId,
                    ConfirmStaffId = item.Flow.StaffId,
                    CurrentFlowId = item.Flow.Id,
                    RequestId = item.Flow.RequestId,
                    Evt = "A",
                    Work = value,
                    BoundaryName = null,
                    ApiStaffId = item.Flow.StaffId
                };
                var nextFlowStaffResult = GetNextFlowStaff(param);
                param.Work = value;
                if (nextFlowStaffResult.End == "Terminate") param.IsEnd = true;
                var flowStatusAcceptId = _unitOfWork.LookUps.GetByTypeAndCode("FlowStatus", 2).Id;
                var currentFlow = _unitOfWork.Flows.ChangeFlowStatus(param, flowStatusAcceptId);
                AcceptRejectSimilarCode(currentFlow.WorkFlowDetail, param, nextFlowStaffResult);
                _unitOfWork.Complete();
                string content = item.WorkFlowDetail.ScriptTaskMethod;
                // todo: uncomment later 
                var result = ""; //CompileRuntime.RoslynChangeJsonVaule(param, item.WorkFlowDetail.ScriptTaskMethod);
                var request = _unitOfWork.Request.Single(c => c.Id == item.Flow.RequestId);
                var requestValue = encoding.GetString(request.Value);
                requestValue = result;
                request.Value = encoding.GetBytes(requestValue);
                _unitOfWork.Request.AddOrUpdate(request);
                _unitOfWork.Complete();

            }

            catch (Exception e)
            {
                var exceptions = new CustomExceptionHandler();
                exceptions.HandleException(e);
                exceptions.HandleException(e,
                    $"Error Occured on auto accepting scripttask task. \nFlowId: {item.Flow.Id}, RequestId: {item.Flow.RequestId}\nError Message: {e.Message}\nInner Exception: {e.InnerException?.Message}");
            }
        }

        if (newQuery.Count > 0)
        {
            // check if any script task exists after this level 
            AutoAcceptScriptTaskFlow(requestId);
        }
    }


    public void AutoAcceptOrRejectManualTaskFlow(Guid requestId)
    {
        var userDefult = _unitOfWork.Staffs.Single(c => c.PersonalCode == "555556");
        var newQuery =
            _unitOfWork.Flows.GetFlowAndWorkFlowDetailByRequestIdAndStaffIdForManualTask(requestId, userDefult.Id);

        foreach (var item in newQuery)
        {
            try
            {
                var encoding = new UnicodeEncoding();
                var value = encoding.GetString(item.Flow.Request.Value);
                var param = new FlowParam
                {
                    StaffId = item.Flow.Request.StaffId,
                    OrganizationPostTitleId = item.Flow.Request.OrganizationPostTitleId,
                    RequestTypeId = item.Flow.Request.Workflow.RequestTypeId,
                    CurrentStep = item.Flow.WorkFlowDetailId,
                    ConfirmStaffId = item.Flow.StaffId,
                    CurrentFlowId = item.Flow.Id,
                    RequestId = item.Flow.RequestId,
                    Evt = "A",
                    Work = value,
                    BoundaryName = null,
                    ApiStaffId = item.Flow.StaffId
                };
                var nextFlowStaffResult = GetNextFlowStaff(param);
                param.Work = value;
                if (nextFlowStaffResult.End == "Terminate") param.IsEnd = true;
                var flowStatusAcceptId = _unitOfWork.LookUps.GetByTypeAndCode("FlowStatus", 2).Id;
                var currentFlow = _unitOfWork.Flows.ChangeFlowStatus(param, flowStatusAcceptId);
                AcceptRejectSimilarCode(currentFlow.WorkFlowDetail, param, nextFlowStaffResult);
                _unitOfWork.Complete();
            }
            catch (Exception e)
            {
                var exceptions = new CustomExceptionHandler();
                exceptions.HandleException(e);
                exceptions.HandleException(e, "Error Occured on auto accepting manual task. \n" +
                                              $"FlowId: {item.Flow.Id}, RequestId: {item.Flow.RequestId}\n" +
                                              $"Error Message: {e.Message}\n" +
                                              $"Inner Exception: {e.InnerException?.Message}");
            }
        }

        if (newQuery.Count > 0)
        {
            // check if any manual task exists after this level 
            AutoAcceptOrRejectManualTaskFlow(requestId);
        }
    }

    public void AutoAcceptServiceTaskFlow(Guid requestId, string webRootPath)
    {
        var userDefult = _unitOfWork.Staffs.Single(c => c.PersonalCode == "555556");
        var newQuery =
            _unitOfWork.Flows.GetFlowAndWorkFlowDetailByRequestIdAndStaffIdForServiceTask(requestId, userDefult.Id);
        foreach (var item in newQuery)
        {
            try
            {
                var encoding = new UnicodeEncoding();
                var value = encoding.GetString(item.Flow.Request.Value);
                var param = new FlowParam
                {
                    StaffId = item.Flow.Request.StaffId,
                    OrganizationPostTitleId = item.Flow.Request.OrganizationPostTitleId,
                    RequestTypeId = item.Flow.Request.Workflow.RequestTypeId,
                    CurrentStep = item.Flow.WorkFlowDetailId,
                    ConfirmStaffId = item.Flow.StaffId,
                    CurrentFlowId = item.Flow.Id,
                    RequestId = item.Flow.RequestId,
                    Evt = "A",
                    Work = value,
                    BoundaryName = null,
                    ApiStaffId = item.Flow.StaffId
                };
                var nextFlowStaffResult = GetNextFlowStaff(param);
                param.Work = value;
                if (nextFlowStaffResult.End == "Terminate") param.IsEnd = true;
                var flowStatusAcceptId = _unitOfWork.LookUps.GetByTypeAndCode("FlowStatus", 2).Id;
                var currentFlow = _unitOfWork.Flows.ChangeFlowStatus(param, flowStatusAcceptId);
                AcceptRejectSimilarCode(currentFlow.WorkFlowDetail, param, nextFlowStaffResult);

                _unitOfWork.Complete();

                // set responce data from external api into param.work => jsonValue in front 
                var objectName = item.WorkFlowDetail.ServiceTaskApiResponse;
                var externalApiResult = _externalApiService.TestApiById((Guid)item.WorkFlowDetail.ExternalApiId, param.Work, webRootPath, true);
                var content = externalApiResult.Content;

                if (!externalApiResult.Success)
                {
                    var workFlow = _unitOfWork.Flows.GetWorkFlowIncludedTypeByIB(item.WorkFlowDetail.WorkFlowId);

                    // log data
                    _unitOfWork.ServiceTaskLog.CreateServiceTaskLog(new ServiceTaskLog()
                    {
                        ExternalApiDataJson = JsonConvert.SerializeObject(externalApiResult),
                        WorkFlowDetailTitle = item.WorkFlowDetail.Title,
                        WorkFlowTitle = workFlow.RequestType.Title,
                        WorkFlowVersion = workFlow.OrginalVersion + "." + workFlow.SecondaryVersion,
                        ServiceTaskObjName = objectName,
                        CreatedDate = int.Parse(DateTime.Now.ToString("yyyyMMdd")),
                        CreatedTime = DateTime.Now.ToString("HHmm")
                    });

                    _unitOfWork.Complete();

                    content = "";
                }
                else
                {
                    if (content.StartsWith("\""))
                    {
                        content = content.Remove(0, 1);
                    }

                    if (content.EndsWith("\""))
                    {
                        content = content.Remove(content.Length - 1, 1);
                    }
                }


                var request = _unitOfWork.Request.Single(c => c.Id == item.Flow.RequestId);
                var requestValue = encoding.GetString(request.Value);
                requestValue = requestValue.Remove(requestValue.Length - 1, 1) + ",";

                var obj = new
                {
                    data = content,
                    success = externalApiResult.Success
                };

                var serializedObj = JsonConvert.SerializeObject(obj);

                requestValue += "\"" + objectName + "\":" + serializedObj + "}";
                request.Value = encoding.GetBytes(requestValue);
                _unitOfWork.Request.AddOrUpdate(request);
                _unitOfWork.Complete();
            }
            catch (Exception e)
            {
                var exceptions = new CustomExceptionHandler();
                exceptions.HandleException(e);
                exceptions.HandleException(e, "Error Occured on auto accepting serviceTask. \n" +
                                              $"FlowId: {item.Flow.Id}, RequestId: {item.Flow.RequestId}\n" +
                                              $"Error Message: {e.Message}\n" +
                                              $"Inner Exception: {e.InnerException?.Message}");
            }
        }

        if (newQuery.Count > 0)
        {
            // check if any serviceTask exists after this level 
            AutoAcceptServiceTaskFlow(requestId,webRootPath);
        }
    }

    public void CheckWorkFLowBoundaryForSchedule()
    {
        var s2W = _unitOfWork.LookUps.GetByTypeAndCode("WorkTime", 1);
        var thr = _unitOfWork.LookUps.GetByTypeAndCode("WorkTime", 2);
        //  var holyday = DbContext.Holydays.ToList();
        var holyday = _unitOfWork.Holydays.GetHolidays();
        var localOffset = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time").BaseUtcOffset;
        var datetime = DateTime.UtcNow.Add(localOffset);
        var query = _unitOfWork.Flows.GetFlowAndWorkFlowDetailAndBoudary();

        foreach (var item in query)
        {
            try
            {
                var reqDate = DateTime.ParseExact(item.Flow.DelayDate + item.Flow.DelayTime, "yyyyMMddHHmm", null, DateTimeStyles.None);
                var diff = _unitOfWork.Flows.CalculateDelay(reqDate, datetime, s2W, thr, holyday);
                var waithingTImeForAct = JObject.Parse(item.WorkFlowBoundary.Info).GetValue("WaitingTimeForAct").ToString();
                var minute = int.Parse(waithingTImeForAct);
                if (!(diff >= minute)) continue;
                var act = JObject.Parse(item.WorkFlowBoundary.Info).GetValue("Act").ToString();

                var encoding = new UnicodeEncoding();
                var value = encoding.GetString(item.Flow.Request.Value);
                var param = new FlowParam
                {
                    StaffId = item.Flow.Request.StaffId,
                    OrganizationPostTitleId = item.Flow.Request.OrganizationPostTitleId,
                    RequestTypeId = item.Flow.Request.Workflow.RequestTypeId,
                    CurrentStep = item.Flow.WorkFlowDetailId,
                    ConfirmStaffId = item.Flow.StaffId,
                    CurrentFlowId = item.Flow.Id,
                    RequestId = item.Flow.RequestId,
                    Evt = act,
                    Work = value,
                    BoundaryName = item.WorkFlowBoundary.BoundaryId,
                    ApiStaffId = item.Flow.StaffId
                };
                var nextflowstaffresult = GetNextFlowStaff(param);
                param.Work = value;
                if (nextflowstaffresult.End == "Terminate") param.IsEnd = true;
                var flowstatusId = act == "A"
                    ? _unitOfWork.LookUps.GetByTypeAndCode("FlowStatus", 2).Id
                    : _unitOfWork.LookUps.GetByTypeAndCode("FlowStatus", 3).Id;
                var currentFlow = _unitOfWork.Flows.ChangeFlowStatus(param, flowstatusId);
                AcceptRejectSimilarCode(currentFlow.WorkFlowDetail, param, nextflowstaffresult);
                _unitOfWork.Complete();
            }
            catch (Exception e)
            {
                var exceptions = new CustomExceptionHandler();
                exceptions.HandleException(e, $"Id: {item.WorkFlowBoundary.Id}, BoundaryId: {item.WorkFlowBoundary.BoundaryId}");
            }
        }
    }

    public void CheckWorkFLowNonInterruptingBoundaryForSchedule(string webRootPath)
    {
        var s2W = _unitOfWork.LookUps.GetByTypeAndCode("WorkTime", 1);
        var thr = _unitOfWork.LookUps.GetByTypeAndCode("WorkTime", 2);
        //  var holiday = DbContext.Holydays.ToList();
        var holiday = _unitOfWork.Holydays.GetHolidays();
        var localOffset = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time").BaseUtcOffset;
        var datetime = DateTime.UtcNow.Add(localOffset);

        var query = _unitOfWork.Flows.GetByContainNonInterruptingBoundaryTimerEvent();

        foreach (var item in query)
        {
            if (item.Flow.RequestId != Guid.Parse("C7D1829D-CADD-4659-BBBF-D26E1136CD7C"))
            {
                continue;
            }
            try
            {
                var reqDate = DateTime.ParseExact(item.Flow.DelayDate + item.Flow.DelayTime, "yyyyMMddHHmm", null, DateTimeStyles.None);
                var diff = _unitOfWork.Flows.CalculateDelay(reqDate, datetime, s2W, thr, holiday);
                var waithingTImeForAct = JObject.Parse(item.WorkFlowBoundary.Info).GetValue("WaitingTimeForAct").ToString();
                var minute = int.Parse(waithingTImeForAct);
                if (!(diff >= minute)) continue;

                var encoding = new UnicodeEncoding();
                var value = encoding.GetString(item.Flow.Request.Value);

                var currentWorkFlow = _unitOfWork.Workflows.Single(c => c.Id == item.Flow.Request.WorkFlowId);

                var param = new FlowParam
                {
                    StaffId = item.Flow.Request.StaffId,
                    OrganizationPostTitleId = item.Flow.Request.OrganizationPostTitleId,
                    RequestTypeId = currentWorkFlow.RequestTypeId,
                    CurrentStep = item.Flow.WorkFlowDetailId,
                    ConfirmStaffId = item.Flow.StaffId,
                    CurrentFlowId = item.Flow.Id,
                    RequestId = item.Flow.RequestId,
                    Work = value,
                    BoundaryName = item.WorkFlowBoundary.BoundaryId,
                    ApiStaffId = item.Flow.StaffId
                };
                var nextFlowStaffResult = GetNextFlowStaff(param);
                param.Work = value;
                if (nextFlowStaffResult.End == "Terminate") param.IsEnd = true;
                var flowstatusId = _unitOfWork.LookUps.GetByTypeAndCode("FlowStatus", 1).Id;
                var currentFlow = _unitOfWork.Flows.ChangeFlowStatus(param, flowstatusId);
                var cheakFlow = _unitOfWork.Flows.Where(t => t.RequestId == param.RequestId).ToList();

                foreach (var flow in nextFlowStaffResult.FlowStaves)
                {
                    var hasFlow = cheakFlow.Any(t => t.WorkFlowDetailId == flow.WorkFlowDetailId);
                    if (hasFlow != true)
                    {
                        AcceptRejectSimilarCode(currentFlow.WorkFlowDetail, param, nextFlowStaffResult);
                        _unitOfWork.Complete();
                    }
                }

                AutoAcceptScriptTaskFlow(param.RequestId);
                AutoAcceptServiceTaskFlow(param.RequestId,webRootPath);
                AutoAcceptOrRejectManualTaskFlow(param.RequestId);
            }
            catch (Exception e)
            {
                var exceptions = new CustomExceptionHandler();
                exceptions.HandleException(e, $"Id: {item.WorkFlowBoundary.Id}, BoundaryId: {item.WorkFlowBoundary.BoundaryId}");
            }
        }
    }

    public SelectAcceptorViewModel CreateWork(FlowParam param)
    {
        selectAcceptor = new SelectAcceptorViewModel();
        param.Evt = "A";
        var bytes = GetBytesOfWork(param);

        var request = new Request()
        {
            Id = param.RequestId,
            OrganizationPostTitleId = param.OrganizationPostTitleId,
            RegisterDate = int.Parse(DateTime.Now.ToString("yyyyMMdd")),
            RegisterTime = DateTime.Now.ToString("HHmm"),
            RequestStatusId = _unitOfWork.LookUps.Single(c => c.Code == 1 && c.Type == "RequestStatus").Id,
            StaffId = param.StaffId,
            WorkFlowId = _unitOfWork.Workflows.Single(p => p.RequestTypeId == param.RequestTypeId && p.IsActive).Id,
            Value = bytes,
        };

        var nextflowstaffresult = GetNextFlowStaff(param);
        if (nextflowstaffresult.IsSelectAcceptor)
        {
            selectAcceptor.IsSelectAcceptor = true;
            return selectAcceptor;
        }

        var flowResult = new List<Flow>();

        if (nextflowstaffresult.FlowStaves == null)
        {
            return selectAcceptor;
        }

        foreach (var flowStaff in nextflowstaffresult.FlowStaves)
        {
            var flows = FlowStaffToFlow(flowStaff, null, param.CurrentStep);
            foreach (var flow in flows)
            {
                if (flowResult.Any(c =>
                        c.RequestId == flow.RequestId && c.StaffId == flow.StaffId &&
                        c.WorkFlowDetailId == flow.WorkFlowDetailId) == false)
                {
                    flowResult.Add(flow);
                }
            }
            //flowResult.AddRange(flow);
        }

        //if (flowResult.Any())
        //{
        request.Flows = flowResult;
        _unitOfWork.Request.Add(request);
        //}

        MainHub.UpdateDashboardCharts();
        selectAcceptor.Request = request;
        selectAcceptor.FlowId = _unitOfWork.Flows.NextFlowIdIfAcepptorExist(param.StaffId);
        _unitOfWork.Complete();
        return selectAcceptor;
        //ارسال نوتیفیکیشن
    }

    public MainFlowStaff GetNextFlowStaff(FlowParam param)
    {
        var list = new List<FlowStaff>();
        var Gateways = new List<string>();



        string evt = string.Empty;

        var wfd =
            (from wd in _unitOfWork.WorkflowDetails.GetAsQueryable()
             join wf in _unitOfWork.Flows.GetWorkFlows() on wd.WorkFlowId equals wf.Id
             where /*wf.RequestTypeId == param.RequestTypeId &&*/ wd.Id == param.CurrentStep
             select wd).Single();
        var wfns = new List<WorkFlowNextStep>();

        if (param.BoundaryName != null)
        {
            var boundaryid = param.BoundaryName.Split('_')[1];
            wfns = _unitOfWork.Flows.GetWorkFlowNextSteps()
                .Include(p => p.NextStepToWfd)
                .Where(p => param.Path == null || p.Path == param.Path)
                .Where(w => w.FromWfdId == wfd.Id && w.BoundaryName.Contains(boundaryid))
                .ToList();
        }
        else
        {
            wfns = _unitOfWork.Flows.GetWorkFlowNextSteps()
                .Include(p => p.NextStepToWfd)
                .Where(p => param.Path == null || p.Path == param.Path)
                .Where(w => w.FromWfdId == wfd.Id && w.BoundaryName == null)
                .ToList();
        }

        if (param.BsNextStep != null)
        {
            var mystep = wfns.Single(w => w.NextStepToWfd.Step == param.BsNextStep.Value);
            wfns.Clear();
            wfns.Add(mystep);
        }
        var org = _unitOfWork.OrganizationInfos.GetOrgByStaffIdAndOrgPostId(param.StaffId,
            param.OrganizationPostTitleId);
        if (org == null)
        {
            throw new ArgumentException("رکورد فعال برای پست سازمانی انتخاب شده وجود ندارد");
        }
        var flowNode = GetChartsParentNode(org.ChartId);



        var trueFlowLine = new List<Tuple<string, string>>();
        var falseFlowLine = new List<Tuple<string, string>>();
        var gateway = string.Empty;
        var flowline = string.Empty;
        foreach (var workFlowNextStep in wfns.OrderBy(p => p.FlowLine))
        {
            //     var emailSmsList = new List<EsbViewModel>();
            //     var sendRemoteId = new List<SignalViewModel>();
            var workflowEsbs = new List<WorkflowEsb>();


            var flag = false;

            //var isParallel = false;

            var evts = workFlowNextStep.Evt.Split('@');
            var myevt = evts[0];
            if (myevt == "R" && param.Evt == "A")
            {
                throw new ArgumentException("مرحله بعد پیدا نشد.");
            }
            if (myevt == "A" && param.Evt == "R")
            {
                return new MainFlowStaff()
                {
                    FlowStaves = null,
                    Evt = myevt,
                    Gateways = null
                };
            }



            if (workFlowNextStep.Gateway != null)
            {


                var exps = workFlowNextStep.Exp.Split('@');
                var getWays = workFlowNextStep.Gateway.Split('@');
                var flowlines = workFlowNextStep.FlowLine.Split('@');
                var methods = workFlowNextStep.Method.Split('@');


                for (int i = 0; i < getWays.Length; i++)
                {

                    var exp = exps[i + 1];
                    var method = methods[i + 1];
                    gateway = getWays[i];
                    flowline = flowlines[i + 1];

                    myevt = evts[i + 1];

                    if (myevt != param.Evt)
                    {
                        falseFlowLine.Add(new Tuple<string, string>(item1: gateway, item2: flowline));
                        flag = true;
                        break;
                    }



                    if (falseFlowLine.Any(d => d.Item1 == gateway && d.Item2 == flowline))
                    {
                        flag = true;
                        break;
                    }

                    if (gateway.ToLower().Contains("exclusive") && trueFlowLine.Any())
                    {
                        if (trueFlowLine.Any(d => d.Item1 == gateway && d.Item2 != flowline))
                        {
                            flag = true;
                            break;
                        }

                    }
                    if (exp == "null" || exp == "")
                    {
                        if (gateway.ToLower().Contains("exclusive") && !trueFlowLine.Any(p => p.Item1 == gateway && p.Item2 == flowline))
                        {
                            trueFlowLine.Add(new Tuple<string, string>(item1: gateway, item2: flowline));
                        }
                        continue;
                    }
                    if (!gateway.ToLower().Contains("parallel"))
                    {

                        var expression = HelperBs.ConvertArraysToCommaDelimited(exp);
                        if (method != "null" && !string.IsNullOrWhiteSpace(method))
                        {
                            var work = param.Work.Replace(@"""", "'");
                            expression = expression.Replace("work", work);
                        }
                        var rule = JsonConvert.DeserializeObject<SystemRuleDto>(expression);
                        Type typeWork = param.Work.GetType();
                        if (typeWork.Name.ToLower() != "string")
                        {
                            param.Work = param.Work.ToString();
                        }

                        var myClass = ""; // CompileRuntime.RoslynGenerateClass(param.Work, method);

                        // todo: uncomment later 
                        // if (!CompileRuntime.BpmsRuleEngine(myClass, rule))
                        // {
                            if ((gateway.ToLower().Contains("exclusive") ||
                                 gateway.ToLower().Contains("inclusive")) &&
                                !falseFlowLine.Any(p => p.Item1 == gateway && p.Item2 == flowline))
                            {
                                falseFlowLine.Add(new Tuple<string, string>(item1: gateway, item2: flowline));
                            }
                            flag = true;
                            break;
                        // }
                    }


                    if (gateway.ToLower().Contains("exclusive") && !trueFlowLine.Any(p => p.Item1 == gateway && p.Item2 == flowline))
                    {
                        trueFlowLine.Add(new Tuple<string, string>(item1: gateway, item2: flowline));
                    }

                }
            }


            if (flag)
                continue;
            if (workFlowNextStep.Esb != null)
            {
                var esbs = workFlowNextStep.Esb.Split('@');
                foreach (var esb in esbs)
                {
                    var workflowesb =
                        _unitOfWork.WorkflowEsb.GetWorkflowEsbByWorkFlowNextStepIdAndEventId(workFlowNextStep.Id,
                            esb);
                    workflowEsbs.Add(workflowesb);


                }
            }
            if (workFlowNextStep.Gateway != null)
                Gateways.AddRange(workFlowNextStep.Gateway.Split('@'));
            evt = myevt;

            if (workFlowNextStep.NextStepToWfd.Step == int.MaxValue)
            {
                return new MainFlowStaff()
                {
                    FlowStaves = null,
                    Evt = evt,
                    Gateways = Gateways,
                    End = "Terminate",
                    WorkflowEsbs = workflowEsbs

                };

            }
            if (workFlowNextStep.NextStepToWfd.Step == int.MinValue)
            {
                return new MainFlowStaff()
                {
                    FlowStaves = null,
                    Evt = evt,
                    Gateways = Gateways,
                    End = "End",
                    WorkflowEsbs = workflowEsbs
                };

            }

            var flowStaff = new FlowStaff
            {
                WorkFlowDetailId = workFlowNextStep.NextStepToWfd.Id,
                RequestId = param.RequestId,
                // SubProcessIsActive = activeFlows,
                Gateway = gateway.Contains("eventBasedGateway") ? gateway : null,
                WorkflowEsbs = workflowEsbs,
                Param = param
            };

            var nextWorkFlowDetail = workFlowNextStep.NextStepToWfd;
            if (nextWorkFlowDetail.CallProcessId != null)
            {
                //  var workflow = DbContext.Workflows.Find(workFlowNextStep.NextStepToWfd.CallProcessId);
                var workflow = _unitOfWork.Workflows.FindById(workFlowNextStep.NextStepToWfd.CallProcessId);
                var reqTypeId = workflow.RequestTypeId;
                var workFlowDetail = _unitOfWork.WorkflowDetails.Single(w => w.Step == 0 && w.WorkFlowId == workflow.Id);

                var newParam = new FlowParam
                {
                    RequestTypeId = reqTypeId,
                    Work = param.Work,
                    StaffId = param.StaffId,
                    RequestId = Guid.NewGuid(),
                    OrganizationPostTitleId = param.OrganizationPostTitleId,
                    CurrentStep = workFlowDetail.Id
                };

                if (workFlowNextStep.NextStepToWfd.ExiteMethod == ExiteMethod.Standalone)
                {
                    CreateWork(newParam);
                    return GetNextFlowStaff(new FlowParam
                    {
                        StaffId = param.StaffId,
                        OrganizationPostTitleId = param.OrganizationPostTitleId,
                        RequestTypeId = param.RequestTypeId,
                        CurrentStep = workFlowNextStep.NextStepToWfd.Id,
                        ConfirmStaffId = param.ConfirmStaffId,
                        CurrentFlowId = param.CurrentFlowId,
                        RequestId = param.RequestId,
                        WorkFlowDetailId = workFlowNextStep.NextStepToWfd.Id,
                        Dsr = param.Dsr,
                        Work = param.Work
                    });
                }

                flowStaff.WorkFlowDetailId = workFlowDetail.Id;
                flowStaff.CallActivityId = nextWorkFlowDetail.Id;
                nextWorkFlowDetail = workFlowDetail;

                //newParam.RequestIdForCallActivity = param.RequestId;
                //newParam.CallActivityId = workFlowNextStep.NextStepToWfd.Id;
                //CreateWork(newParam);
                //list.Add(flowStaff);
                //return new MainFlowStaff()
                //{
                //    FlowStaves = list,
                //    Evt = evt,
                //    Gateways = Gateways
                //};

            }

            if (nextWorkFlowDetail.BusinessAcceptor)
            {
                if (param.BsNextStaffIds == null)
                {
                    //var value = CompileRuntime.GetStaffIds(param.Work);
                    // todo: uncomment later 
                    var value = new List<Guid>(); // CompileRuntime.RoslynGenerateBusinnesAcceptor(param, nextWorkFlowDetail.BusinessAcceptorMethod);
                    param.BsNextStaffIds = value ?? throw new ArgumentException("پاسخ دهندای انتخاب نشده است");
                }

                flowStaff.StaffIds = param.BsNextStaffIds;
                list.Add(flowStaff);

            }
            if (nextWorkFlowDetail.SelectAcceptor)
            {
                if (param.SelectedStaffIds == null)
                {
                    return new MainFlowStaff()
                    {
                        IsSelectAcceptor = true
                    };
                }

                flowStaff.StaffIds = param.SelectedStaffIds;
                list.Add(flowStaff);

            }
            if (nextWorkFlowDetail.RequesterAccept)
            {
                flowStaff.StaffIds = new List<Guid> { param.StaffId };
                list.Add(flowStaff);


            }
            if (nextWorkFlowDetail.StaffId != null)
            {
                flowStaff.StaffIds = new List<Guid> { nextWorkFlowDetail.StaffId.Value };
                list.Add(flowStaff);
            }
            if (nextWorkFlowDetail.ResponseGroupId != null)
            {
                var staffId = from staff in _unitOfWork.Flows.GetStaffs()
                              join assingnment in _unitOfWork.Flows.GetAssingnments() on staff.Id equals assingnment.StaffId
                              join groupResponse in _unitOfWork.Flows.GetLookUps() on assingnment.ResponseTypeGroupId equals groupResponse.Id
                              where groupResponse.Id == nextWorkFlowDetail.ResponseGroupId
                              select staff.Id;
                flowStaff.StaffIds = staffId;
                list.Add(flowStaff);


            }
            if (nextWorkFlowDetail.OrganizationPostTitleId != null)
            {

                var staffIds = _unitOfWork.OrganizationInfos.GetOrgInfos()
                    .Where(l => l.OrganiztionPostId == nextWorkFlowDetail.OrganizationPostTitleId
                                && l.Staff.EngType.Code == 1 && l.Staff.Users.First().IsActive)
                    .Select(s => s.StaffId).ToList();
                if (!staffIds.Any())
                {
                    throw new ArgumentException("با توجه به انتخاب عنوان پست سازمانی پرسنلی وجود ندارد");
                }

                flowStaff.StaffIds = staffIds;
                list.Add(flowStaff);

            }
            if (nextWorkFlowDetail.OrganizationPostTypeId != null)
            {
                var orgs = flowNode.Select(item => _unitOfWork.Flows.GetOrganiztionInfos()
                        .Where(l => l.ChartId == item.Id && l.IsActive)
                        .Where(p => p.OrganiztionPost.Aux.ToUpper() ==
                                    nextWorkFlowDetail.OrganizationPostTypeId.ToString().ToUpper()
                                    && p.StaffId != param.StaffId
                                    && p.Staff.Users.FirstOrDefault().IsActive
                                    && p.Staff.EngType.Code == 1
                        ).ToList())
                    .Where(values => values.Any()).ToList();
                if (!orgs.Any())
                {
                    trueFlowLine.Remove(trueFlowLine.Single(t => t.Item1 == gateway && t.Item2 == flowline));
                    Gateways.Clear();
                }
                foreach (var values in orgs)
                {
                    flowStaff.StaffIds = values.Select(d => d.StaffId);
                    list.Add(flowStaff);
                }
            }
            if (nextWorkFlowDetail.WorkflowDetailPatternId != null)
            {
                var patternId = nextWorkFlowDetail.WorkflowDetailPatternId;
                var items = _unitOfWork.WorkFlowDetailPatternItem.Where(i => i.WorkflowDetailPatternId == patternId)
                    .OrderBy(i => i.Index);
                foreach (var patternItem in items)
                {
                    var firstSelected = false;

                    var orgs = flowNode.Select(item => _unitOfWork.Flows.GetOrganiztionInfos()
                            .Where(l => l.ChartId == item.Id && l.IsActive)
                            .Where(p => p.OrganiztionPost.Aux.ToUpper() ==
                                        patternItem.LookupOrganizationPostId.ToString().ToUpper()
                                        && p.StaffId != param.StaffId
                                        && p.Staff.Users.FirstOrDefault().IsActive
                                        && p.Staff.EngType.Code == 1
                                        && p.Staff.EngType.Type == "EngType"
                            ).ToList())
                        .Where(values => values.Any()).ToList();

                    foreach (var values in orgs)
                    {

                        var staffIds = values.Select(d => d.StaffId);
                        foreach (var staffId in staffIds)
                        {
                            var newFlowStaff = new FlowStaff()
                            {
                                WorkFlowDetailId = flowStaff.WorkFlowDetailId,
                                CallActivityId = flowStaff.CallActivityId,
                                Gateway = flowStaff.Gateway,
                                MessageList = flowStaff.MessageList,
                                Param = flowStaff.Param,
                                RequestId = flowStaff.RequestId,
                                SendRemoteId = flowStaff.SendRemoteId,
                                StaffIds = new List<Guid>() { staffId },
                                WorkflowEsbs = flowStaff.WorkflowEsbs
                            };
                            list.Add(newFlowStaff);

                        }

                        if (nextWorkFlowDetail.SelectFirstPostPattern && staffIds != null)
                        {
                            firstSelected = true;
                        }
                    }

                    if (firstSelected)
                    {
                        break;
                    }

                }
                if (!list.Any())
                {
                    throw new ArgumentException("با توجه به انتخاب عنوان الگوها، پرسنلی وجود ندارد");
                }
            }

        }
        if (list.Any())
        {

            return new MainFlowStaff()
            {
                FlowStaves = list,
                Evt = evt,
                Gateways = Gateways
            };

        }
        throw new ArgumentException("هیچ رکوردی برای تعیین مرحله بعد وجود ندارد");
    }


    public SelectAcceptorViewModel ApiCreateWork(CreateworkDto model, string username)
    {
        var user = _unitOfWork.Users.Single(c => c.UserName == username);
        var staffId = user.StaffId;
        var staff = _unitOfWork.Staffs.GetStaffById(staffId);

        var organizationInfo = _unitOfWork.OrganizationInfos.GetOrgInfoByStaffId(staff.Id);
        if (organizationInfo == null)
        {
            throw new ArgumentException("پست سازمانی اصلی برای این نام کاربری موجود نمی باشد.");
        }
        var workFlow = _unitOfWork.Workflows.GetActiveWorkflowByRemoteId(model.RemoteId);
        if (workFlow == null)
        {
            throw new ArgumentException("فرآیند وجود ندارد.");
        }

        var workFlowDetail = _unitOfWork.WorkflowDetails.Single(p => p.Step == 0 && p.WorkFlowId == workFlow.Id);
        var param = new FlowParam()
        {
            Work = model.Content,
            StaffId = staffId,
            RequestTypeId = workFlow.RequestTypeId,
            RequestId = Guid.NewGuid(),
            OrganizationPostTitleId = organizationInfo.OrganiztionPostId,
            CurrentStep = workFlowDetail.Id
        };

        var req = CreateWork(param);
        return req;
    }


    public void SendSmsAndEmail(EsbViewModel esb, dynamic paramWork)
    {
        string[] paramWorkArray = { };
        string[] smsTextWorkArray = { };
        var workString = (string)paramWork.ToString();


        // Remote Id
        if (esb.SendRemoteId != null)
        {
            var remoteId = esb.SendRemoteId;
            var dynamicGetCode = esb.DynamicGetCode;
            var workflows = string.IsNullOrWhiteSpace(dynamicGetCode) ? _unitOfWork.Workflows.Where(d => d.RemoteId == remoteId) :
                _unitOfWork.Workflows.Where(d => d.RemoteId == remoteId && d.CodeId == dynamicGetCode);
            foreach (var workflow in workflows.ToList())
            {
                var workFlowDetail = _unitOfWork.WorkflowDetails.Single(p => p.Step == 0 && p.WorkFlowId == workflow.Id);
                CreateWork(new FlowParam()
                {
                    RequestTypeId = workflow.RequestTypeId,
                    Work = esb.Param.Work,
                    StaffId = esb.Param.StaffId,
                    RequestId = Guid.NewGuid(),
                    OrganizationPostTitleId = esb.Param.OrganizationPostTitleId,
                    CurrentStep = workFlowDetail.Id
                });

            }
            var acceptFlowEvent = new List<FlowEvent>();
            var flowsEvents = _unitOfWork.FlowEvents.GetFlowEventsContainsintermediateCatchEventMessage();
            foreach (var flowsEvent in flowsEvents)
            {
                var value = JObject.Parse(flowsEvent.Value);
                if (value["RemoteId"].ToString() != remoteId) continue;
                if (string.IsNullOrWhiteSpace(dynamicGetCode))
                    acceptFlowEvent.Add(flowsEvent);
                else
                {
                    var work = JObject.Parse(esb.Param.Work);
                    if (value["CodeId"].ToString() == work[dynamicGetCode].ToString())
                        acceptFlowEvent.Add(flowsEvent);
                }
            }
            foreach (var flowsEvent in acceptFlowEvent)
            {
                var flow = _unitOfWork.Flows.Find(i => i.Id == flowsEvent.FlowId).FirstOrDefault();
                var flag = true;
                var events = _unitOfWork.FlowEvents.GetEvents(flow.Id);
                // چک کردن فلو ها که به ترتیب فعال شده باشند
                for (var i = 0; i < flowsEvent.Order; i++)
                {
                    if (events[i].IsActive == false) flag = false;
                }

                if (flag)
                {
                    flowsEvent.IsActive = true;
                    _unitOfWork.Flows.CheckEventGateWay(flowsEvent);
                    NextEvent(flowsEvent, flowsEvents, new FlowParam()
                    {
                        StaffId = esb.Param.StaffId,
                        Work = esb.Param.Work,
                        OrganizationPostTitleId = esb.Param.OrganizationPostTitleId,
                        RequestId = esb.Param.RequestId
                    });
                }

                if (events.All(d => d.IsActive))
                {
                    flow.IsActive = true;
                    flow.DelayDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                    flow.DelayTime = DateTime.Now.ToString("HHmm");
                }
            }
        }
        var phoneNumbers = new List<string>();

        // Send Sms
        if (esb.SmsRequester)
        {
            var req = _unitOfWork.Request.FindById(esb.Param.RequestId);
            if (req != null) phoneNumbers.Add(req.Staff.PhoneNumber);
        }

        if (esb.SmsRecieve != null)
        {
            var smsJson = JsonConvert.DeserializeObject<WorkflowEsbViewModel>(esb.SmsRecieve);

            // get user phone number from passed steps form 
            if (smsJson.FromForm != null)
            {

                while (workString.Contains("["))
                {
                    int firstIndex = workString.IndexOf("[", StringComparison.Ordinal);
                    int lastIndex = workString.IndexOf("]", StringComparison.Ordinal);
                    workString = workString.Remove(firstIndex, lastIndex - firstIndex + 1);
                    workString = workString.Insert(firstIndex, "\"\"");
                }

                var workArray = workString.Split(',');

                paramWorkArray = workArray;

                var fieldId = smsJson.FromForm.FieldId;
                var selectedIndex = workArray.FirstOrDefault(c => c.Contains(fieldId));
                if (selectedIndex != null)
                {
                    string value = selectedIndex.Split(':')[1].Replace("\"", "").Replace("{", "").Replace("}", "");
                    bool isMobile = Regex.IsMatch(value, @"(?:0|\+98|98|0098)?(9\d{9})");
                    if (isMobile)
                    {
                        phoneNumbers.Add(value);
                    }
                    else
                    {
                        if (Guid.TryParse(value, out var staffId))
                        {
                            var staff = _unitOfWork.Staffs.SingleOrDefault(s => s.Id == staffId);
                            if (staff != null)
                            {
                                phoneNumbers.Add(staff.PhoneNumber);
                            }
                        }
                    }
                }
            }

            foreach (var staffId in smsJson.Staffs)
            {
                phoneNumbers.Add(_unitOfWork.Staffs.Single(s => s.Id == staffId).PhoneNumber);
            }

            foreach (var chartId in smsJson.Charts)
            {
                var query = _unitOfWork.Charts.GetPhoneNumbersByChartId(chartId);
                phoneNumbers.AddRange(query.ToList());
            }

            foreach (var other in smsJson.Others)
            {
                phoneNumbers.Add(other.PhoneNumber);
            }
            foreach (var clientId in smsJson.Clients)
            {
                phoneNumbers.Add(_unitOfWork.Client.Single(s => s.Id == clientId).CellPhone);
            }
        }

        if (phoneNumbers.Any())
        {
            esb.SmsText = esb.SmsText ?? "";
            smsTextWorkArray = workString.Split(',');
            while (esb.SmsText.Contains("["))
            {
                int firstIndex = esb.SmsText.IndexOf("[", StringComparison.Ordinal);
                int lastIndex = esb.SmsText.IndexOf("]", StringComparison.Ordinal);
                var variableFieldString = esb.SmsText.Substring(firstIndex, lastIndex - firstIndex);
                var variableFieldId = variableFieldString.Replace("[", "").Replace("]", "");
                var selectedIndex = smsTextWorkArray.FirstOrDefault(c => c.Contains(variableFieldId));
                if (selectedIndex != null)
                {
                    string value = selectedIndex.Split(':')[1].Replace("\"", "").Replace("{", "").Replace("}", "");
                    if (Guid.TryParse(value, out var staffId))
                    {
                        var staff = _unitOfWork.Staffs.SingleOrDefault(s => s.Id == staffId);
                        if (staff != null)
                        {
                            value = staff.FName + " " + staff.LName;
                        }
                    }
                    esb.SmsText = esb.SmsText.Replace($"[{variableFieldId}]", value);
                }
            }


            _sendingSmsService.SendSms(phoneNumbers.Distinct().ToList(), esb.SmsText);
        }
        // Send Email
        var emails = new List<string>();
        if (esb.EmailRequester)
        {
            var req = _unitOfWork.Request.FindById(esb.Param.RequestId);
            if (req != null) emails.Add(req.Staff.Email);
        }
        if (esb.EmailRecieve != null) /*continue;*/
        {
            var emailJson = JsonConvert.DeserializeObject<WorkflowEsbViewModel>(esb.EmailRecieve);

            foreach (var staffId in emailJson.Staffs)
            {
                emails.Add(_unitOfWork.Staffs.Single(s => s.Id == staffId).Email);
            }

            foreach (var chartId in emailJson.Charts)
            {
                var query = _unitOfWork.Charts.GetEmailsByChartId(chartId);
                emails.AddRange(query.ToList());
            }

            foreach (var other in emailJson.Others)
            {
                emails.Add(other.Email);
            }
            foreach (var clientId in emailJson.Clients)
            {
                emails.Add(_unitOfWork.Client.Single(s => s.Id == clientId).Email);
            }
            foreach (var companyId in emailJson.Companies)
            {
                emails.Add(_unitOfWork.Company.Single(s => s.Id == companyId).Email);
            }
        }

        if (emails.Any())
        {
            // todo: uncomment when u done moving this method to a service 
            //new EmailSender().Send(emails.Distinct().ToList(), new MessageContent() { Subject = esb.EmailSubject, Body = esb.EmailBody });
        }
    }

    public void SendSignalMessage(SignalViewModel item)
    {
        //  foreach (var item in model)
        // {
        if (item.SendRemoteId != null)
        {
            var remoteId = item.SendRemoteId;
            var workflows = _unitOfWork.Workflows.Where(d => d.RemoteId == remoteId);
            foreach (var workflow in workflows.ToList())
            {
                var workFlowDetail = _unitOfWork.WorkflowDetails.Single(p => p.Step == 0 && p.WorkFlowId == workflow.Id);
                CreateWork(new FlowParam()
                {
                    RequestTypeId = workflow.RequestTypeId,
                    Work = item.Param.Work,
                    StaffId = item.Param.StaffId,
                    RequestId = Guid.NewGuid(),
                    OrganizationPostTitleId = item.Param.OrganizationPostTitleId,
                    CurrentStep = workFlowDetail.Id
                });

            }

            var flowsEvents = _unitOfWork.FlowEvents.GetFlowEventsContainsintermediateCatchEventSignal(remoteId);
            foreach (var flowsEvent in flowsEvents)
            {
                var flow = _unitOfWork.Flows.Find(i => i.Id == flowsEvent.FlowId).FirstOrDefault();
                var flag = true;
                var events = _unitOfWork.FlowEvents.GetEvents(flow.Id);
                // چک کردن فلو ها که به ترتیب فعال شده باشند
                for (var i = 0; i < flowsEvent.Order; i++)
                {
                    if (events[i].IsActive == false) flag = false;
                }

                if (flag)
                {
                    flowsEvent.IsActive = true;
                    _unitOfWork.Flows.CheckEventGateWay(flowsEvent);
                    NextEvent(flowsEvent, flowsEvents, new FlowParam()
                    {
                        StaffId = item.Param.StaffId,
                        Work = item.Param.Work,
                        OrganizationPostTitleId = item.Param.OrganizationPostTitleId
                    });
                }

                if (events.All(d => d.IsActive))
                {
                    flow.IsActive = true;
                    flow.DelayDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                    flow.DelayTime = DateTime.Now.ToString("HHmm");
                }
            }
        }
        // }
    }

    public void NextEvent(FlowEvent flowsEvent, List<FlowEvent> events, FlowParam param)
    {
        for (var i = flowsEvent.Order + 1; i < events.Count; i++)
        {
            if (events[i].WorkflowEsb.EventId.Contains("intermediateThrowEventMessage"))
            {
                var esbjson = JsonConvert.DeserializeObject<EsbViewModel>(events[i].WorkflowEsb.Info);
                esbjson.Param = param;
                SendSmsAndEmail(esbjson, esbjson.Param.Work);
                events[i].IsActive = true;
                continue;
            };
            if (events[i].WorkflowEsb.EventId.Contains("intermediateThrowEventSignal"))
            {
                var obj = JObject.Parse(events[i].WorkflowEsb.Info);
                var signal = new SignalViewModel()
                {
                    SendRemoteId = obj.GetValue("SendRemoteId").ToString(),
                    Param = param
                };
                SendSignalMessage(signal);
                events[i].IsActive = true;
                continue;
            };
            if (events[i].WorkflowEsb.EventId.Contains("intermediateCatchEventTimer"))
            {
                var obj = JObject.Parse(events[i].WorkflowEsb.Info);
                var dynamicWaitingDate = obj.GetValue("DynamicWaitingDate").ToString();
                int waitingDate = 0;
                if (dynamicWaitingDate != string.Empty)
                {
                    string dwt = GetDynamicTime(dynamicWaitingDate, param.Work);
                    if (dwt != string.Empty)
                    {
                        bool parsed = int.TryParse(dwt, out int result);
                        if (parsed)
                            waitingDate = result;
                    }
                }
                if (waitingDate == 0)
                {
                    waitingDate = int.Parse(obj.GetValue("WaitingDate").ToString());
                }
                var date = DateTime.Now.AddDays(waitingDate).ToString("yyyyMMdd");
                events[i].Value = date;

            };
            break;
        }
    }

    public List<Flow> FlowStaffToFlow(FlowStaff flowStaff, Guid? previousFlowid = null, Guid? previousWfdId = null)
    {
        var flows = new List<Flow>();
        if (flowStaff == null)
        {
            return null;
        }

        var flowevent = new List<FlowEvent>();
        var eventIsActive = true;
        var order = 0;
        foreach (var workflowEsb in flowStaff.WorkflowEsbs)
        {
            if (workflowEsb == null) continue;
            if (workflowEsb.EventId.Contains("intermediateThrowEventMessage"))
            {

                var esbjson = JsonConvert.DeserializeObject<EsbViewModel>(workflowEsb.Info);
                esbjson.Param = new FlowParam()
                {
                    Work = flowStaff.Param.Work,
                    StaffId = flowStaff.Param.StaffId,
                    OrganizationPostTitleId = flowStaff.Param.OrganizationPostTitleId,
                    RequestId = flowStaff.RequestId
                };
                flowevent.Add(new FlowEvent()
                {
                    WorkFlowEsbId = workflowEsb.Id,
                    Value = workflowEsb.Info,
                    Order = order,
                    IsActive = eventIsActive,
                });
                if (eventIsActive)
                {

                    SendSmsAndEmail(esbjson, flowStaff.Param.Work);
                }
            }
            if (workflowEsb.EventId.Contains("intermediateThrowEventSignal"))
            {
                var obj = JObject.Parse(workflowEsb.Info);
                var signal = new SignalViewModel()
                {
                    SendRemoteId = obj.GetValue("SendRemoteId").ToString(),
                    Param = new FlowParam()
                    {
                        Work = flowStaff.Param.Work,
                        StaffId = flowStaff.Param.StaffId,
                        OrganizationPostTitleId = flowStaff.Param.OrganizationPostTitleId,
                    }
                };
                flowevent.Add(new FlowEvent()
                {
                    WorkFlowEsbId = workflowEsb.Id,
                    Value = workflowEsb.Info,
                    Order = order,
                    IsActive = eventIsActive,
                });
                if (eventIsActive)
                {
                    SendSignalMessage(signal);
                }
            }
            if (workflowEsb.EventId.Contains("intermediateCatchEventTimer"))
            {
                var obj = JObject.Parse(workflowEsb.Info);
                string date;
                var waitingDate = obj.GetValue("WaitingDate").ToString();
                if (!string.IsNullOrWhiteSpace(waitingDate) && waitingDate != "NaN")
                    date = DateTime.Now.AddDays(int.Parse(waitingDate)).ToString("yyyyMMdd");
                else
                {
                    var dynamicWaitingDate = obj.GetValue("DynamicWaitingDate").ToString();
                    var timerType = obj.GetValue("TimerType") == null ? "1" : obj.GetValue("TimerType").ToString();

                    var work = JObject.Parse(flowStaff.Param.Work);
                    var time = work.GetValue(dynamicWaitingDate).ToString();
                    date = timerType == "2" ? (string)time.Replace("/", "") : (string)DateTime.Now.AddDays(int.Parse(time)).ToString("yyyyMMdd");

                }
                flowevent.Add(new FlowEvent()
                {
                    WorkFlowEsbId = workflowEsb.Id,
                    Value = date,
                    Order = order,
                    IsActive = false,
                    GatewayEventBase = flowStaff.Gateway
                });
                eventIsActive = false;
            }
            if (workflowEsb.EventId.Contains("intermediateCatchEventMessage"))
            {
                //  var obj = JObject.Parse(workflowEsb.Info);
                // var remoteid = obj.GetValue("RemoteId").ToString();

                flowevent.Add(new FlowEvent()
                {
                    WorkFlowEsbId = workflowEsb.Id,
                    Value = workflowEsb.Info,
                    Order = order,
                    IsActive = false,
                    GatewayEventBase = flowStaff.Gateway
                });
                eventIsActive = false;
            }
            if (workflowEsb.EventId.Contains("intermediateCatchEventSignal"))
            {
                //var obj = JObject.Parse(workflowEsb.Info);
                //var remoteid = obj.GetValue("RemoteId").ToString();
                flowevent.Add(new FlowEvent()
                {
                    WorkFlowEsbId = workflowEsb.Id,
                    IsActive = false,
                    Value = workflowEsb.Info,
                    Order = order,
                    GatewayEventBase = flowStaff.Gateway
                });
                eventIsActive = false;
            }

            order++;
        }

        if (flowStaff.StaffIds != null)
        {
            foreach (var item in flowStaff.StaffIds.ToList())
            {
                int delayDate;
                string delayTime;
                Guid? calactivityId = flowStaff.CallActivityId;
                if (previousFlowid == null)
                {
                    delayDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                    delayTime = DateTime.Now.ToString("HHmm");
                }
                else
                {
                    var previousFlow = _unitOfWork.Flows.Single(s => s.Id == previousFlowid);
                    delayDate = previousFlow.ResponseDate.Value;
                    delayTime = previousFlow.ResponseTime;

                    var currentWfd = _unitOfWork.WorkflowDetails.Single(d => d.Id == flowStaff.WorkFlowDetailId);
                    var sefrStep = currentWfd.WorkFlow.WorkflowDetails.Single(d => d.Step == 0);

                    var firstFlow =
                        _unitOfWork.Flows.GetFlowByRequestIdAndWorkFlowDetailId(previousFlow.RequestId,
                            sefrStep.Id);
                    if (firstFlow != null)
                        calactivityId = firstFlow.CallActivityId;
                }
                var newFlow = new Flow()
                {
                    StaffId = item,
                    RequestId = flowStaff.RequestId,
                    WorkFlowDetailId = flowStaff.WorkFlowDetailId,
                    PreviousFlowId = previousFlowid,
                    FlowStatusId = _unitOfWork.LookUps.Single(c => c.Code == 1 && c.Type == "FlowStatus").Id,
                    IsActive = eventIsActive,
                    FlowEvents = flowevent,
                    DelayDate = delayDate,
                    DelayTime = delayTime,
                    CallActivityId = calactivityId,
                    PreviousWorkFlowDetailId = previousWfdId
                };

                flows.Add(newFlow);
            }
        }
        else
        {
            throw new ArgumentException("انتخاب پاسخ دهنده جهت ثبت درخواست الزامی میباشد.");
        }

        return flows;
    }
    public void CheckIntermediateTimerForSchedule()
    {
        var uniqud = new UnicodeEncoding();
        var flowsEvents = _unitOfWork.FlowEvents
            .Where(d =>
                d.WorkflowEsb.EventId.Contains(BpmnNodeConstant.Boundary.IntermediateCatchEventTimer) &&
                !d.IsActive &&
                d.Flow.LookUpFlowStatus.Code == 1)
            .ToList();

        foreach (var flowsEvent in flowsEvents)
        {
            var flow = _unitOfWork.Flows.Find(i => i.Id == flowsEvent.FlowId).FirstOrDefault();
            var flag = true;
            var events = _unitOfWork.FlowEvents.Where(d => d.FlowId == flow.Id).OrderBy(d => d.Order).ToList();
            // چک کردن فلو ها که به ترتیب فعال شده باشند
            for (var i = 0; i < flowsEvent.Order; i++)
            {
                if (events[i].IsActive == false) flag = false;
            }

            if (flag)
            {
                var now = DateTime.Now.ToString("yyyyMMdd");
                bool isTimeToAccept = flowsEvent.Value != null && int.Parse(now) >= int.Parse(flowsEvent.Value);

                var work = uniqud.GetString(flow.Request.Value);
                var wfesb = _unitOfWork.WorkflowEsb.SingleOrDefault(c => c.Id == flowsEvent.WorkFlowEsbId);
                if (wfesb != null)
                {
                    var selectedElement = JObject.Parse(wfesb.Info)["DynamicWaitingDate"].ToString();
                    if (string.IsNullOrEmpty(selectedElement) == false)
                    {
                        var selectedElementValue = JObject.Parse(work)[selectedElement].ToString();

                        if (string.IsNullOrEmpty(selectedElementValue) == false)
                        {
                            bool isDateValid = DateTime.TryParse(selectedElementValue, out var dynamicDate);
                            if (isDateValid && dynamicDate.Date <= DateTime.Now)
                            {
                                isTimeToAccept = true;
                            }
                        }
                    }
                }

                if (isTimeToAccept)
                {
                    flowsEvent.IsActive = true;
                    _unitOfWork.Flows.CheckEventGateWay(flowsEvent);
                    NextEvent(flowsEvent, events, new FlowParam()
                    {
                        StaffId = flow.Request.StaffId,
                        OrganizationPostTitleId = flow.Request.OrganizationPostTitleId,
                        Work = work
                    });
                }
            }

            if (events.All(d => d.IsActive))
            {
                if (flow != null)
                {
                    flow.IsActive = true;
                    flow.DelayDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                    flow.DelayTime = DateTime.Now.ToString("HHmm");
                }
            }

            _unitOfWork.Complete();
        }
    }
    public bool SaveWork(FlowParam param)
    {
        var bytes = GetBytesOfWork(param);
        var flow = _unitOfWork.Flows.Find(i => i.Id == param.CurrentFlowId).FirstOrDefault();
        flow.Value = bytes;
        var result = _unitOfWork.Complete();
        return result > 0;
    }
    public string GetDynamicTime(string dynamicWaitingDate, dynamic work)
    {
        var formData = JObject.Parse(Convert.ToString(work));
        string time = formData.GetValue(dynamicWaitingDate).ToString();
        return time;
    }
    private static byte[] GetBytesOfWork(FlowParam param)
    {
        byte[] bytes = null;
        if (param.Work.GetType().Name.ToLower() == "string")
        {
            var content = HttpUtility.UrlDecode(param.Work, Encoding.UTF8);
            var encoding = new UnicodeEncoding();
            bytes = encoding.GetBytes(content);
        }

        return bytes;
    }
    private List<Chart> GetChartsParentNode(Guid chartId)
    {
        var ch = new List<Chart>();
        _unitOfWork.Flows.FindParentFromChartId(chartId, ch);
        return ch;
    }

}