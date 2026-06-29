namespace QuantumComputer.Core.Primitives;

public sealed class Estimator
{
    public EstimatorResult Estimate(
        QuantumCircuit circuit,
        IReadOnlyList<PauliTerm> observable)
    {
        if (circuit is null)
            throw new ArgumentNullException(nameof(circuit));

        if (observable is null)
            throw new ArgumentNullException(nameof(observable));

        var simulator = new QuantumSimulator(circuit.QubitCount);

        circuit.Run(simulator, resetFirst: true);

        double value = simulator
            .ExpectPauliString(observable)
            .Real;

        return new EstimatorResult(
            circuit.QubitCount,
            observable,
            value);
    }
}