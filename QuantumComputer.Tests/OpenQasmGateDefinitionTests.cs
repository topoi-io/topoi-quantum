using NUnit.Framework;
using QuantumComputer.Core;
using QuantumComputer.OpenQasm;

namespace QuantumComputer.Tests;

[TestFixture]
public sealed class OpenQasmGateDefinitionTests
{
    [Test]
    public void LoadFromString_CustomBellGate_ExpandsToBuiltInGates()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        gate bell a, b {
            h a;
            cx a, b;
        }

        qubit[2] q;

        bell q[0], q[1];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.QubitCount, Is.EqualTo(2));
        Assert.That(circuit.Operations.Count, Is.EqualTo(2));

        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.H));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));

        Assert.That(circuit.Operations[1].Kind, Is.EqualTo(GateKind.CX));
        Assert.That(circuit.Operations[1].Qubits, Is.EqualTo(new[] { 0, 1 }));
    }

    [Test]
    public void LoadFromString_CustomBellGate_RunCreatesBellState()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        gate bell a, b {
            h a;
            cx a, b;
        }

        qubit[2] q;

        bell q[0], q[1];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        var simulator = new QuantumSimulator(2);
        circuit.Run(simulator);

        double[] probabilities = simulator.Register.Probabilities();

        Assert.That(probabilities[TestHelpers.Basis()], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
        Assert.That(probabilities[TestHelpers.Basis(0, 1)], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void LoadFromString_CustomGate_WithWrongQubitCount_Throws()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        gate bell a, b {
            h a;
            cx a, b;
        }

        qubit[1] q;

        bell q[0];
        """;

        Assert.Throws<OpenQasmParseException>(() =>
            OpenQasmCircuitLoader.LoadFromString(source));
    }

    [Test]
    public void LoadFromString_DuplicateGateDefinition_Throws()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        gate test a {
            h a;
        }

        gate test a {
            x a;
        }

        qubit[1] q;

        test q[0];
        """;

        Assert.Throws<OpenQasmParseException>(() =>
            OpenQasmCircuitLoader.LoadFromString(source));
    }

    [Test]
    public void LoadFromString_RecursiveGateDefinition_Throws()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        gate recurse a {
            recurse a;
        }

        qubit[1] q;

        recurse q[0];
        """;

        Assert.Throws<OpenQasmParseException>(() =>
            OpenQasmCircuitLoader.LoadFromString(source));
    }

    [Test]
    public void LoadFromString_NestedGateDefinition_ExpandsCorrectly()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        gate flip a {
            x a;
        }

        gate doubleflip a {
            flip a;
            flip a;
        }

        qubit[1] q;

        doubleflip q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(2));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.X));
        Assert.That(circuit.Operations[1].Kind, Is.EqualTo(GateKind.X));
    }

    [Test]
    public void LoadFromString_ParameterizedGateDefinition_SubstitutesAngle()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        gate phase(theta) a {
            rz(theta) a;
        }

        qubit[1] q;

        phase(pi / 2) q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.RZ));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
        Assert.That(circuit.Operations[0].Angle, Is.EqualTo(Math.PI / 2).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void LoadFromString_ParameterizedGateDefinition_SubstitutesAngleExpression()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        gate half_phase(theta) a {
            rz(theta / 2) a;
        }

        qubit[1] q;

        half_phase(pi) q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.RZ));
        Assert.That(circuit.Operations[0].Angle, Is.EqualTo(Math.PI / 2).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void LoadFromString_NestedParameterizedGateDefinition_ExpandsCorrectly()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        gate phase(theta) a {
            rz(theta) a;
        }

        gate double_phase(theta) a {
            phase(theta) a;
            phase(theta) a;
        }

        qubit[1] q;

        double_phase(pi / 4) q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(2));

        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.RZ));
        Assert.That(circuit.Operations[0].Angle, Is.EqualTo(Math.PI / 4).Within(TestHelpers.Tolerance));

        Assert.That(circuit.Operations[1].Kind, Is.EqualTo(GateKind.RZ));
        Assert.That(circuit.Operations[1].Angle, Is.EqualTo(Math.PI / 4).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void LoadFromString_ParameterizedGateDefinition_WithWrongParameterCount_Throws()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        gate phase(theta) a {
            rz(theta) a;
        }

        qubit[1] q;

        phase() q[0];
        """;

        Assert.Throws<OpenQasmParseException>(() =>
            OpenQasmCircuitLoader.LoadFromString(source));
    }

    [Test]
    public void LoadFromString_TopLevelGateCall_WithUnknownAngleSymbol_Throws()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        rz(theta) q[0];
        """;

        Assert.Throws<OpenQasmParseException>(() =>
            OpenQasmCircuitLoader.LoadFromString(source));
    }

    [Test]
    public void LoadFromString_TopLevelUnknownAngleSymbol_ReportsSourceLocation()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        rz(theta) q[0];
        """;

        OpenQasmParseException ex = Assert.Throws<OpenQasmParseException>(() =>
            OpenQasmCircuitLoader.LoadFromString(source))!;

        Assert.That(ex.Message, Does.Contain("theta"));
        Assert.That(ex.Line, Is.GreaterThan(1));
        Assert.That(ex.Column, Is.GreaterThan(1));
    }
}