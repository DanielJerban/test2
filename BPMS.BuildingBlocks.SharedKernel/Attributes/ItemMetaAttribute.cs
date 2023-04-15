namespace BPMS.BuildingBlocks.SharedKernel.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class ItemMetaAttribute : Attribute
{
    public string Name { get; set; }
    public string Content { get; set; }

    public ItemMetaAttribute(string name, string content)
    {
        Name = name;
        Content = content;
    }
}