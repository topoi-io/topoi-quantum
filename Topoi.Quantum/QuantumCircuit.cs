namespace Topoi.Quantum;

public sealed class QuantumCircuit
{
    private readonly List<GateOperation> _operations = new();

    public int QubitCount { get; }

    public IReadOnlyList<GateOperation> Operations => _operations;

    public QuantumCircuit(int qubitCount)
    {
        if (qubitCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(qubitCount));

        QubitCount = qubitCount;
    }

    public void Add(GateOperation operation)
    {
        operation.ValidateForCircuit(QubitCount);
        _operations.Add(operation);
    }

    public void Run(QuantumSimulator simulator, bool resetFirst = true)
    {
        if (simulator is null)
            throw new ArgumentNullException(nameof(simulator));

        if (simulator.Register.QubitCount != QubitCount)
        {
            throw new InvalidOperationException(
                $"Simulator has {simulator.Register.QubitCount} qubits, but circuit requires {QubitCount}.");
        }

        if (resetFirst)
            simulator.Reset();

        foreach (GateOperation operation in _operations)
            operation.Apply(simulator);
    }

    private void ValidateOperation(GateOperation operation)
    {
        foreach (int q in operation.Qubits)
        {
            if ((uint)q >= (uint)QubitCount)
                throw new ArgumentOutOfRangeException(
                    nameof(operation),
                    $"Gate {operation.Kind} uses qubit {q}, but circuit has qubits 0..{QubitCount - 1}");
        }

        if (operation.Qubits.Count != operation.Qubits.Distinct().Count())
            throw new ArgumentException($"Gate {operation.Kind} contains duplicate qubits.");
    }
}