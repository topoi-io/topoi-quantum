namespace QuantumComputer.Core;

public sealed class QuantumCircuitBuilder
{
    private readonly QuantumCircuit _circuit;

    private QuantumCircuitBuilder(int qubitCount)
    {
        _circuit = new QuantumCircuit(qubitCount);
    }

    public static QuantumCircuitBuilder WithQubits(int qubitCount)
    {
        return new QuantumCircuitBuilder(qubitCount);
    }

    public QuantumCircuit Build()
    {
        return _circuit;
    }

    public QuantumCircuitBuilder I(int q) => Add(GateKind.I, q);
    public QuantumCircuitBuilder X(int q) => Add(GateKind.X, q);
    public QuantumCircuitBuilder Y(int q) => Add(GateKind.Y, q);
    public QuantumCircuitBuilder Z(int q) => Add(GateKind.Z, q);
    public QuantumCircuitBuilder H(int q) => Add(GateKind.H, q);
    public QuantumCircuitBuilder S(int q) => Add(GateKind.S, q);
    public QuantumCircuitBuilder SDG(int q) => Add(GateKind.SDG, q);
    public QuantumCircuitBuilder T(int q) => Add(GateKind.T, q);
    public QuantumCircuitBuilder TDG(int q) => Add(GateKind.TDG, q);
    public QuantumCircuitBuilder SX(int q) => Add(GateKind.SX, q);
    public QuantumCircuitBuilder SXDG(int q) => Add(GateKind.SXDG, q);

    public QuantumCircuitBuilder RX(int q, double theta) => Add(GateKind.RX, new[] { q }, theta);
    public QuantumCircuitBuilder RY(int q, double theta) => Add(GateKind.RY, new[] { q }, theta);
    public QuantumCircuitBuilder RZ(int q, double theta) => Add(GateKind.RZ, new[] { q }, theta);

    public QuantumCircuitBuilder CX(int control, int target) => Add(GateKind.CX, control, target);
    public QuantumCircuitBuilder CNOT(int control, int target) => CX(control, target);
    public QuantumCircuitBuilder CY(int control, int target) => Add(GateKind.CY, control, target);
    public QuantumCircuitBuilder CZ(int control, int target) => Add(GateKind.CZ, control, target);
    public QuantumCircuitBuilder CH(int control, int target) => Add(GateKind.CH, control, target);
    public QuantumCircuitBuilder SWAP(int q1, int q2) => Add(GateKind.SWAP, q1, q2);

    public QuantumCircuitBuilder CP(int control, int target, double theta) =>
        Add(GateKind.CP, new[] { control, target }, theta);

    public QuantumCircuitBuilder CRX(int control, int target, double theta) =>
        Add(GateKind.CRX, new[] { control, target }, theta);

    public QuantumCircuitBuilder CRY(int control, int target, double theta) =>
        Add(GateKind.CRY, new[] { control, target }, theta);

    public QuantumCircuitBuilder CRZ(int control, int target, double theta) =>
        Add(GateKind.CRZ, new[] { control, target }, theta);

    public QuantumCircuitBuilder CCX(int control1, int control2, int target) =>
        Add(GateKind.CCX, new[] { control1, control2, target });

    public QuantumCircuitBuilder Toffoli(int control1, int control2, int target) =>
        CCX(control1, control2, target);

    private QuantumCircuitBuilder Add(GateKind kind, int q)
    {
        return Add(kind, new[] { q });
    }

    private QuantumCircuitBuilder Add(GateKind kind, int q1, int q2)
    {
        return Add(kind, new[] { q1, q2 });
    }

    private QuantumCircuitBuilder Add(
        GateKind kind,
        IReadOnlyList<int> qubits,
        double? angle = null)
    {
        _circuit.Add(new GateOperation(kind, qubits, angle));
        return this;
    }
}