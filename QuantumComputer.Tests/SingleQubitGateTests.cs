using NUnit.Framework;
using QuantumComputer.Core;

namespace QuantumComputer.Tests;

[TestFixture]
public sealed class SingleQubitGateTests
{
    [SetUp]
    public void SetUp()
    {
        Quantum.Init(1);
        Quantum.Reset();
    }

    [Test]
    public void Reset_PreparesZeroState()
    {
        TestHelpers.AssertProbability(0, 1.0);
        TestHelpers.AssertProbability(1, 0.0);
    }

    [Test]
    public void X_FlipsZeroToOne()
    {
        Quantum.X(0);

        TestHelpers.AssertProbability(0, 0.0);
        TestHelpers.AssertProbability(1, 1.0);
    }

    [Test]
    public void H_CreatesEqualSuperposition()
    {
        Quantum.H(0);

        TestHelpers.AssertProbability(0, 0.5);
        TestHelpers.AssertProbability(1, 0.5);
    }

    [Test]
    public void Z_DoesNotChangeMeasurementProbabilities()
    {
        Quantum.H(0);
        Quantum.Z(0);

        TestHelpers.AssertProbability(0, 0.5);
        TestHelpers.AssertProbability(1, 0.5);
    }

    [Test]
    public void RY_Pi_MapsZeroToOne()
    {
        Quantum.RY(0, Math.PI);

        TestHelpers.AssertProbability(0, 0.0);
        TestHelpers.AssertProbability(1, 1.0);
    }

    [Test]
    public void H_ThenH_ReturnsToZero()
    {
        Quantum.H(0);
        Quantum.H(0);

        TestHelpers.AssertProbability(0, 1.0);
        TestHelpers.AssertProbability(1, 0.0);
    }
}