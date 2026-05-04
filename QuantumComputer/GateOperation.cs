namespace QuantumComputer;

public sealed record GateOperation(GateKind Kind, IReadOnlyList<int> Qubits, double? Angle = null)
{
    public void Apply()
    {
        switch (Kind)
        {
            case GateKind.X:
                Quantum.X(Qubits[0]);
                break;

            case GateKind.Y:
                Quantum.Y(Qubits[0]);
                break;

            case GateKind.Z:
                Quantum.Z(Qubits[0]);
                break;

            case GateKind.H:
                Quantum.H(Qubits[0]);
                break;

            case GateKind.S:
                Quantum.S(Qubits[0]);
                break;

            case GateKind.T:
                Quantum.T(Qubits[0]);
                break;

            case GateKind.RX:
                Quantum.RX(Qubits[0], RequireAngle());
                break;

            case GateKind.RY:
                Quantum.RY(Qubits[0], RequireAngle());
                break;

            case GateKind.RZ:
                Quantum.RZ(Qubits[0], RequireAngle());
                break;

            case GateKind.CX:
                Quantum.CX(Qubits[0], Qubits[1]);
                break;

            case GateKind.CZ:
                Quantum.CZ(Qubits[0], Qubits[1]);
                break;

            case GateKind.SWAP:
                Quantum.SWAP(Qubits[0], Qubits[1]);
                break;

            case GateKind.CCX:
                Quantum.CCX(Qubits[0], Qubits[1], Qubits[2]);
                break;

            case GateKind.CRX:
                Quantum.CRX(Qubits[0], Qubits[1], RequireAngle());
                break;

            case GateKind.CRY:
                Quantum.CRY(Qubits[0], Qubits[1], RequireAngle());
                break;

            case GateKind.CRZ:
                Quantum.CRZ(Qubits[0], Qubits[1], RequireAngle());
                break;

            default:
                throw new NotSupportedException($"Unsupported gate kind: {Kind}");
        }
    }

    public string ToCommandString()
    {
        return Kind switch
        {
            GateKind.X or GateKind.Y or GateKind.Z or GateKind.H or GateKind.S or GateKind.T
                => $"{Kind} {Qubits[0]}",

            GateKind.RX or GateKind.RY or GateKind.RZ
                => $"{Kind} {Qubits[0]} {RequireAngle():R}",

            GateKind.CX or GateKind.CZ or GateKind.SWAP
                => $"{Kind} {Qubits[0]} {Qubits[1]}",

            GateKind.CCX
                => $"{Kind} {Qubits[0]} {Qubits[1]} {Qubits[2]}",

            GateKind.CRX or GateKind.CRY or GateKind.CRZ
                => $"{Kind} {Qubits[0]} {Qubits[1]} {RequireAngle():R}",

            _ => Kind.ToString()
        };
    }

    private double RequireAngle()
    {
        if (Angle is null)
            throw new InvalidOperationException($"Gate {Kind} requires an angle.");

        return Angle.Value;
    }
}