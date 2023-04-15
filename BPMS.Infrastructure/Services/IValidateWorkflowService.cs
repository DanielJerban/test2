using BPMS.Domain.Common.ViewModels;
using System.Xml.Linq;

namespace BPMS.Infrastructure.Services;

public interface IValidateWorkflowService
{
    XDocument ValidateModel(BpmnDiagramViewModel model);
}