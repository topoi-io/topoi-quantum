namespace Topoi.Quantum.Primitives;

public sealed record EstimatorResult(
    int QubitCount,
    IReadOnlyList<PauliTerm> Observable,
    double Value);
