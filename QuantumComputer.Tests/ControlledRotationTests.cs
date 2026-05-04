using NUnit.Framework;

namespace QuantumComputer.Tests;

[TestFixture]
public sealed class ControlledRotationTests
{
    [Test]
    public void CRY_DoesNothingWhenControlIsZero()
    {
        Quantum.Init(2);
        Quantum.Reset();

        Quantum.CRY(0, 1, Math.PI);

        TestHelpers.AssertProbability(TestHelpers.Basis(), 1.0);
    }

    [Test]
    public void CRY_Pi_RotatesTargetWhenControlIsOne()
    {
        Quantum.Init(2);
        Quantum.Reset();

        Quantum.X(0);
        Quantum.CRY(0, 1, Math.PI);

        TestHelpers.AssertProbability(TestHelpers.Basis(0, 1), 1.0);
    }

    [Test]
    public void CRX_DoesNothingWhenControlIsZero()
    {
        Quantum.Init(2);
        Quantum.Reset();

        Quantum.CRX(0, 1, Math.PI);

        TestHelpers.AssertProbability(TestHelpers.Basis(), 1.0);
    }

    [Test]
    public void CRZ_DoesNotChangeComputationalBasisProbability()
    {
        Quantum.Init(2);
        Quantum.Reset();

        Quantum.X(0);
        Quantum.X(1);

        Quantum.CRZ(0, 1, Math.PI);

        TestHelpers.AssertProbability(TestHelpers.Basis(0, 1), 1.0);
    }
}