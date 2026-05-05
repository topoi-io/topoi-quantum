namespace QuantumComputer.OpenQasm;

public sealed record OpenQasmToken(OpenQasmTokenKind Kind, string Text, int Line, int Column);