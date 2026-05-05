namespace QuantumComputer.OpenQasm;

public sealed class OpenQasmProgram
{
    public string Version { get; }
    public IReadOnlyList<OpenQasmStatement> Statements { get; }

    public OpenQasmProgram(string version, IReadOnlyList<OpenQasmStatement> statements)
    {
        Version = version;
        Statements = statements;
    }
}