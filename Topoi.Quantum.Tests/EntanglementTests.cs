using NUnit.Framework;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class EntanglementTests
{
    [Test]
    public void CX_CreatesBellState()
    {
        var simulator = new QuantumSimulator(2);
        simulator.H(0);
        simulator.CX(0, 1);

        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(), 0.5);
        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(0), 0.0);
        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(1), 0.0);
        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(0, 1), 0.5);
    }

    [Test]
    public void BellState_HasExpectedCorrelations()
    {
        var simulator = new QuantumSimulator(2);
        simulator.H(0);
        simulator.CX(0, 1);

        TestHelpers.AssertExpectation(
            simulator,
            0.0,
            new PauliTerm('Z', 0));

        TestHelpers.AssertExpectation(
            simulator,
            0.0,
            new PauliTerm('Z', 1));

        TestHelpers.AssertExpectation(
            simulator,
            1.0,
            new PauliTerm('Z', 0),
            new PauliTerm('Z', 1));

        TestHelpers.AssertExpectation(
            simulator,
            1.0,
            new PauliTerm('X', 0),
            new PauliTerm('X', 1));
    }

    [Test]
    public void GHZState_HasExpectedProbabilities()
    {
        var simulator = new QuantumSimulator(3);
        simulator.H(0);
        simulator.CX(0, 1);
        simulator.CX(1, 2);

        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(), 0.5);
        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(0, 1, 2), 0.5);

        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(0), 0.0);
        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(1), 0.0);
        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(2), 0.0);
    }

    [Test]
    public void GHZState_HasExpectedCorrelations()
    {
        var simulator = new QuantumSimulator(3);

        simulator.H(0);
        simulator.CX(0, 1);
        simulator.CX(1, 2);

        TestHelpers.AssertExpectation(
            simulator,
            1.0,
            new PauliTerm('Z', 0),
            new PauliTerm('Z', 1));

        TestHelpers.AssertExpectation(
            simulator,
            1.0,
            new PauliTerm('Z', 1),
            new PauliTerm('Z', 2));

        TestHelpers.AssertExpectation(
            simulator,
            1.0,
            new PauliTerm('Z', 0),
            new PauliTerm('Z', 2));

        TestHelpers.AssertExpectation(
            simulator,
            1.0,
            new PauliTerm('X', 0),
            new PauliTerm('X', 1),
            new PauliTerm('X', 2));
    }

    [Test]
    public void SWAP_ExchangesTwoQubits()
    {
        var simulator = new QuantumSimulator(2);
        simulator.X(0);

        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(0), 1.0);

        simulator.SWAP(0, 1);

        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(1), 1.0);
    }

    [Test]
    public void CCX_FlipsTargetOnlyWhenBothControlsAreOne()
    {
        var simulator = new QuantumSimulator(3);
        simulator.X(0);
        simulator.X(1);
        simulator.CCX(0, 1, 2);

        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(0, 1, 2), 1.0);
    }

    [Test]
    public void CCX_DoesNotFlipTargetWhenOneControlIsZero()
    {
        var simulator = new QuantumSimulator(3);
        simulator.X(0);
        simulator.CCX(0, 1, 2);

        TestHelpers.AssertProbability(simulator, TestHelpers.Basis(0), 1.0);
    }
}