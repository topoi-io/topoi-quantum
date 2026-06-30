namespace Topoi.Quantum.OpenQasm;

public sealed record OpenQasmQubitOperand(string Name, int? Index)
{
    public bool IsIndexed => Index is not null;

    public override string ToString()
    {
        return Index is null
            ? Name
            : $"{Name}[{Index.Value}]";
    }
}