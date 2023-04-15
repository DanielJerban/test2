using System.Xml.Linq;

namespace BPMS.Domain.Common.Helpers;

public static class BpmnHelper
{
    public static XElement NextFlow(this XElement element, XDocument doc)
    {
        return doc.Descendants().ToList().FirstOrDefault(d => (string)d.Attribute("sourceRef") == element.Attribute("targetRef")?.Value);
    }

    public static XElement TargetNode(this XElement element, XDocument doc)
    {
        return doc.Descendants().ToList().FirstOrDefault(d => (string)d.Attribute("id") == element.Attribute("targetRef")?.Value);
    }

    public static IEnumerable<XElement> NextFlows(this XElement gateway, XDocument doc)
    {
        return doc.Descendants().ToList().Where(d => (string)d.Attribute("sourceRef") == gateway.Attribute("targetRef")?.Value).ToList();
    }

    public static bool NameContains(this XElement element, string name)
    {
        return element.Name.ToString().ToLower().Contains(name);
    }
}