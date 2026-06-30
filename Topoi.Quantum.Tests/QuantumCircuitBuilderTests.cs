using NUnit.Framework;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class QuantumCircuitBuilderTests
{
    [Test]
    public void Builder_BellCircuit_CreatesExpectedOperations()
    {
        QuantumCircuit circuit = QuantumCircuitBuilder
            .WithQubits(2)
            .H(0)
            .CX(0, 1)
            .Build();

        Assert.That(circuit.QubitCount, Is.EqualTo(2));
        Assert.That(circuit.Operations.Count, Is.EqualTo(2));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.H));
        Assert.That(circuit.Operations[1].Kind, Is.EqualTo(GateKind.CX));
    }

    [Test]
    public void Builder_Rotation_AddsAngle()
    {
        QuantumCircuit circuit = QuantumCircuitBuilder
            .WithQubits(1)
            .RY(0, Math.PI / 2)
            .Build();

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.RY));
        Assert.That(circuit.Operations[0].Angle, Is.EqualTo(Math.PI / 2).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void QuantumToolkit_Circuit_CreatesBuilder()
    {
        QuantumCircuit circuit = QuantumToolkit
            .Circuit(1)
            .H(0)
            .Build();

        Assert.That(circuit.QubitCount, Is.EqualTo(1));
        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
    }
}