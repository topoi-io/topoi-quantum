using NUnit.Framework;
using QuantumComputer.Core;
using System.Numerics;

namespace QuantumComputer.Tests;

[TestFixture]
public sealed class PhaseGateTests
{
    [SetUp]
    public void SetUp()
    {
        Quantum.Init(1);
        Quantum.Reset();
    }

    [Test]
    public void S_AppliesImaginaryPhaseToOneState()
    {
        Quantum.X(0);
        Quantum.S(0);

        TestHelpers.AssertAmplitude(1, Complex.ImaginaryOne);
    }

    [Test]
    public void T_AppliesPiOverFourPhaseToOneState()
    {
        Quantum.X(0);
        Quantum.T(0);

        Complex expected = Complex.Exp(Complex.ImaginaryOne * Math.PI / 4.0);

        TestHelpers.AssertAmplitude(1, expected);
    }

    [Test]
    public void RZ_Pi_OnZeroAppliesNegativeHalfPhase()
    {
        Quantum.RZ(0, Math.PI);

        Complex expected = Complex.Exp(-Complex.ImaginaryOne * Math.PI / 2.0);

        TestHelpers.AssertAmplitude(0, expected);
    }

    [Test]
    public void Y_MapsZeroToIOne()
    {
        Quantum.Y(0);

        TestHelpers.AssertAmplitude(1, Complex.ImaginaryOne);
    }
}