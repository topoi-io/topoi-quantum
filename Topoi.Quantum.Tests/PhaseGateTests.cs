using NUnit.Framework;
using System.Numerics;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class PhaseGateTests
{
    QuantumSimulator _simulator;

    [SetUp]
    public void SetUp()
    {
        _simulator = new QuantumSimulator(1);
    }

    [Test]
    public void S_AppliesImaginaryPhaseToOneState()
    {
        _simulator.X(0);
        _simulator.S(0);

        TestHelpers.AssertAmplitude(_simulator, 1, Complex.ImaginaryOne);
    }

    [Test]
    public void T_AppliesPiOverFourPhaseToOneState()
    {
        _simulator.X(0);
        _simulator.T(0);

        Complex expected = Complex.Exp(Complex.ImaginaryOne * Math.PI / 4.0);

        TestHelpers.AssertAmplitude(_simulator, 1, expected);
    }

    [Test]
    public void RZ_Pi_OnZeroAppliesNegativeHalfPhase()
    {
        _simulator.RZ(0, Math.PI);

        Complex expected = Complex.Exp(-Complex.ImaginaryOne * Math.PI / 2.0);

        TestHelpers.AssertAmplitude(_simulator, 0, expected);
    }

    [Test]
    public void Y_MapsZeroToIOne()
    {
        _simulator.Y(0);

        TestHelpers.AssertAmplitude(_simulator, 1, Complex.ImaginaryOne);
    }
}