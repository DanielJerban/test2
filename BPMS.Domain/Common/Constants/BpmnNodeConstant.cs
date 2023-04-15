namespace BPMS.Domain.Common.Constants;

public class BpmnNodeConstant
{
    public const string Task = "task";
    public const string ManualTask = "manualTask";
    public const string ScriptTask = "scriptTask";
    public const string SubProcess = "subProcess";
    public const string CallActivity = "callActivity";
    public const string ServiceTask = "serviceTask";
    public const string BusinessRuleTask = "businessRuleTask";
    public const string AdHocSubProcess = "adHocSubProcess";


    public class Boundary
    {
        public const string BoundaryTimerEvent = "BoundaryTimerEvent";
        public const string NonInterruptingBoundaryTimerEvent = "NonInterruptingBoundaryTimerEvent";
        public const string IntermediateCatchEventTimer = "intermediateCatchEventTimer";
        public const string IntermediateCatchEventSignal = "intermediateCatchEventSignal";
        public const string IntermediateCatchEventMessage = "intermediateCatchEventMessage";
        public const string BoundaryErrorEvent = "BoundaryErrorEvent";
    }
}