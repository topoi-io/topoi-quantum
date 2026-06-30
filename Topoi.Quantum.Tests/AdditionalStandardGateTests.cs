using NUnit.Framework;
using System.Numerics;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class AdditionalStandardGateTests
{
    [Test]
    public void SDG_UndoesS()
    {
        Quantum.Init(1);
        Quantum.Reset();

        Quantum.X(0);
        Quantum.S(0);
        Quantum.SDG(0);

        TestHelpers.AssertAmplitude(1, Complex.One);
    }

    [Test]
    public void TDG_UndoesT()
    {
        Quantum.Init(1);
        Quantum.Reset();

        Quantum.X(0);
        Quantum.T(0);
        Quantum.TDG(0);

        TestHelpers.AssertAmplitude(1, Complex.One);
    }

    [Test]
    public void SX_Twice_EqualsX()
    {
        Quantum.Init(1);
        Quantum.Reset();

        Quantum.SX(0);
        Quantum.SX(0);

        TestHelpers.AssertProbability(1, 1.0);
    }

    [Test]
    public void SXDG_UndoesSX()
    {
        Quantum.Init(1);
        Quantum.Reset();

        Quantum.SX(0);
        Quantum.SXDG(0);

        TestHelpers.AssertProbability(0, 1.0);
    }

    [Test]
    public void CY_AppliesYWhenControlIsOne()
    {
        Quantum.Init(2);
        Quantum.Reset();

        Quantum.X(0);
        Quantum.CY(0, 1);

        TestHelpers.AssertProbability(TestHelpers.Basis(0, 1), 1.0);
    }

    [Test]
    public void CH_AppliesHadamardWhenControlIsOne()
    {
        Quantum.Init(2);
        Quantum.Reset();

        Quantum.X(0);
        Quantum.CH(0, 1);

        TestHelpers.AssertProbability(TestHelpers.Basis(0), 0.5);
        TestHelpers.AssertProbability(TestHelpers.Basis(0, 1), 0.5);
    }

    [Test]
    public void CP_Pi_EqualsCZOnBasisState()
    {
        Quantum.Init(2);
        Quantum.Reset();

        Quantum.X(0);
        Quantum.X(1);
        Quantum.CP(0, 1, Math.PI);

        TestHelpers.AssertAmplitude(TestHelpers.Basis(0, 1), -Complex.One);
    }
}