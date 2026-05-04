namespace QuantumComputer;

public sealed record GateOperation(GateKind Kind, IReadOnlyList<int> Qubits, double? Angle = null)
{
    public void ValidateForCircuit(int circuitQubitCount)
    {
        if (Qubits is null)
            throw new ArgumentException("Gate operation qubit list cannot be null.");

        GateSpec spec = GateSpecs.For(Kind);

        if (Qubits.Count != spec.QubitCount)
        {
            throw new ArgumentException(
                $"Gate {Kind} expects {spec.QubitCount} qubit(s), " +
                $"but received {Qubits.Count}.");
        }

        if (spec.RequiresAngle && Angle is null)
            throw new ArgumentException($"Gate {Kind} requires an angle.");

        if (!spec.RequiresAngle && Angle is not null)
            throw new ArgumentException($"Gate {Kind} does not take an angle.");

        foreach (int q in Qubits)
        {
            if ((uint)q >= (uint)circuitQubitCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(Qubits),
                    $"Gate {Kind} uses qubit {q}, but valid qubits are 0..{circuitQubitCount - 1}.");
            }
        }

        if (Qubits.Count != Qubits.Distinct().Count())
            throw new ArgumentException($"Gate {Kind} contains duplicate qubits.");
    }

    public void Apply()
    {
        Apply(Quantum.DefaultSimulator);
    }

    public void Apply(QuantumSimulator simulator)
    {
        if (simulator is null)
            throw new ArgumentNullException(nameof(simulator));

        ValidateForCircuit(simulator.Register.QubitCount);

        switch (Kind)
        {
            case GateKind.X:
                simulator.X(Qubits[0]);
                break;

            case GateKind.Y:
                simulator.Y(Qubits[0]);
                break;

            case GateKind.Z:
                simulator.Z(Qubits[0]);
                break;

            case GateKind.H:
                simulator.H(Qubits[0]);
                break;

            case GateKind.S:
                simulator.S(Qubits[0]);
                break;

            case GateKind.T:
                simulator.T(Qubits[0]);
                break;

            case GateKind.RX:
                simulator.RX(Qubits[0], RequireAngle());
                break;

            case GateKind.RY:
                simulator.RY(Qubits[0], RequireAngle());
                break;

            case GateKind.RZ:
                simulator.RZ(Qubits[0], RequireAngle());
                break;

            case GateKind.CX:
                simulator.CX(Qubits[0], Qubits[1]);
                break;

            case GateKind.CZ:
                simulator.CZ(Qubits[0], Qubits[1]);
                break;

            case GateKind.SWAP:
                simulator.SWAP(Qubits[0], Qubits[1]);
                break;

            case GateKind.CCX:
                simulator.CCX(Qubits[0], Qubits[1], Qubits[2]);
                break;

            case GateKind.CRX:
                simulator.CRX(Qubits[0], Qubits[1], RequireAngle());
                break;

            case GateKind.CRY:
                simulator.CRY(Qubits[0], Qubits[1], RequireAngle());
                break;

            case GateKind.CRZ:
                simulator.CRZ(Qubits[0], Qubits[1], RequireAngle());
                break;

            default:
                throw new NotSupportedException($"Unsupported gate kind: {Kind}");
        }
    }

    private double RequireAngle()
    {
        if (Angle is null)
            throw new InvalidOperationException($"Gate {Kind} requires an angle.");

        return Angle.Value;
    }

    // keep your existing ToCommandString() here
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
}