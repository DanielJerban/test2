namespace BPMS.BuildingBlocks.SharedKernel.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class TitleAttribute : Attribute
{
    public string Title { get; set; }

    public TitleAttribute(string title)
    {
        Title = title;
    }
}