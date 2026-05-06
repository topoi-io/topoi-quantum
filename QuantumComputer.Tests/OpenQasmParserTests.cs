using NUnit.Framework;
using QuantumComputer.Core;
using QuantumComputer.OpenQasm;

namespace QuantumComputer.Tests;

[TestFixture]
public sealed class OpenQasmParserTests
{
    [Test]
    public void LoadFromString_BellCircuit_CreatesExpectedCircuit()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        h q[0];
        cx q[0], q[1];
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
    public void LoadFromString_BellCircuit_RunCreatesBellState()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        h q[0];
        cx q[0], q[1];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        var simulator = new QuantumSimulator(2);
        circuit.Run(simulator);

        double[] probabilities = simulator.Register.Probabilities();

        Assert.That(probabilities[TestHelpers.Basis()], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
        Assert.That(probabilities[TestHelpers.Basis(0, 1)], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void LoadFromString_RotationGate_ParsesPiExpression()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        ry(pi) q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.RY));
        Assert.That(circuit.Operations[0].Angle, Is.EqualTo(Math.PI).Within(1e-12));
    }

    [Test]
    public void LoadFromString_GHZCircuit_RunCreatesGHZState()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[3] q;

        h q[0];
        cx q[0], q[1];
        cx q[1], q[2];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        var simulator = new QuantumSimulator(3);
        circuit.Run(simulator);

        double[] probabilities = simulator.Register.Probabilities();

        Assert.That(probabilities[TestHelpers.Basis()], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
        Assert.That(probabilities[TestHelpers.Basis(0, 1, 2)], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void LoadFromString_SXGate_IsSupported()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        sx q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.QubitCount, Is.EqualTo(1));
        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.SX));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
    }

    [Test]
    public void LoadFromString_UnsupportedGate_Throws()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        u3(pi / 2, 0, pi) q[0];
        """;

        Assert.Throws<OpenQasmParseException>(() =>
            OpenQasmCircuitLoader.LoadFromString(source));
    }
}