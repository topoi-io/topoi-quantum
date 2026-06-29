using NUnit.Framework;
using QuantumComputer.Core;
using QuantumComputer.Core.Primitives;

namespace QuantumComputer.Tests;

[TestFixture]
public sealed class EstimatorTests
{
    [Test]
    public void Estimate_BellCircuit_ZZ_ReturnsOne()
    {
        var circuit = new QuantumCircuit(2);

        circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));
        circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 1 }));

        var estimator = new Estimator();

        EstimatorResult result = estimator.Estimate(
            circuit,
            new[]
            {
                new PauliTerm('Z', 0),
                new PauliTerm('Z', 1)
            });

        Assert.That(result.Value, Is.EqualTo(1.0).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void Estimate_Hadamard_X_ReturnsOne()
    {
        var circuit = new QuantumCircuit(1);

        circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));

        var estimator = new Estimator();

        EstimatorResult result = estimator.Estimate(
            circuit,
            new[]
            {
                new PauliTerm('X', 0)
            });

        Assert.That(result.Value, Is.EqualTo(1.0).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void Estimate_Hadamard_Z_ReturnsZero()
    {
        var circuit = new QuantumCircuit(1);

        circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));

        var estimator = new Estimator();

        EstimatorResult result = estimator.Estimate(
            circuit,
            new[]
            {
                new PauliTerm('Z', 0)
            });

        Assert.That(result.Value, Is.EqualTo(0.0).Within(TestHelpers.Tolerance));
    }
}