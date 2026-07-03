using NUnit.Framework;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class MeasurementTests
{
    [Test]
    public void MeasureAll_CollapsesZeroStateToZero()
    {
        var simulator = new QuantumSimulator(2);
        int outcome = simulator.MeasureAll();

        Assert.That(outcome, Is.EqualTo(0));
        TestHelpers.AssertProbability(simulator, 0, 1.0);
    }

    [Test]
    public void MeasureAll_CollapsesKnownBasisState()
    {
        var simulator = new QuantumSimulator(3);
        simulator.X(0);
        simulator.X(2);

        int expected = TestHelpers.Basis(0, 2);
        int outcome = simulator.MeasureAll();

        Assert.That(outcome, Is.EqualTo(expected));
        TestHelpers.AssertProbability(simulator, expected, 1.0);
    }

    [Test]
    public void MeasureQubit_OnKnownZero_ReturnsZero()
    {
        var simulator = new QuantumSimulator(3);
        int bit = simulator.Measure(0);

        Assert.That(bit, Is.EqualTo(0));
        TestHelpers.AssertProbability(simulator, 0, 1.0);
    }

    [Test]
    public void MeasureQubit_OnKnownOne_ReturnsOne()
    {
        var simulator = new QuantumSimulator(2);
        simulator.X(1);

        int bit = simulator.Measure(1);

        Assert.That(bit, Is.EqualTo(1));
        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(1), 1.0);
    }

    [Test]
    public void SnapshotAndRestore_PreservesPreparedState()
    {
        var simulator = new QuantumSimulator(2);
        simulator.H(0);
        simulator.CX(0, 1);

        var snapshot = simulator.SnapshotState();

        simulator.MeasureAll();
        simulator.RestoreState(snapshot);

        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(), 0.5);
        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(0, 1), 0.5);
    }
}