using NUnit.Framework;

using Topoi.Quantum.Drawing;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class CircuitDrawerTests
{
    [Test]
    public void Draw_BellCircuit_ContainsExpectedSymbols()
    {
        var circuit = new QuantumCircuit(2);

        circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));
        circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 1 }));

        string drawing = CircuitDrawer.Draw(circuit);

        Assert.That(drawing, Does.Contain("q0:"));
        Assert.That(drawing, Does.Contain("q1:"));
        Assert.That(drawing, Does.Contain("H"));
        Assert.That(drawing, Does.Contain("●"));
        Assert.That(drawing, Does.Contain("X"));
        Assert.That(drawing, Does.Contain("│"));
    }

    [Test]
    public void Draw_BellCircuit_VerticalConnectorAlignsWithControlAndTarget()
    {
        var circuit = new QuantumCircuit(2);

        circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));
        circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 1 }));

        string[] lines = CircuitDrawer.Draw(circuit)
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        int controlColumn = lines[0].IndexOf('●');
        int verticalColumn = lines[1].IndexOf('│');
        int targetColumn = lines[2].IndexOf('X');

        Assert.That(controlColumn, Is.EqualTo(verticalColumn));
        Assert.That(targetColumn, Is.EqualTo(verticalColumn));
    }

    [Test]
    public void Draw_GHZCircuit_ContainsThreeQubitLines()
    {
        var circuit = new QuantumCircuit(3);

        circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));
        circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 1 }));
        circuit.Add(new GateOperation(GateKind.CX, new[] { 1, 2 }));

        string drawing = CircuitDrawer.Draw(circuit);

        Assert.That(drawing, Does.Contain("q0:"));
        Assert.That(drawing, Does.Contain("q1:"));
        Assert.That(drawing, Does.Contain("q2:"));
    }

    [Test]
    public void Draw_ToffoliCircuit_ContainsTwoControlsAndTarget()
    {
        var circuit = new QuantumCircuit(3);

        circuit.Add(new GateOperation(GateKind.CCX, new[] { 0, 1, 2 }));

        string drawing = CircuitDrawer.Draw(circuit);

        int controls = drawing.Count(c => c == '●');
        int targets = drawing.Count(c => c == 'X');

        Assert.That(controls, Is.EqualTo(2));
        Assert.That(targets, Is.EqualTo(1));
    }

    [Test]
    public void Draw_SwapCircuit_ContainsSwapSymbols()
    {
        var circuit = new QuantumCircuit(2);

        circuit.Add(new GateOperation(GateKind.SWAP, new[] { 0, 1 }));

        string drawing = CircuitDrawer.Draw(circuit);

        int swaps = drawing.Count(c => c == '×');

        Assert.That(swaps, Is.EqualTo(2));
        Assert.That(drawing, Does.Contain("│"));
    }
}