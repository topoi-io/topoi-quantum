namespace Topoi.Quantum.OpenQasm;

public sealed class OpenQasmExecutableProgram
{
    public int QubitCount { get; }
    public IReadOnlyDictionary<string, (int Offset, int Size)> QubitRegisters { get; }
    public IReadOnlyDictionary<string, (int Offset, int Size)> BitRegisters { get; }
    public IReadOnlyList<OpenQasmExecutableOperation> Operations { get; }

    public OpenQasmExecutableProgram(
        int qubitCount,
        IReadOnlyDictionary<string, (int Offset, int Size)> qubitRegisters,
        IReadOnlyDictionary<string, (int Offset, int Size)> bitRegisters,
        IReadOnlyList<OpenQasmExecutableOperation> operations)
    {
        QubitCount = qubitCount;
        QubitRegisters = qubitRegisters;
        BitRegisters = bitRegisters;
        Operations = operations;
    }

    public QuantumCircuit ToUnitaryCircuit()
    {
        var circuit = new QuantumCircuit(QubitCount);

        foreach (OpenQasmExecutableOperation operation in Operations)
        {
            if (operation is OpenQasmGateOperation gate)
                circuit.Add(gate.Operation);
            else if (operation is OpenQasmBarrierOperation)
                continue;
            else
                throw new InvalidOperationException(
                    $"Cannot convert non-unitary operation '{operation.GetType().Name}' to QuantumCircuit.");
        }

        return circuit;
    }
}