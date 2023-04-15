namespace BPMS.BuildingBlocks.SharedKernel.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class SummaryAttribute : Attribute
{
    public string Summary { get; set; }

    public SummaryAttribute(string summary)
    {
        Summary = summary;
    }
}