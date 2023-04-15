using System.Xml.Linq;
using BPMS.Domain.Entities;

namespace BPMS.Domain.Common.Dtos;

public class ModelerParamDto
{
    public XDocument Myxml { get; set; }
    public string Target { get; set; }
    public string BoundryName { get; set; }
    public WorkFlowDetail WorkFlowDetailsFrom { get; set; }
    public WorkFlowDetail WorkFlowDetailsEnd { get; set; }
    public WorkFlowDetail WorkFlowDetailsTerminate { get; set; }
    public List<WorkFlowDetail> WorkFlowDetailsList { get; set; }
    public List<WorkFlowNextStep> WorkFlowNextStepsList { get; set; }
    public List<WorkflowEsb> WorkflowEsbsList { get; set; }
    public Guid WorkFlowNextStepId { get; set; }

    public Stack<string> Path { get; set; }
    public Stack<string> Exp { get; set; }
    public Stack<string> Esb { get; set; }
    public Stack<string> Evt { get; set; }
    public Stack<string> Method { get; set; }
    public Stack<string> Gateway { get; set; }
    public Stack<string> FlowLine { get; set; }
}