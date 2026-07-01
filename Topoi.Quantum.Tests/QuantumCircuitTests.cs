using NUnit.Framework;
using Topoi.Quantum.Cli;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class QuantumCircuitTests
{
    [Test]
    public void Circuit_Add_RejectsWrongQubitCount()
    {
        var circuit = new QuantumCircuit(2);

        Assert.Throws<ArgumentException>(() =>
            circuit.Add(new GateOperation(GateKind.CX, new[] { 0 })));

        Assert.Throws<ArgumentException>(() =>
            circuit.Add(new GateOperation(GateKind.X, new[] { 0, 1 })));
    }

    [Test]
    public void Circuit_Add_RejectsMissingAngleForRotationGate()
    {
        var circuit = new QuantumCircuit(1);

        Assert.Throws<ArgumentException>(() =>
            circuit.Add(new GateOperation(GateKind.RX, new[] { 0 })));
    }

    [Test]
    public void Circuit_Add_RejectsAngleForNonRotationGate()
    {
        var circuit = new QuantumCircuit(1);

        Assert.Throws<ArgumentException>(() =>
            circuit.Add(new GateOperation(GateKind.H, new[] { 0 }, Math.PI)));
    }

    [Test]
    public void Circuit_Run_CreatesBellState()
    {
        var circuit = new QuantumCircuit(2);

        circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));
        circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 1 }));

        circuit.Run();

        TestHelpers.AssertProbability(TestHelpers.Basis(), 0.5);
        TestHelpers.AssertProbability(TestHelpers.Basis(0, 1), 0.5);

        TestHelpers.AssertExpectation(
            1.0,
            new PauliTerm('Z', 0),
            new PauliTerm('Z', 1));

        TestHelpers.AssertExpectation(
            1.0,
            new PauliTerm('X', 0),
            new PauliTerm('X', 1));
    }

    [Test]
    public void Circuit_Run_CreatesGHZState()
    {
        var circuit = new QuantumCircuit(3);

        circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));
        circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 1 }));
        circuit.Add(new GateOperation(GateKind.CX, new[] { 1, 2 }));

        circuit.Run();

        TestHelpers.AssertProbability(TestHelpers.Basis(), 0.5);
        TestHelpers.AssertProbability(TestHelpers.Basis(0, 1, 2), 0.5);

        TestHelpers.AssertExpectation(
            1.0,
            new PauliTerm('Z', 0),
            new PauliTerm('Z', 1));

        TestHelpers.AssertExpectation(
            1.0,
            new PauliTerm('Z', 1),
            new PauliTerm('Z', 2));

        TestHelpers.AssertExpectation(
            1.0,
            new PauliTerm('X', 0),
            new PauliTerm('X', 1),
            new PauliTerm('X', 2));
    }

    [Test]
    public void Circuit_Add_RejectsOutOfRangeQubit()
    {
        var circuit = new QuantumCircuit(2);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            circuit.Add(new GateOperation(GateKind.H, new[] { 2 })));
    }

    [Test]
    public void Circuit_Add_RejectsDuplicateQubits()
    {
        var circuit = new QuantumCircuit(2);

        Assert.Throws<ArgumentException>(() =>
            circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 0 })));
    }

    [Test]
    public void Circuit_Run_WithoutReset_ComposesWithExistingState()
    {
        Quantum.Init(1);
        Quantum.Reset();

        Quantum.X(0);

        var circuit = new QuantumCircuit(1);
        circuit.Add(new GateOperation(GateKind.X, new[] { 0 }));

        circuit.Run(resetFirst: false);

        TestHelpers.AssertProbability(0, 1.0);
        TestHelpers.AssertProbability(1, 0.0);
    }

    [Test]
    public void FormatCircuit_WithBellCircuit_ReturnsOperationList()
    {
        QuantumCircuit circuit = QuantumCircuitBuilder
            .WithQubits(2)
            .H(0)
            .CX(0, 1)
            .Build();

        string text = QuantumConsolePrinter.FormatCircuit(circuit);

        Assert.That(text, Does.Contain("Circuit: 2 qubits, 2 operations"));
        Assert.That(text, Does.Contain("0: H 0"));
        Assert.That(text, Does.Contain("1: CX 0 1"));
    }

    [Test]
    public void FormatCircuit_WithEmptyCircuit_PrintsEmpty()
    {
        var circuit = new QuantumCircuit(2);

        string text = QuantumConsolePrinter.FormatCircuit(circuit);

        Assert.That(text, Does.Contain("Circuit: 2 qubits, 0 operations"));
        Assert.That(text, Does.Contain("(empty)"));
    }
}