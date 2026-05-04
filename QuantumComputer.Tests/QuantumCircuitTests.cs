using NUnit.Framework;

namespace QuantumComputer.Tests;

[TestFixture]
public sealed class QuantumCircuitTests
{
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
}