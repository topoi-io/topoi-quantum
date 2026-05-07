using QuantumComputer.Core;

namespace QuantumComputer.OpenQasm;

public abstract record OpenQasmExecutableOperation;

public sealed record OpenQasmGateOperation(GateOperation Operation)
    : OpenQasmExecutableOperation;

public sealed record OpenQasmMeasureOperation(int Qubit, int Bit)
    : OpenQasmExecutableOperation;

public sealed record OpenQasmResetOperation(int Qubit)
    : OpenQasmExecutableOperation;

public sealed record OpenQasmBarrierOperation(IReadOnlyList<int> Qubits)
    : OpenQasmExecutableOperation;