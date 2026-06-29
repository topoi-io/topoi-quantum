namespace QuantumComputer.Core.Primitives;

public sealed record EstimatorResult(
    int QubitCount,
    IReadOnlyList<PauliTerm> Observable,
    double Value);
