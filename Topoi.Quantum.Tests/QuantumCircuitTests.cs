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
        QuantumCircuit circuit = QuantumCircuitBuilder
            .WithQubits(2)
            .H(0)
            .CX(0, 1)
            .Build();

        var simulator = new QuantumSimulator(circuit.QubitCount);

        circuit.Run(simulator);

        double[] probabilities = simulator.Register.Probabilities();

        Assert.That(probabilities[TestHelpers.Basis()], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
        Assert.That(probabilities[TestHelpers.Basis(0, 1)], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void Circuit_Run_CreatesGHZState()
    {
        QuantumCircuit circuit = QuantumCircuitBuilder
            .WithQubits(3)
            .H(0)
            .CX(0, 1)
            .CX(1, 2)
            .Build();

        var simulator = new QuantumSimulator(circuit.QubitCount);

        circuit.Run(simulator);

        double[] probabilities = simulator.Register.Probabilities();

        Assert.That(
            probabilities[TestHelpers.Basis()],
            Is.EqualTo(0.5).Within(TestHelpers.Tolerance));

        Assert.That(
            probabilities[TestHelpers.Basis(0, 1, 2)],
            Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
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

        circuit.Run(Quantum.DefaultSimulator, resetFirst: false);

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