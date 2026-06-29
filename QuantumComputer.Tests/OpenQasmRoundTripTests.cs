using NUnit.Framework;
using QuantumComputer.Core;
using QuantumComputer.OpenQasm;

namespace QuantumComputer.Tests;

[TestFixture]
public sealed class OpenQasmRoundTripTests
{
    [Test]
    public void RoundTrip_BellCircuit_PreservesOperations()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        h q[0];
        cx q[0], q[1];
        """;

        OpenQasmExecutableProgram result = RoundTrip(source);

        Assert.That(result.QubitCount, Is.EqualTo(2));
        Assert.That(result.Operations.Count, Is.EqualTo(2));

        var gate0 = (OpenQasmGateOperation)result.Operations[0];
        var gate1 = (OpenQasmGateOperation)result.Operations[1];

        Assert.That(gate0.Operation.Kind, Is.EqualTo(GateKind.H));
        Assert.That(gate1.Operation.Kind, Is.EqualTo(GateKind.CX));
    }

    [Test]
    public void RoundTrip_MeasurementCircuit_PreservesMeasurement()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;
        bit[1] c;

        h q[0];
        c[0] = measure q[0];
        """;

        OpenQasmExecutableProgram result = RoundTrip(source);

        Assert.That(result.QubitCount, Is.EqualTo(1));
        Assert.That(result.BitRegisters.Single().Value.Size, Is.EqualTo(1));
        Assert.That(result.Operations.OfType<OpenQasmMeasureOperation>().Count(), Is.EqualTo(1));
    }

    [Test]
    public void RoundTrip_ResetCircuit_PreservesReset()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        x q[0];
        reset q[0];
        """;

        OpenQasmExecutableProgram result = RoundTrip(source);

        Assert.That(result.Operations.OfType<OpenQasmResetOperation>().Count(), Is.EqualTo(1));
    }

    [Test]
    public void RoundTrip_BarrierCircuit_PreservesBarrier()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        h q[0];
        barrier q[0], q[1];
        cx q[0], q[1];
        """;

        OpenQasmExecutableProgram result = RoundTrip(source);

        Assert.That(result.Operations.OfType<OpenQasmBarrierOperation>().Count(), Is.EqualTo(1));
    }

    [Test]
    public void RoundTrip_ParameterizedCustomGate_ExportsExpandedGate()
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

        OpenQasmExecutableProgram result = RoundTrip(source);

        OpenQasmGateOperation gate =
            result.Operations.OfType<OpenQasmGateOperation>().Single();

        Assert.That(gate.Operation.Kind, Is.EqualTo(GateKind.RZ));
        Assert.That(gate.Operation.Angle, Is.EqualTo(Math.PI / 2).Within(TestHelpers.Tolerance));
    }

    private static OpenQasmExecutableProgram RoundTrip(string source)
    {
        OpenQasmExecutableProgram first =
            OpenQasmCircuitLoader.LoadExecutableFromString(source);

        string exported = OpenQasmExporter.Export(first);

        return OpenQasmCircuitLoader.LoadExecutableFromString(exported);
    }
}
