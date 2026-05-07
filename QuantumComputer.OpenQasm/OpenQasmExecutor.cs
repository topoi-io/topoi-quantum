using QuantumComputer.Core;

namespace QuantumComputer.OpenQasm;

public static class OpenQasmExecutor
{
    public static OpenQasmExecutionResult Execute(
        OpenQasmExecutableProgram program,
        IRandomSource? randomSource = null)
    {
        if (program is null)
            throw new ArgumentNullException(nameof(program));

        var simulator = new QuantumSimulator(program.QubitCount, randomSource);
        int classicalBitCount = program.BitRegisters.Values.Sum(r => r.Size);
        int[] classicalBits = new int[classicalBitCount];

        foreach (OpenQasmExecutableOperation operation in program.Operations)
        {
            switch (operation)
            {
                case OpenQasmGateOperation gate:
                    gate.Operation.Apply(simulator);
                    break;

                case OpenQasmMeasureOperation measure:
                    classicalBits[measure.Bit] = simulator.Measure(measure.Qubit);
                    break;

                case OpenQasmResetOperation reset:
                    ResetQubitToZero(simulator, reset.Qubit);
                    break;

                case OpenQasmBarrierOperation:
                    // No-op in this simulator.
                    break;

                default:
                    throw new NotSupportedException($"Unsupported operation: {operation.GetType().Name}");
            }
        }

        return new OpenQasmExecutionResult(simulator, classicalBits);
    }

    private static void ResetQubitToZero(QuantumSimulator simulator, int qubit)
    {
        int measured = simulator.Measure(qubit);

        if (measured == 1)
            simulator.X(qubit);
    }
}