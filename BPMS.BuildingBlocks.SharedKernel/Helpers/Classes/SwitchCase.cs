namespace BPMS.BuildingBlocks.SharedKernel.Helpers.Classes;

public class SwitchCase<T, TR>
{
    internal readonly T input;

    internal TR? output;

    internal SwitchCase(T input) => this.input = input;

    internal void Set(TR tr) => output = tr;
}