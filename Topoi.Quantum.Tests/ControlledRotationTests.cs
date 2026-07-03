using NUnit.Framework;
using System.Numerics;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class ControlledRotationTests
{
    [Test]
    public void CRY_DoesNothingWhenControlIsZero()
    {
        var simulator = new QuantumSimulator(2);
        simulator.CRY(0, 1, Math.PI);

        TestHelpers.AssertAmplitude(simulator, TestHelpers.Basis(), Complex.One);
    }

    [Test]
    public void CRY_Pi_RotatesTargetWhenControlIsOne()
    {
        var simulator = new QuantumSimulator(2);
        simulator.X(0);
        simulator.CRY(0, 1, Math.PI);

        TestHelpers.AssertAmplitude(simulator, TestHelpers.Basis(0, 1), Complex.One);
    }

    [Test]
    public void CRX_DoesNothingWhenControlIsZero()
    {
        var simulator = new QuantumSimulator(2);
        simulator.CRX(0, 1, Math.PI);

        TestHelpers.AssertAmplitude(simulator, TestHelpers.Basis(), Complex.One);
    }

    [Test]
    public void CRZ_DoesNotChangeComputationalBasisProbability()
    {
        var simulator = new QuantumSimulator(2);
        simulator.X(0);
        simulator.X(1);
        simulator.CRZ(0, 1, Math.PI);

        TestHelpers.AssertProbability(
        simulator,
        TestHelpers.Basis(0, 1),
        1.0);
    }
}