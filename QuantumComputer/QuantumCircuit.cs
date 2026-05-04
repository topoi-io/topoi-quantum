namespace QuantumComputer;

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

    public void Run(bool resetFirst = true)
    {
        if (Quantum.Register.QubitCount != QubitCount)
        {
            if (!resetFirst)
            {
                throw new InvalidOperationException(
                    "Cannot run circuit without reset because the current simulator register " +
                    $"has {Quantum.Register.QubitCount} qubits but the circuit requires {QubitCount}.");
            }

            Quantum.Init(QubitCount);
        }
        else if (resetFirst)
        {
            Quantum.Reset();
        }

        foreach (GateOperation operation in _operations)
            operation.Apply();
    }

    public void Print()
    {
        Console.WriteLine($"Circuit: {QubitCount} qubits, {_operations.Count} operations");

        if (_operations.Count == 0)
        {
            Console.WriteLine("(empty)");
            return;
        }

        for (int i = 0; i < _operations.Count; i++)
            Console.WriteLine($"{i}: {_operations[i].ToCommandString()}");
    }

    public string Draw()
    {
        return CircuitDrawer.Draw(this);
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