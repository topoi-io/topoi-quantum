using NUnit.Framework;

namespace QuantumComputer.Tests;

[TestFixture]
public sealed class EntanglementTests
{
    [Test]
    public void CX_CreatesBellState()
    {
        Quantum.Init(2);
        Quantum.Reset();

        Quantum.H(0);
        Quantum.CX(0, 1);

        TestHelpers.AssertProbability(TestHelpers.Basis(), 0.5);       // |00>
        TestHelpers.AssertProbability(TestHelpers.Basis(0), 0.0);      // |01>
        TestHelpers.AssertProbability(TestHelpers.Basis(1), 0.0);      // |10>
        TestHelpers.AssertProbability(TestHelpers.Basis(0, 1), 0.5);   // |11>
    }

    [Test]
    public void BellState_HasExpectedCorrelations()
    {
        Quantum.Init(2);
        Quantum.Reset();

        Quantum.H(0);
        Quantum.CX(0, 1);

        TestHelpers.AssertExpectation(0.0, new PauliTerm('Z', 0));
        TestHelpers.AssertExpectation(0.0, new PauliTerm('Z', 1));
        TestHelpers.AssertExpectation(1.0, new PauliTerm('Z', 0), new PauliTerm('Z', 1));
        TestHelpers.AssertExpectation(1.0, new PauliTerm('X', 0), new PauliTerm('X', 1));
    }

    [Test]
    public void GHZState_HasExpectedProbabilities()
    {
        Quantum.Init(3);
        Quantum.Reset();

        Quantum.H(0);
        Quantum.CX(0, 1);
        Quantum.CX(1, 2);

        TestHelpers.AssertProbability(TestHelpers.Basis(), 0.5);          // |000>
        TestHelpers.AssertProbability(TestHelpers.Basis(0, 1, 2), 0.5);   // |111>

        TestHelpers.AssertProbability(TestHelpers.Basis(0), 0.0);
        TestHelpers.AssertProbability(TestHelpers.Basis(1), 0.0);
        TestHelpers.AssertProbability(TestHelpers.Basis(2), 0.0);
    }

    [Test]
    public void GHZState_HasExpectedCorrelations()
    {
        Quantum.Init(3);
        Quantum.Reset();

        Quantum.H(0);
        Quantum.CX(0, 1);
        Quantum.CX(1, 2);

        TestHelpers.AssertExpectation(1.0, new PauliTerm('Z', 0), new PauliTerm('Z', 1));
        TestHelpers.AssertExpectation(1.0, new PauliTerm('Z', 1), new PauliTerm('Z', 2));
        TestHelpers.AssertExpectation(1.0, new PauliTerm('Z', 0), new PauliTerm('Z', 2));
        TestHelpers.AssertExpectation(1.0, new PauliTerm('X', 0), new PauliTerm('X', 1), new PauliTerm('X', 2));
    }

    [Test]
    public void SWAP_ExchangesTwoQubits()
    {
        Quantum.Init(2);
        Quantum.Reset();

        Quantum.X(0);

        TestHelpers.AssertProbability(TestHelpers.Basis(0), 1.0); // |01>

        Quantum.SWAP(0, 1);

        TestHelpers.AssertProbability(TestHelpers.Basis(1), 1.0); // |10>
    }

    [Test]
    public void CCX_FlipsTargetOnlyWhenBothControlsAreOne()
    {
        Quantum.Init(3);
        Quantum.Reset();

        Quantum.X(0);
        Quantum.X(1);
        Quantum.CCX(0, 1, 2);

        TestHelpers.AssertProbability(TestHelpers.Basis(0, 1, 2), 1.0); // |111>
    }

    [Test]
    public void CCX_DoesNotFlipTargetWhenOneControlIsZero()
    {
        Quantum.Init(3);
        Quantum.Reset();

        Quantum.X(0);
        Quantum.CCX(0, 1, 2);

        TestHelpers.AssertProbability(TestHelpers.Basis(0), 1.0); // |001>
    }
}