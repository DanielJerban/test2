using System.CodeDom.Compiler;
using BPMS.Domain.Common.Constants;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.Services;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using BPMS.Domain.Common.Dtos;

namespace BPMS.Application.Services;

public class ValidateWorkflowService : IValidateWorkflowService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly XNamespace _bpmn2 = "http://www.omg.org/spec/BPMN/20100524/MODEL";
    private readonly XNamespace _workflow = "http://magic";

    public ValidateWorkflowService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public XDocument ValidateModel(BpmnDiagramViewModel model)
    {
        var uniqXml = HttpUtility.UrlDecode(model.Workflow.XmlFile, Encoding.UTF8);
        var xmlDocument = XDocument.Parse(uniqXml);

        var start = ValidateStartNotation(xmlDocument);

        ValidateTimerStartEvent(start);

        ValidateEventBasedGateway(xmlDocument);

        ValidateStartOutgoing(start);

        ValidateSequenceFlow(xmlDocument, start, model.Workflow.SubProcessId);

        ValidateEndEvent(xmlDocument);

        ValidateLane(xmlDocument);

        ValidateGateways(xmlDocument);

        ValidateSequenceOnChange(xmlDocument);

        ValidateBoundaryEvent(xmlDocument);

        ValidateIntermediateCatchEvent(xmlDocument);

        var allNodes = GetAllNodes(xmlDocument);

        ValidateMinimumSteps(allNodes, model.Workflow.SubProcessId);

        ValidateWorkflowDetails(allNodes, model.Workflow.Id);

        return xmlDocument;
    }

    private List<XElement> GetAllNodes(XDocument xml)
    {
        var allTask = xml.Descendants(_bpmn2 + BpmnNodeConstant.Task);
        var allBusinessRuleTask = xml.Descendants(_bpmn2 + BpmnNodeConstant.BusinessRuleTask);
        var allSubProcess = xml.Descendants(_bpmn2 + BpmnNodeConstant.SubProcess);
        var adHocSubProcess = xml.Descendants(_bpmn2 + BpmnNodeConstant.AdHocSubProcess);
        var allCallActivities = xml.Descendants(_bpmn2 + BpmnNodeConstant.CallActivity);
        var allServiceTask = xml.Descendants(_bpmn2 + BpmnNodeConstant.ServiceTask);
        var allScriptTask = xml.Descendants(_bpmn2 + BpmnNodeConstant.ScriptTask);
        var allManualTask = xml.Descendants(_bpmn2 + BpmnNodeConstant.ManualTask);

        return allTask
            .Union(allSubProcess)
            .Union(allBusinessRuleTask)
            .Union(allCallActivities)
            .Union(adHocSubProcess)
            .Union(allServiceTask)
            .Union(allManualTask)
            .Union(allScriptTask)
            .ToList();
    }

    private XElement ValidateStartNotation(XDocument xml)
    {
        var start = xml.Descendants(_bpmn2 + "startEvent").ToList();

        if (!start.Any())
        {
            throw new ArgumentException(@"حتماً باید یک شروع در دیاگرام وجود داشته باشد.");
        }

        return start.First();
    }

    private void ValidateTimerStartEvent(XElement start)
    {
        var timerStartEvent = start.Elements(_bpmn2 + "timerEventDefinition").FirstOrDefault();
        if (timerStartEvent == null) return;

        var startDate = start.Attribute("StartDate")?.Value;
        if (startDate is null)
            throw new ArgumentException("تاریخ شروع را وارد کنید.");

        var startTime = start.Attribute("StartTime")?.Value;
        if (startTime is null)
            throw new ArgumentException("ساعت شروع را وارد کنید.");

        var isSequential = start.Attribute("Repeating")?.Value;
        if (isSequential is "true")
        {
            var waitingDate = start.Attribute("WaitingDate")?.Value;
            var waitingTime = start.Attribute("WaitingTime")?.Value;
            if (waitingDate is null && waitingTime is null)
                throw new ArgumentException("روز و ساعت تکرار نمیتواند خالی باشد.");
        }

        var hasExpireDate = start.Attribute("HasExpiry")?.Value;
        if (hasExpireDate is "true")
        {
            var endDate = start.Attribute("EndDate")?.Value;
            if (endDate is null)
                throw new ArgumentException("تاریخ پایان را وارد کنید.");
            var endTime = start.Attribute("EndTime")?.Value;
            if (endTime is null)
                throw new ArgumentException("ساعت پایان را وارد کنید.");
        }
    }

    private void ValidateEventBasedGateway(XDocument xml)
    {
        var ebg = xml.Descendants(_bpmn2 + "eventBasedGateway").ToList();
        if (ebg.Any())
        {
            throw new ArgumentException(@"در حال حاضر استفاده از دروازه مبتنی بر رویداد امکانپذیر نیست.");
        }
    }

    private void ValidateStartOutgoing(XElement start)
    {
        var outgoingStart = start.Elements(_bpmn2 + "outgoing").ToList();
        if (outgoingStart.Count > 1)
        {
            throw new ArgumentException(@"شروع نمی تواند دو خروجی داشته باشد.", start.Attribute("id")!.Value);
        }
    }

    private void ValidateSequenceFlow(XDocument xml, XElement start, Guid? subProcessId)
    {
        var seq = xml.Descendants(_bpmn2 + "sequenceFlow").ToList().FirstOrDefault(d => (string)d.Attribute("sourceRef") == (string)start.Attribute("id"));
        if (seq == null)
        {
            throw new ArgumentException(@"شروع به جایی وصل نشده است.", start.Attribute("id")!.Value);
        }

        var seqEvt = (string)seq.Attribute(_workflow + "Evt") ?? "A";
        if (seqEvt != "A")
            throw new ArgumentException(@"رویداد خروجی شروع باید بر روی OnAccept باشد.", seq.Attribute("id")!.Value);

        var startTarget = xml.Descendants().ToList()
            .First(d => (string)d.Attribute("id") == (string)seq.Attribute("targetRef"));

        if (startTarget.Name != _bpmn2 + "task" && subProcessId == null)
        {
            throw new ArgumentException(@"خروجی شروع حتما باید تسک باشد.", startTarget.Attribute("id")!.Value);
        }

        if (startTarget.Name != _bpmn2 + "task" && startTarget.Name != _bpmn2 + BpmnNodeConstant.ManualTask && subProcessId != null
            && startTarget.Name != _bpmn2 + "businessRuleTask" && startTarget.Name != _bpmn2 + BpmnNodeConstant.ScriptTask)
        {
            throw new ArgumentException(@"خروجی شروع حتما باید تسک معمولی یا تسک دستی یا اسکریپت تسک باشد.", startTarget.Attribute("id")!.Value);
        }

        if (IsSecondNodeBusinessRuleTask(seq, xml))
        {
            throw new ArgumentException(@"مرحله دوم نباید بیزینس تسک باشد.");
        }

        var incomingStartTarget = startTarget.Elements(_bpmn2 + "incoming").ToList();

        if (incomingStartTarget.Count > 1)
        {
            throw new ArgumentException(@"تسک ثبت درخواست نباید بتواند به جز شروع ورودی دیگری داشته باشد.", startTarget.Attribute("id")!.Value);
        }
    }

    private bool IsSecondNodeBusinessRuleTask(XElement startFlow, XDocument xml)
    {
        var secondFlow = startFlow.NextFlow(xml);

        var secondNode = secondFlow.TargetNode(xml);

        if (secondNode.NameContains("business"))
            return true;

        if (!secondNode.NameContains("gateway")) return false;

        var gatewayFlows = secondFlow.NextFlows(xml).ToList();

        foreach (var flow in gatewayFlows)
        {
            var flowTargetNode = flow.TargetNode(xml);
            if (flowTargetNode.NameContains("business"))
                return true;
        }
        return false;
    }

    private void ValidateEndEvent(XDocument xml)
    {
        var end = xml.Descendants(_bpmn2 + "endEvent").ToList();
        if (!end.Any())
        {
            throw new ArgumentException(@"حتماً باید یک پایان در دیاگرام وجود داشته باشد.");

        }

        foreach (var xElement in end)
        {
            var incomingEnd = xElement.Elements(_bpmn2 + "incoming");
            if (!incomingEnd.Any())
            {
                throw new ArgumentException(@"پایان به جایی متصل نشده است.", (string)xElement.Attribute("id"));
            }
        }
    }

    private void ValidateLane(XDocument xml)
    {
        var lanes = xml.Descendants(_bpmn2 + "lane");
        foreach (var xElement in lanes)
        {
            var name = (string)xElement.Attribute("name");
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(@"نام Lane وارد نشده است.", (string)xElement.Attribute("id"));
            }
        }
    }

    private void ValidateGateways(XDocument xml)
    {
        ValidateExclusiveGateway(xml);
        ValidateInclusiveGateway(xml);
        ValidateParallelGateway(xml);
    }

    private void ValidateExclusiveGateway(XDocument xml)
    {
        var exclusiveGateway = xml.Descendants(_bpmn2 + "exclusiveGateway");
        foreach (var xElement in exclusiveGateway)
        {
            var incoming = xElement.Elements(_bpmn2 + "incoming").ToList();
            var outgoing = xElement.Elements(_bpmn2 + "outgoing").ToList();
            if (!incoming.Any() || !outgoing.Any())
            {
                throw new ArgumentException(@"دروازه های انحصاری باید حداقل یک خروجی و یک ورودی داشته باشند.", (string)xElement.Attribute("id"));
            }

            var defaultFlow = (string)xElement.Attribute("default");
            if (outgoing.Count == 2 && string.IsNullOrWhiteSpace(defaultFlow))
            {
                throw new ArgumentException(@"دروازه های انحصاری باید یک خروجی از نوع پیشفرض داشته باشند.", (string)xElement.Attribute("id"));
            }
        }
    }

    private void ValidateInclusiveGateway(XDocument xml)
    {
        var inclusiveGateway = xml.Descendants(_bpmn2 + "inclusiveGateway");
        foreach (var xElement in inclusiveGateway)
        {
            var incoming = xElement.Elements(_bpmn2 + "incoming").ToList();
            var outgoing = xElement.Elements(_bpmn2 + "outgoing").ToList();
            if (!outgoing.Any() || !incoming.Any())
            {
                throw new ArgumentException(@"دروازه های جامع باید حداقل یک خروجی و یک ورودی داشته باشند.", (string)xElement.Attribute("id"));
            }
        }
    }

    private void ValidateParallelGateway(XDocument xml)
    {
        var parallelGateway = xml.Descendants(_bpmn2 + "parallelGateway");
        foreach (var xElement in parallelGateway)
        {
            var incoming = xElement.Elements(_bpmn2 + "incoming").ToList();
            var outgoing = xElement.Elements(_bpmn2 + "outgoing").ToList();
            if (!outgoing.Any() || !incoming.Any())
            {
                throw new ArgumentException(@"دروازه های موازی باید حداقل یک خروجی و یک ورودی داشته باشند.", (string)xElement.Attribute("id"));
            }

            //ولیدیشن های خط های خروجی دروازه موازی
            var sequences = xml.Descendants(_bpmn2 + "sequenceFlow").ToList()
                .Where(d => (string)d.Attribute("sourceRef") == (string)xElement.Attribute("id")).ToList();

            var firstEvtParallel = (string)sequences[0].Attribute(_workflow + "Evt") ?? "A";
            foreach (var el in sequences)
            {
                var evt = (string)el.Attribute(_workflow + "Evt") ?? "A";

                if (evt != firstEvtParallel)
                {
                    throw new ArgumentException(@"رویدادهای خروجی های دروازه های موازی باید با هم برابر باشند.", (string)xElement.Attribute("id"));
                }
            }
        }
    }

    private void ValidateSequenceOnChange(XDocument xml)
    {
        var seqChanges = xml.Descendants(_bpmn2 + "sequenceFlow").ToList().Where(d => (string)d.Attribute(_workflow + "Evt") == "C").ToList();
        foreach (var seqChange in seqChanges)
        {
            var sequenceSource = (string)seqChange.Attribute("sourceRef");
            var element = xml.Descendants().ToList().Single(d => (string)d.Attribute("id") == sequenceSource);
            if (!(element.Name.LocalName == "task" || element.Name.LocalName == "businessRuleTask" || element.Name.LocalName == "timerTask"))
            {
                throw new ArgumentException(@"فقط رویداد خروجی های تسک ها میتواند OnChange باشد.", (string)seqChange.Attribute("id"));
            }
        }
    }

    private void ValidateBoundaryEvent(XDocument xml)
    {
        var boundaryEvent = xml.Descendants(_bpmn2 + "boundaryEvent").ToList();
        foreach (var xElement in boundaryEvent)
        {
            var nodeWaitingTimeForAct = (string)xElement.Attribute(_workflow + "WaitingTimeForAct");
            var cancelActivity = (string)xElement.Attribute("cancelActivity");
            var receiveErrorId = (string)xElement.Attribute(_workflow + "RecriveErrorId");
            // var act = (string)xElement.Attribute(_workflow + "Act");
            var timerEventDefinition = xElement.Descendants(_bpmn2 + "timerEventDefinition").ToList();
            var errorEventDefinition = xElement.Descendants(_bpmn2 + "errorEventDefinition").ToList();
            var outgoing = xElement.Descendants(_bpmn2 + "outgoing").ToList();

            if (!outgoing.Any())
            {
                throw new ArgumentException(@"باندری می بایست خروجی داشته باشد. ", (string)xElement.Attribute("id"));

            }

            if (errorEventDefinition.Count != 0)
            {
                if ((receiveErrorId == null || receiveErrorId == "NaN"))
                {

                    throw new ArgumentException(@"کد خطاوارد نشده است.", (string)xElement.Attribute("id"));

                }
            }
            else if (timerEventDefinition.Count != 0)
            {
                if (nodeWaitingTimeForAct == null || nodeWaitingTimeForAct == "NaN")
                {
                    if (cancelActivity == "false")
                    {
                        throw new ArgumentException(@"مدت ساعت عملیات وارد نشده است.", (string)xElement.Attribute("id"));
                    }
                    throw new ArgumentException(@"مدت ساعت عملیات تایید و رد وارد نشده است.", (string)xElement.Attribute("id"));
                }

            }

        }
    }

    private void ValidateIntermediateCatchEvent(XDocument xml)
    {
        var intermediateCatchEvent = xml.Descendants(_bpmn2 + "intermediateCatchEvent").ToList();
        foreach (var xElement in intermediateCatchEvent)
        {
            var nodeWaitingDate = (string)xElement.Attribute(_workflow + "WaitingDate");

            ValidateDynamicTime(xElement);

            if (nodeWaitingDate is null or "NaN")
            {
                throw new ArgumentException(@"زمان انتظار ثابت وارد نشده است.", (string)xElement.Attribute("id"));
            }

            var outgoing = xElement.Descendants(_bpmn2 + "outgoing").ToList();
            if (outgoing.Count > 1)
            {
                throw new ArgumentException(@"رویداد میانی نباید بیش از یک خروجی داشته باشد.", (string)xElement.Attribute("id"));
            }
        }

        void ValidateDynamicTime(XElement xElement)
        {
            var nodeDynamicWaitingDate = (string)xElement.Attribute(_workflow + "DynamicWaitingDate");
            if (nodeDynamicWaitingDate != "NaN" && nodeDynamicWaitingDate != null)
            {
                var nodeDynamicTimerType = (string)xElement.Attribute(_workflow + "TimerType");

                var nodeFormId = (string)xElement.Attribute(_workflow + "FormIdForTimer");
                if (string.IsNullOrEmpty(nodeFormId))
                {
                    return;
                }
                var formId = Guid.Parse(nodeFormId);
                var form = _unitOfWork.WorkFlowForm.Where(c => c.Id == formId).SingleOrDefault();
                if (form == null)
                {
                    throw new ArgumentException(@"فرم انتخاب شده معتبر نمی باشد.", (string)xElement.Attribute("id"));
                }

                var encoding = new UnicodeEncoding();
                var content = encoding.GetString(form.Content);
                var fields = JObject.Parse(content)["fields"];
                if (fields == null)
                {
                    throw new ArgumentException(@"در فرم انتخاب شده المانی وجود ندارد.", (string)xElement.Attribute("id"));
                }

                foreach (var field in fields.ToList())
                {
                    var fieldElements = JObject.Parse(field.ToString());
                    var attrs = JObject.Parse(fieldElements["attrs"]!.ToString());
                    var meta = JObject.Parse(fieldElements["meta"]!.ToString());
                    var fieldId = attrs["id"]!.ToString();
                    var fieldTypeId = meta["id"]!.ToString();
                    if (fieldId == nodeDynamicWaitingDate)
                    {
                        if (nodeDynamicTimerType == "2" && fieldTypeId != "date-picker")
                        {
                            throw new ArgumentException(@"فیلد انتخاب شده برای زمان انتظار متغیر باید از نوع تاریخ باشد.", (string)xElement.Attribute("id"));
                        }

                        if (nodeDynamicTimerType != "2" && fieldTypeId != "number")
                        {
                            throw new ArgumentException(@"فیلد انتخاب شده برای زمان انتظار متغیر باید از نوع عددی باشد.", (string)xElement.Attribute("id"));
                        }
                    }
                }
            }
        }
    }

    private void ValidateMinimumSteps(List<XElement> allNodes, Guid? subProcessId)
    {
        if (allNodes.Count <= 1 && subProcessId == null)
        {
            throw new ArgumentException(@"برای ایجاد فرآیند حداقل یک مرحله مورد نیاز است.");
        }
    }

    #region WorkflowDetail Validators
    private void ValidateWorkflowDetails(List<XElement> allNodes, Guid workflowId)
    {
        foreach (var node in allNodes)
        {
            string title = ValidateWFDTitle(node);

            ValidateWFDNonRepetition(node, workflowId);

            ValidateAdHoc(node, title, out bool doContinue);
            if (doContinue)
            {
                continue;
            }

            ValidateMultiInstanceLoop(node, title);

            ValidateIncomingOutgoing(node, title);

            ValidateWFDHasForm(node, title);

            ValidateCallActivity(node, title, out bool doContinueCallActivity);
            if (doContinueCallActivity)
            {
                continue;
            }

            ValidateSubProcess(node, title, out bool doContinueSubProcess);
            if (doContinueSubProcess)
            {
                continue;
            }

            ValidateScriptTask(node, title);

            ValidateServiceTask(node, title);

            ValidateNormalTask(node, title, out bool doContinueNormalTask);
            if (doContinueNormalTask)
            {
                continue;
            }

            ValidateResponder(node, title);
        }

    }
    private string ValidateWFDTitle(XElement node)
    {
        var title = (string)node.Attribute("name");
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(@"عنوان مرحله وارد نشده است.", (string)node.Attribute("id"));
        }

        return title;
    }
    private void ValidateWFDNonRepetition(XElement node, Guid workFlowId)
    {
        var wfdId = node.ToString()
            .Split(new[] { "workflow:WorkFlowDetailId=\"" }, StringSplitOptions.None)[1]
            .Split(new[] { "\"" }, StringSplitOptions.None)[0];

        var wfdGuid = Guid.Parse(wfdId);
        var wfd = _unitOfWork.WorkflowDetails.SingleOrDefault(c => c.Id == wfdGuid);
        if (wfd != null && workFlowId != wfd.WorkFlowId)
        {
            throw new ArgumentException(@"شناسه فرآیند تکراریست و امکان ثبت فرآیند وجود ندارد.");
        }
    }
    private void ValidateAdHoc(XElement node, string title, out bool doContinue)
    {
        if (node.Name.LocalName == "adHocSubProcess")
        {
            var income = node.Elements(_bpmn2 + "incoming").ToList();
            var outgo = node.Elements(_bpmn2 + "outgoing").ToList();
            if (outgo.Any() || income.Any())
            {
                throw new ArgumentException(@"زیر فرآیند Ad-Hoc نباید ورودی و خروجی داشته باشد.", (string)node.Attribute("id"));
            }
            var workflowId = (string)node.Attribute(_workflow + "WorkflowId");
            if (string.IsNullOrWhiteSpace(workflowId) || workflowId == "null")
            {
                throw new ArgumentException(@"در مرحله " + title + @" فرآیند انتخاب نشده است.", (string)node.Attribute("id"));
            }
            if (_unitOfWork.Workflows.Single(d => d.Id.ToString() == workflowId).FlowType.Code != 2)
                throw new ArgumentException(@"در مرحله " + title + @" زیر فرآیند نهایی نشده است.", (string)node.Attribute("id"));
            doContinue = true;
        }
        else
        {
            doContinue = false;
        }
    }
    private void ValidateMultiInstanceLoop(XElement node, string title)
    {
        var isOrLogic = (string)node.Attribute(_workflow + "IsOrLogic");
        if (isOrLogic != null)
        {
            if (bool.Parse(isOrLogic) && node.ToString().Contains("<bpmn2:multiInstanceLoopCharacteristics />"))
            {
                throw new ArgumentException($@"خطای عملکرد چند وظیفه ای در تسک {title}.", (string)node.Attribute("id"));
            }
        }
    }
    private void ValidateIncomingOutgoing(XElement node, string title)
    {
        var outgoing = node.Elements(_bpmn2 + "outgoing").ToList();
        var incoming = node.Elements(_bpmn2 + "incoming").ToList();

        if (!outgoing.Any())
        {
            throw new ArgumentException(@" مرحله " + title + @" خروجی ندارد.", (string)node.Attribute("id"));
        }

        if (!incoming.Any())
        {
            throw new ArgumentException(@" مرحله " + title + @" ورودی ندارد.", (string)node.Attribute("id"));
        }
        if (outgoing.Count > 1)
        {
            throw new ArgumentException(@" مرحله " + title + @" نمی تواند بیش تر از یک خروجی داشته باشد.", (string)node.Attribute("id"));
        }
    }
    private void ValidateWFDHasForm(XElement node, string title)
    {
        var viewName = (string)node.Attribute(_workflow + "ViewName");
        var workflowForm = (string)node.Attribute(_workflow + "WorkFlowFormId");
        if ((viewName == null || viewName == "null") && (workflowForm == null || workflowForm == "null") && node.Name != _bpmn2 + "callActivity" && node.Name != _bpmn2 + "subProcess" && node.Name != _bpmn2 + "serviceTask" && node.Name != _bpmn2 + BpmnNodeConstant.ManualTask && node.Name != _bpmn2 + BpmnNodeConstant.ScriptTask)
        {
            throw new ArgumentException(@"فرمی برای  " + title + @" انتخاب نشده است.", (string)node.Attribute("id"));
        }
        if (!(viewName == null || viewName == "null") && !(workflowForm == null || workflowForm == "null"))
        {
            throw new ArgumentException(@"نمی توان برای " + title + @" هم فرم پویا و هم فرم ثابت انتخاب کرد.", (string)node.Attribute("id"));
        }

        if (node.Name.LocalName.ToLower() != BpmnNodeConstant.SubProcess.ToLower()
            && node.Name.LocalName.ToLower() != BpmnNodeConstant.CallActivity.ToLower()
            && node.Name.LocalName.ToLower() != BpmnNodeConstant.ServiceTask.ToLower()
            && node.Name.LocalName.ToLower() != BpmnNodeConstant.ManualTask.ToLower()
            && node.Name.LocalName.ToLower() != BpmnNodeConstant.ScriptTask.ToLower())
        {
            var workflowFormId = Guid.Parse(workflowForm!);
            var form = _unitOfWork.WorkFlowForm.SingleOrDefault(c => c.Id == workflowFormId);
            if (form != null)
            {
                if (!ValidateFormList(form))
                {
                    throw new ArgumentException($@"در مرحله {title}، فرم دارای لیست خالی می باشد.", (string)node.Attribute("id"));
                }
            }
        }
    }
    private bool ValidateFormList(WorkFlowForm form)
    {
        var encoding = new UnicodeEncoding();
        var encodeJson = HelperBs.EncodeUri(encoding.GetString(form.Content));
        var json = HttpUtility.UrlDecode(encodeJson, Encoding.UTF8);
        var jsonObject = JObject.Parse(json);
        var fields = jsonObject["fields"];
        if (fields != null)
        {
            foreach (JToken filed in fields)
            {
                var fli = filed.First!.ToObject<JObject>();
                var fieldId = (string)fli!["meta"]!["id"];
                if (fieldId == "gridList")
                {
                    var value = (string)fli["attrs"]!["value"];
                    var isExist = _unitOfWork.WorkFlowFormLists.Find(c => c.Id == Guid.Parse(value));
                    if (isExist == null)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
    private void ValidateCallActivity(XElement node, string title, out bool doContinue)
    {
        if (node.Name == _bpmn2 + BpmnNodeConstant.CallActivity)
        {
            var workflowId = (string)node.Attribute(_workflow + "WorkflowId");
            if (string.IsNullOrWhiteSpace(workflowId) || workflowId == "null")
            {
                throw new ArgumentException(@"در مرحله " + title + @" فرآیند انتخاب نشده است.", (string)node.Attribute("id"));
            }

            doContinue = true;
        }
        else
        {
            doContinue = false;
        }
    }
    private void ValidateSubProcess(XElement node, string title, out bool doContinue)
    {
        if (node.Name == _bpmn2 + "subProcess")
        {
            var workflowId = (string)node.Attribute(_workflow + "WorkflowId");
            if (string.IsNullOrWhiteSpace(workflowId) || workflowId == "null")
            {
                throw new ArgumentException(@"در مرحله " + title + @" فرآیند انتخاب نشده است.", (string)node.Attribute("id"));
            }
            if (_unitOfWork.Workflows.Single(d => d.Id.ToString() == workflowId).FlowType.Code != 2)
                throw new ArgumentException(@"در مرحله " + title + @" زیر فرآیند نهایی نشده است.", (string)node.Attribute("id"));

            doContinue = true;
        }
        else
        {
            doContinue = false;
        }
    }
    private void ValidateScriptTask(XElement node, string title)
    {
        if (node.Name == _bpmn2 + BpmnNodeConstant.ScriptTask)
        {
            var scriptTaskMethod = (string)node.Attribute(_workflow + "ScriptTaskMethod");
            if (string.IsNullOrWhiteSpace(scriptTaskMethod))
            {
                throw new ArgumentException(@"در مرحله " + title + @" کد نوشته نشده است.", (string)node.Attribute("id"));
            }
        }
    }
    private void ValidateServiceTask(XElement node, string title)
    {
        if (node.Name == _bpmn2 + BpmnNodeConstant.ServiceTask)
        {
            var apiId = (string)node.Attribute(_workflow + "requestApiId");
            var apiResultObjectName = (string)node.Attribute(_workflow + "responseApiId");

            if (string.IsNullOrWhiteSpace(apiId))
            {
                throw new ArgumentException(@"در مرحله " + title + @" سرویسی انتخاب نشده است.", (string)node.Attribute("id"));
            }

            if (string.IsNullOrWhiteSpace(apiResultObjectName))
            {
                throw new ArgumentException(@"در مرحله " + title + @"عنوان خروجی سرویس وارد نشده است.", (string)node.Attribute("id"));
            }

            CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
            if (!provider.IsValidIdentifier(apiResultObjectName))
            {
                throw new ArgumentException(@"در مرحله " + title + @"عنوان خروجی سرویس معتبر نمی باشد.", (string)node.Attribute("id"));
            }
        }
    }
    private void ValidateNormalTask(XElement node, string title, out bool doContinue)
    {
        doContinue = false;
        if (node.Name != _bpmn2 + BpmnNodeConstant.ServiceTask
            && node.Name != _bpmn2 + BpmnNodeConstant.ManualTask
            && node.Name != _bpmn2 + BpmnNodeConstant.ScriptTask)
        {
            var nodeWaitingTime = (string)node.Attribute(_workflow + "WaitingTime");
            if (nodeWaitingTime == null && node.Name != _bpmn2 + "serviceTask")
            {
                throw new ArgumentException(@"زمان انتظار وارد نشده است.", (string)node.Attribute("id"));
            }

            if (node.Name.LocalName == "businessRuleTask")
            {
                var businessAcceptorMethod = (string)node.Attribute(_workflow + "BusinessAcceptorMethod");
                if (string.IsNullOrWhiteSpace(businessAcceptorMethod))
                {
                    throw new ArgumentException(@"در مرحله " + title + @" کد نوشته نشده است.", (string)node.Attribute("id"));
                }

                doContinue = true;
            }
        }
    }
    #endregion


    #region Responders Validator
    private void ValidateResponder(XElement node, string title)
    {
        var model = new ValidateAcceptorsDto()
        {
            RequesterAccept = Convert.ToBoolean((string)node.Attribute(_workflow + "RequesterAccept")),
            SelectAcceptor = Convert.ToBoolean((string)node.Attribute(_workflow + "SelectAcceptor")),
            BusinessAcceptor = Convert.ToBoolean((string)node.Attribute(_workflow + "BusinessAcceptor")),
            OrganizationPostTitleId = (string)node.Attribute(_workflow + "OrganizationPostTitleId"),
            OrganizationPostTypeId = (string)node.Attribute(_workflow + "OrganizationPostTypeId"),
            ResponseGroupId = (string)node.Attribute(_workflow + "ResponseGroupId"),
            PatternId = (string)node.Attribute(_workflow + "PatternId"),
            SelectPatternFirst = Convert.ToBoolean((string)node.Attribute(_workflow + "SelectPatternFirst")),
            SelectPatternAll = Convert.ToBoolean((string)node.Attribute(_workflow + "SelectPatternAll")),
            StaffId = (string)node.Attribute(_workflow + "StaffId")
        };

        if (node.Name == _bpmn2 + BpmnNodeConstant.ServiceTask
            || node.Name == _bpmn2 + BpmnNodeConstant.ManualTask
            || node.Name == _bpmn2 + BpmnNodeConstant.ScriptTask)
        {
            var staff = _unitOfWork.Staffs.GetUserDefaultStaff();
            model.StaffId = staff.StaffId.ToString();
        }

        if (node.Name == _bpmn2 + BpmnNodeConstant.Task)
        {
            if (model.RequesterAccept)
            {
                ValidateRequesterAcceptor(node, model, title);
            }
            else if (model.BusinessAcceptor)
            {
                ValidateBusinessAcceptor(node, model, title);
            }
            else if (model.SelectAcceptor)
            {
                ValidateSelectAcceptor(node, model, title);
            }
            else if (!(model.StaffId == null || model.StaffId == "null"))
            {
                ValidateStaffAcceptor(node, model, title);
            }
            else if (!(model.OrganizationPostTypeId == null || model.OrganizationPostTypeId == "null"))
            {
                ValidateOrganizationPostTypeAcceptor(node, model, title);
            }
            else if (!(model.OrganizationPostTitleId == null || model.OrganizationPostTitleId == "null"))
            {
                ValidateOrganizationPostTitleAcceptor(node, model, title);
            }
            else if (!(model.PatternId == null || model.PatternId == "null"))
            {
                ValidatePatternAcceptor(node, model, title);
            }
            else if (!(model.ResponseGroupId == null || model.ResponseGroupId == "null"))
            {
                ValidateGroupAcceptor(node, model, title);
            }
            else if (model.ResponseGroupId == null || model.ResponseGroupId == "null")
                throw new ArgumentException(@" در مرحله " + title + @" هیچ پاسخ دهنده ای انتخاب نشده است.",
                    (string)node.Attribute("id"));
        }
    }
    private void ValidateRequesterAcceptor(XElement node, ValidateAcceptorsDto model, string title)
    {
        if (model.BusinessAcceptor)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد تایید مرحله توسط درخواست دهنده نمی توان فیلد پاسخ دهنده توسط کد انتخاب شود را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (model.SelectAcceptor)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد تایید مرحله توسط درخواست دهنده نمی توان فیلد انتخاب پاسخ دهنده توسط  تایید کننده را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (!(model.StaffId == null || model.StaffId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد تایید مرحله توسط درخواست دهنده نمی توان فیلد نام پرسنل را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (!(model.OrganizationPostTypeId == null || model.OrganizationPostTypeId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد تایید مرحله توسط درخواست دهنده نمی توان فیلد نوع پست سازمانی را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (!(model.OrganizationPostTitleId == null || model.OrganizationPostTitleId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد تایید مرحله توسط درخواست دهنده نمی توان فیلد عنوان پست سازمانی را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (!(model.ResponseGroupId == null || model.ResponseGroupId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد تایید مرحله توسط درخواست دهنده نمی توان فیلد گروه پاسخ دهنده را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (!(model.PatternId == null || model.PatternId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد تایید مرحله توسط درخواست دهنده نمی توان  پاسخ دهنده توسط الگو را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (model.SelectPatternFirst)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد تایید مرحله توسط درخواست دهنده نمی توان  پاسخ دهنده توسط الگو (ارسال به اولین پست )را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (model.SelectPatternAll)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد تایید مرحله توسط درخواست دهنده نمی توان  پاسخ دهنده توسط الگو (ارسال ترتیبی به همه پست ها)را انتخاب نمود.", (string)node.Attribute("id"));
        }
    }
    private void ValidateBusinessAcceptor(XElement node, ValidateAcceptorsDto model, string title)
    {
        if (node.Name != _bpmn2 + "businessRuleTask")
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط کد انتخاب شود حتما باید Task از نوع BussinessRole انتخاب شود.", (string)node.Attribute("id"));
        }
        if (model.SelectAcceptor)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد تایید مرحله توسط درخواست دهنده نمی توان فیلد انتخاب پاسخ دهنده توسط  تایید کننده را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (!(model.StaffId == null || model.StaffId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط کد انتخاب شود نمی توان فیلد نام پرسنل را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (!(model.OrganizationPostTypeId == null || model.OrganizationPostTypeId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط کد انتخاب شود نمی توان فیلد نوع پست سازمانی را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (!(model.OrganizationPostTitleId == null || model.OrganizationPostTitleId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط کد انتخاب شود نمی توان فیلد عنوان پست سازمانی را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (!(model.ResponseGroupId == null || model.ResponseGroupId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد  پاسخ دهنده توسط کد انتخاب شود نمی توان فیلد گروه پاسخ دهنده را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (!(model.PatternId == null || model.PatternId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد  پاسخ دهنده توسط کد انتخاب شود نمی توان  پاسخ دهنده توسط الگو را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (model.SelectPatternFirst)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد  پاسخ دهنده توسط کد انتخاب شود نمی توان  پاسخ دهنده توسط الگو (ارسال به اولین پست )را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (model.SelectPatternAll)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد  پاسخ دهنده توسط کد انتخاب شود نمی توان  پاسخ دهنده توسط الگو (ارسال ترتیبی به همه پست ها)را انتخاب نمود.", (string)node.Attribute("id"));
        }
    }
    private void ValidateSelectAcceptor(XElement node, ValidateAcceptorsDto model, string title)
    {
        if (!(model.StaffId == null || model.StaffId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط  تایید کننده نمی توان فیلد نام پرسنل را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (!(model.OrganizationPostTypeId == null || model.OrganizationPostTypeId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط  تایید کننده نمی توان فیلد نوع پست سازمانی را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (!(model.OrganizationPostTitleId == null || model.OrganizationPostTitleId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط  تایید کننده نمی توان فیلد عنوان پست سازمانی را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (!(model.ResponseGroupId == null || model.ResponseGroupId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط  تایید کننده نمی توان فیلد گروه پاسخ دهنده را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (!(model.PatternId == null || model.PatternId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط  تایید کننده نمی توان  پاسخ دهنده  توسط الگو را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (model.SelectPatternFirst)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط  تایید کننده نمی توان  پاسخ دهنده توسط الگو (ارسال به اولین پست )را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (model.SelectPatternAll)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط  تایید کننده نمی توان  پاسخ دهنده توسط الگو (ارسال ترتیبی به همه پست ها)را انتخاب نمود.", (string)node.Attribute("id"));
        }
    }
    private void ValidateStaffAcceptor(XElement node, ValidateAcceptorsDto model, string title)
    {
        if (!(model.OrganizationPostTypeId == null || model.OrganizationPostTypeId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد  نام پرسنل نمی توان فیلد نوع پست سازمانی را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (!(model.OrganizationPostTitleId == null || model.OrganizationPostTitleId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد نام پرسنل نمی توان فیلد عنوان پست سازمانی را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (!(model.ResponseGroupId == null || model.ResponseGroupId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد  نام پرسنل نمی توان فیلد گروه پاسخ دهنده را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (!(model.PatternId == null || model.PatternId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد  نام پرسنل نمی توان  پاسخ دهنده  توسط الگو را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (model.SelectPatternFirst)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد  نام پرسنل نمی توان  پاسخ دهنده توسط الگو (ارسال به اولین پست )را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (model.SelectPatternAll)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد  نام پرسنل نمی توان  پاسخ دهنده توسط الگو (ارسال ترتیبی به همه پست ها)را انتخاب نمود.", (string)node.Attribute("id"));
        }
        var guStaffId = Guid.Parse(model.StaffId);
        if (!_unitOfWork.Staffs.Any(s => s.Id == guStaffId))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @"نام پرسنل پاسخ دهنده مشخص نشده است.", (string)node.Attribute("id"));
        }
    }
    private void ValidateOrganizationPostTypeAcceptor(XElement node, ValidateAcceptorsDto model, string title)
    {
        if (!(model.OrganizationPostTitleId == null || model.OrganizationPostTitleId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد نوع پست سازمانی نمی توان فیلد عنوان پست سازمانی را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (!(model.ResponseGroupId == null || model.ResponseGroupId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد نوع پست سازمانی نمی توان فیلد گروه پاسخ دهنده را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (!(model.PatternId == null || model.PatternId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد نوع پست سازمانی نمی توان  پاسخ دهنده  توسط الگو را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (model.SelectPatternFirst)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد نوع پست سازمانی نمی توان  پاسخ دهنده توسط الگو (ارسال به اولین پست )را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (model.SelectPatternAll)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد نوع پست سازمانی نمی توان  پاسخ دهنده توسط الگو (ارسال ترتیبی به همه پست ها)را انتخاب نمود.", (string)node.Attribute("id"));
        }
        var guOrganizationPostTypeId = Guid.Parse(model.OrganizationPostTypeId);
        if (!_unitOfWork.LookUps.Where(l => l.Id == guOrganizationPostTypeId).Any())
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @"نوع پست سازمانی مشخص نشده است.", (string)node.Attribute("id"));
        }
    }
    private void ValidateOrganizationPostTitleAcceptor(XElement node, ValidateAcceptorsDto model, string title)
    {
        if (!(model.ResponseGroupId == null || model.ResponseGroupId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد عنوان پست سازمانی نمی توان فیلد گروه پاسخ دهنده را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (!(model.PatternId == null || model.PatternId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد عنوان پست سازمانی نمی توان  پاسخ دهنده توسط الگو را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (model.SelectPatternFirst)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد عنوان پست سازمانی نمی توان  پاسخ دهنده توسط الگو (ارسال به اولین پست )را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (model.SelectPatternAll)
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد عنوان پست سازمانی نمی توان  پاسخ دهنده توسط الگو (ارسال ترتیبی به همه پست ها)را انتخاب نمود.", (string)node.Attribute("id"));
        }
        var guOrganizationPostTitleId = Guid.Parse(model.OrganizationPostTitleId);
        if (!_unitOfWork.LookUps.Where(l => l.Id == guOrganizationPostTitleId).Any())
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @"عنوان پست سازمانی مشخص نشده است.", (string)node.Attribute("id"));
        }
    }
    private void ValidatePatternAcceptor(XElement node, ValidateAcceptorsDto model, string title)
    {
        if ((model.SelectPatternFirst && model.SelectPatternAll) || (!model.SelectPatternFirst && !model.SelectPatternAll))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" یکی از موارد ارسال به اولین پست و ارسال ترتیبی به همه پست ها را انتخاب نمایید.", (string)node.Attribute("id"));
        }
        if (!(model.StaffId == null || model.StaffId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط  تایید کننده نمی توان فیلد نام پرسنل را انتخاب نمود.", (string)node.Attribute("id"));
        }

        if (!(model.OrganizationPostTypeId == null || model.OrganizationPostTypeId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط  تایید کننده نمی توان فیلد نوع پست سازمانی را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (!(model.OrganizationPostTitleId == null || model.OrganizationPostTitleId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط  تایید کننده نمی توان فیلد عنوان پست سازمانی را انتخاب نمود.", (string)node.Attribute("id"));
        }
        if (!(model.ResponseGroupId == null || model.ResponseGroupId == "null"))
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @" با انتخاب فیلد پاسخ دهنده توسط  تایید کننده نمی توان فیلد گروه پاسخ دهنده را انتخاب نمود.", (string)node.Attribute("id"));
        }
        var guPatternId = Guid.Parse(model.PatternId);
        if (!_unitOfWork.WorkFlowDetailPattern.Where(p => p.Id == guPatternId).Any())
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @"الگوی پاسخ دهنده مشخص نشده است.", (string)node.Attribute("id"));
        }

    }
    private void ValidateGroupAcceptor(XElement node, ValidateAcceptorsDto model, string title)
    {
        var guResponseGroupId = Guid.Parse(model.ResponseGroupId);
        if (!_unitOfWork.LookUps.Where(l => l.Id == guResponseGroupId).Any())
        {
            throw new ArgumentException(@" در مرحله " + title +
                                        @"گروه پاسخ دهنده مشخص نشده است.", (string)node.Attribute("id"));
        }
    }
    #endregion
}