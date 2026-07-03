using NUnit.Framework;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class SingleQubitGateTests
{
    private QuantumSimulator _simulator = null!;

    [SetUp]
    public void SetUp()
    {
        _simulator = new QuantumSimulator(1);
        _simulator.Reset();
    }

    [Test]
    public void Reset_PreparesZeroState()
    {
        TestHelpers.AssertProbability(_simulator, 0, 1.0);
        TestHelpers.AssertProbability(_simulator, 1, 0.0);
    }

    [Test]
    public void X_FlipsZeroToOne()
    {
        _simulator.X(0);

        TestHelpers.AssertProbability(_simulator, 0, 0.0);
        TestHelpers.AssertProbability(_simulator, 1, 1.0);
    }

    [Test]
    public void H_CreatesEqualSuperposition()
    {
        _simulator.H(0);

        TestHelpers.AssertProbability(_simulator, 0, 0.5);
        TestHelpers.AssertProbability(_simulator, 1, 0.5);
    }

    [Test]
    public void Z_DoesNotChangeMeasurementProbabilities()
    {
        _simulator.H(0);
        _simulator.Z(0);

        TestHelpers.AssertProbability(_simulator, 0, 0.5);
        TestHelpers.AssertProbability(_simulator, 1, 0.5);
    }

    [Test]
    public void RY_Pi_MapsZeroToOne()
    {
        _simulator.RY(0, Math.PI);

        TestHelpers.AssertProbability(_simulator, 0, 0.0);
        TestHelpers.AssertProbability(_simulator, 1, 1.0);
    }

    [Test]
    public void H_ThenH_ReturnsToZero()
    {
        _simulator.H(0);
        _simulator.H(0);

        TestHelpers.AssertProbability(_simulator, 0, 1.0);
        TestHelpers.AssertProbability(_simulator, 1, 0.0);
    }
}