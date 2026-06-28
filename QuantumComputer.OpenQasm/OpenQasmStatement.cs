namespace QuantumComputer.OpenQasm;

public abstract record OpenQasmStatement;

public sealed record OpenQasmIncludeStatement(string Path) : OpenQasmStatement;

public sealed record OpenQasmQubitDeclaration(
    string Name,
    int Size,
    int Line,
    int Column) : OpenQasmStatement;

public sealed record OpenQasmGateCallStatement(
    string GateName,
    IReadOnlyList<OpenQasmAngleExpression> Parameters,
    IReadOnlyList<OpenQasmQubitOperand> Qubits,
    IReadOnlyList<OpenQasmGateModifier> Modifiers,
    int Line,
    int Column) : OpenQasmStatement;

public sealed record OpenQasmBitDeclaration(
    string Name,
    int Size,
    int Line,
    int Column) : OpenQasmStatement;

public sealed record OpenQasmMeasureStatement(
    OpenQasmQubitReference Qubit,
    OpenQasmBitReference Bit,
    int Line,
    int Column) : OpenQasmStatement;

public sealed record OpenQasmResetStatement(
    OpenQasmQubitReference Qubit,
    int Line,
    int Column) : OpenQasmStatement;

public sealed record OpenQasmBarrierStatement(
    IReadOnlyList<OpenQasmQubitReference> Qubits,
    int Line,
    int Column) : OpenQasmStatement;

public sealed record OpenQasmGateDefinitionStatement(
    string Name,
    IReadOnlyList<string> Parameters,
    IReadOnlyList<string> QubitParameters,
    IReadOnlyList<OpenQasmGateCallStatement> Body,
    int Line,
    int Column) : OpenQasmStatement;