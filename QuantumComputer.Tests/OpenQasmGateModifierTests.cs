using NUnit.Framework;
using QuantumComputer.Core;
using QuantumComputer.OpenQasm;

namespace QuantumComputer.Tests;

[TestFixture]
public sealed class OpenQasmGateModifierTests
{
    [Test]
    public void LoadFromString_CtrlX_MapsToCX()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        ctrl @ x q[0], q[1];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.CX));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0, 1 }));
    }

    [Test]
    public void LoadFromString_CtrlY_MapsToCY()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        ctrl @ y q[0], q[1];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.CY));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0, 1 }));
    }

    [Test]
    public void LoadFromString_CtrlZ_MapsToCZ()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        ctrl @ z q[0], q[1];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.CZ));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0, 1 }));
    }

    [Test]
    public void LoadFromString_CtrlH_MapsToCH()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        ctrl @ h q[0], q[1];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.CH));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0, 1 }));
    }

    [Test]
    public void LoadFromString_CtrlRx_MapsToCRX()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        ctrl @ rx(pi / 2) q[0], q[1];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.CRX));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0, 1 }));
        Assert.That(circuit.Operations[0].Angle, Is.EqualTo(Math.PI / 2).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void LoadFromString_CtrlRy_MapsToCRY()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        ctrl @ ry(pi / 3) q[0], q[1];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.CRY));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0, 1 }));
        Assert.That(circuit.Operations[0].Angle, Is.EqualTo(Math.PI / 3).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void LoadFromString_CtrlRz_MapsToCRZ()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        ctrl @ rz(pi / 4) q[0], q[1];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.CRZ));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0, 1 }));
        Assert.That(circuit.Operations[0].Angle, Is.EqualTo(Math.PI / 4).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void LoadFromString_InvS_MapsToSDG()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        inv @ s q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.SDG));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
    }

    [Test]
    public void LoadFromString_InvSDG_MapsToS()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        inv @ sdg q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.S));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
    }

    [Test]
    public void LoadFromString_InvT_MapsToTDG()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        inv @ t q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.TDG));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
    }

    [Test]
    public void LoadFromString_InvTDG_MapsToT()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        inv @ tdg q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.T));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
    }

    [Test]
    public void LoadFromString_InvSX_MapsToSXDG()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        inv @ sx q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.SXDG));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
    }

    [Test]
    public void LoadFromString_InvSXDG_MapsToSX()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        inv @ sxdg q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.SX));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
    }

    [Test]
    public void LoadFromString_InvX_MapsToX()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        inv @ x q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.X));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
    }

    [Test]
    public void LoadFromString_InvY_MapsToY()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        inv @ y q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.Y));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
    }

    [Test]
    public void LoadFromString_InvZ_MapsToZ()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        inv @ z q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.Z));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
    }

    [Test]
    public void LoadFromString_InvH_MapsToH()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        inv @ h q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.H));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
    }

    [Test]
    public void LoadFromString_InvRx_NegatesAngle()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        inv @ rx(pi / 2) q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.RX));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
        Assert.That(circuit.Operations[0].Angle, Is.EqualTo(-Math.PI / 2).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void LoadFromString_InvRy_NegatesAngle()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        inv @ ry(pi / 3) q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.RY));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
        Assert.That(circuit.Operations[0].Angle, Is.EqualTo(-Math.PI / 3).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void LoadFromString_InvRz_NegatesAngle()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        inv @ rz(pi / 4) q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        Assert.That(circuit.Operations.Count, Is.EqualTo(1));
        Assert.That(circuit.Operations[0].Kind, Is.EqualTo(GateKind.RZ));
        Assert.That(circuit.Operations[0].Qubits, Is.EqualTo(new[] { 0 }));
        Assert.That(circuit.Operations[0].Angle, Is.EqualTo(-Math.PI / 4).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void LoadFromString_CtrlModifier_CreatesBellState()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        h q[0];
        ctrl @ x q[0], q[1];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        var simulator = new QuantumSimulator(2);
        circuit.Run(simulator);

        double[] probabilities = simulator.Register.Probabilities();

        Assert.That(probabilities[TestHelpers.Basis()], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
        Assert.That(probabilities[TestHelpers.Basis(0, 1)], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void LoadFromString_InvS_AfterS_ReturnsOriginalState()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        h q[0];
        s q[0];
        inv @ s q[0];
        """;

        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);

        var simulator = new QuantumSimulator(1);
        circuit.Run(simulator);

        double[] probabilities = simulator.Register.Probabilities();

        Assert.That(probabilities[TestHelpers.Basis()], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
        Assert.That(probabilities[TestHelpers.Basis(0)], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
    }

    [Test]
    public void LoadFromString_CtrlModifier_WithWrongQubitCount_Throws()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        ctrl @ x q[0];
        """;

        Assert.Throws<OpenQasmParseException>(() =>
            OpenQasmCircuitLoader.LoadFromString(source));
    }

    [Test]
    public void LoadFromString_InvModifier_WithWrongQubitCount_Throws()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        inv @ s q[0], q[1];
        """;

        Assert.Throws<OpenQasmParseException>(() =>
            OpenQasmCircuitLoader.LoadFromString(source));
    }

    [Test]
    public void LoadFromString_CtrlModifier_OnUnsupportedGate_Throws()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[3] q;

        ctrl @ ccx q[0], q[1], q[2];
        """;

        Assert.Throws<OpenQasmParseException>(() =>
            OpenQasmCircuitLoader.LoadFromString(source));
    }

    [Test]
    public void LoadFromString_InvModifier_OnUnsupportedGate_Throws()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        inv @ cx q[0], q[1];
        """;

        Assert.Throws<OpenQasmParseException>(() =>
            OpenQasmCircuitLoader.LoadFromString(source));
    }
}