using QuantumComputer.Core.Primitives;

namespace QuantumComputer.Core;

public static class QuantumToolkit
{
    public static Sampler Sampler { get; } = new();

    public static Estimator Estimator { get; } = new();

    public static QuantumCircuitBuilder Circuit(int qubitCount)
    {
        return QuantumCircuitBuilder.WithQubits(qubitCount);
    }
}