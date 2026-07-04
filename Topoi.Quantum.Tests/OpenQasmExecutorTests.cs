using NUnit.Framework;

using Topoi.Quantum.OpenQasm;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class OpenQasmExecutorTests
{
    [Test]
    public void Execute_Measurement_WritesClassicalBit()
    {
        const string source = """
        OPENQASM 3.1;
        include "stdgates.inc";

        qubit[1] q;
        bit[1] c;

        x q[0];
        c[0] = measure q[0];
        """;

        OpenQasmExecutableProgram program =
            OpenQasmCircuitLoader.LoadExecutableFromString(source);

        OpenQasmExecutionResult result =
            OpenQasmExecutor.Execute(program);

        Assert.That(result.ClassicalBits[0], Is.EqualTo(1));
        TestHelpers.AssertProbability(result.Simulator, TestHelpers.Basis(0), 1.0);
    }

    [Test]
    public void Execute_Reset_ResetsMeasuredOneToZero()
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

        OpenQasmExecutionResult result =
            OpenQasmExecutor.Execute(program);

        TestHelpers.AssertProbability(result.Simulator, 0, 1.0);
    }

    [Test]
    public void Execute_Barrier_IsNoOp()
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

        OpenQasmExecutionResult result =
            OpenQasmExecutor.Execute(program);

        TestHelpers.AssertProbability(result.Simulator, 0, 0.5);
        TestHelpers.AssertProbability(result.Simulator, TestHelpers.Basis(0, 1), 0.5);
    }
}