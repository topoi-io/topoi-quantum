using NUnit.Framework;
using System.Numerics;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class AdditionalStandardGateTests
{
    [Test]
    public void SDG_UndoesS()
    {
        var simulator = new QuantumSimulator(1);

        simulator.X(0);
        simulator.S(0);
        simulator.SDG(0);

        TestHelpers.AssertAmplitude(simulator, 1, Complex.One);
    }

    [Test]
    public void TDG_UndoesT()
    {
        var simulator = new QuantumSimulator(1);

        simulator.X(0);
        simulator.T(0);
        simulator.TDG(0);

        TestHelpers.AssertAmplitude(simulator, 1, Complex.One);
    }

    [Test]
    public void SX_Twice_EqualsX()
    {
        var simulator = new QuantumSimulator(1);

        simulator.SX(0);
        simulator.SX(0);

        TestHelpers.AssertProbability(simulator, 1, 1.0);
    }

    [Test]
    public void SXDG_UndoesSX()
    {
        var simulator = new QuantumSimulator(1);

        simulator.SX(0);
        simulator.SXDG(0);

        TestHelpers.AssertProbability(simulator, 0, 1.0);
    }

    [Test]
    public void CY_AppliesYWhenControlIsOne()
    {
        var simulator = new QuantumSimulator(2);

        simulator.X(0);
        simulator.CY(0, 1);

        TestHelpers.AssertProbability(
            simulator,
            TestHelpers.Basis(0, 1),
            1.0);
    }

    [Test]
    public void CH_AppliesHadamardWhenControlIsOne()
    {
        var simulator = new QuantumSimulator(2);

        simulator.X(0);
        simulator.CH(0, 1);

        TestHelpers.AssertProbability(
            simulator,
            TestHelpers.Basis(0),
            0.5);

        TestHelpers.AssertProbability(
            simulator,
            TestHelpers.Basis(0, 1),
            0.5);
    }

    [Test]
    public void CP_Pi_EqualsCZOnBasisState()
    {
        var simulator = new QuantumSimulator(2);

        simulator.X(0);
        simulator.X(1);
        simulator.CP(0, 1, Math.PI);

        TestHelpers.AssertAmplitude(
            simulator,
            TestHelpers.Basis(0, 1),
            -Complex.One);
    }
}