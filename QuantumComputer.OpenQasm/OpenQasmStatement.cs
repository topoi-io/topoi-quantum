namespace QuantumComputer.OpenQasm;

public abstract record OpenQasmStatement;

public sealed record OpenQasmIncludeStatement(string Path) : OpenQasmStatement;

public sealed record OpenQasmQubitDeclaration(string Name, int Size) : OpenQasmStatement;

public sealed record OpenQasmGateCallStatement(string GateName, IReadOnlyList<double> Parameters, IReadOnlyList<OpenQasmQubitReference> Qubits) : OpenQasmStatement;