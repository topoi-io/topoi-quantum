namespace QuantumComputer.OpenQasm;

public abstract record OpenQasmStatement;

public sealed record OpenQasmIncludeStatement(string Path) : OpenQasmStatement;

public sealed record OpenQasmQubitDeclaration(string Name, int Size) : OpenQasmStatement;

public sealed record OpenQasmGateCallStatement(
    string GateName,
    IReadOnlyList<OpenQasmAngleExpression> Parameters,
    IReadOnlyList<OpenQasmQubitOperand> Qubits) : OpenQasmStatement;

public sealed record OpenQasmBitDeclaration(string Name, int Size) : OpenQasmStatement;

public sealed record OpenQasmMeasureStatement(OpenQasmQubitReference Qubit, OpenQasmBitReference Bit) : OpenQasmStatement;

public sealed record OpenQasmResetStatement(OpenQasmQubitReference Qubit) : OpenQasmStatement;

public sealed record OpenQasmBarrierStatement(IReadOnlyList<OpenQasmQubitReference> Qubits) : OpenQasmStatement;

public sealed record OpenQasmGateDefinitionStatement(string Name, IReadOnlyList<string> Parameters, IReadOnlyList<string> QubitParameters, IReadOnlyList<OpenQasmGateCallStatement> Body) : OpenQasmStatement;