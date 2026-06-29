using NUnit.Framework;
using QuantumComputer.Core;
using QuantumComputer.OpenQasm;

namespace QuantumComputer.Tests;

[TestFixture]
public sealed class OpenQasmExporterTests
{
    [Test]
    public void Export_ExecutableProgram_WithMeasurement_ExportsBitDeclarationAndMeasurement()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;
        bit[1] c;

        h q[0];
        c[0] = measure q[0];
        """;

        OpenQasmExecutableProgram program =
            OpenQasmCircuitLoader.LoadExecutableFromString(source);

        string exported = OpenQasmExporter.Export(program);

        Assert.That(exported, Does.Contain("qubit[1] q;"));
        Assert.That(exported, Does.Contain("bit[1] c;"));
        Assert.That(exported, Does.Contain("h q[0];"));
        Assert.That(exported, Does.Contain("c[0] = measure q[0];"));
    }

    [Test]
    public void Export_ExecutableProgram_WithReset_ExportsReset()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;

        x q[0];
        reset q[0];
        """;

        OpenQasmExecutableProgram program =
            OpenQasmCircuitLoader.LoadExecutableFromString(source);

        string exported = OpenQasmExporter.Export(program);

        Assert.That(exported, Does.Contain("reset q[0];"));
    }

    [Test]
    public void Export_ExecutableProgram_WithBarrier_ExportsBarrier()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[2] q;

        h q[0];
        barrier q[0], q[1];
        cx q[0], q[1];
        """;

        OpenQasmExecutableProgram program =
            OpenQasmCircuitLoader.LoadExecutableFromString(source);

        string exported = OpenQasmExporter.Export(program);

        Assert.That(exported, Does.Contain("barrier q[0], q[1];"));
    }
}
