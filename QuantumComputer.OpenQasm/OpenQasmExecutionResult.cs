using QuantumComputer.Core;

namespace QuantumComputer.OpenQasm;

public sealed class OpenQasmExecutionResult
{
    public QuantumSimulator Simulator { get; }
    public int[] ClassicalBits { get; }

    public OpenQasmExecutionResult(QuantumSimulator simulator, int[] classicalBits)
    {
        Simulator = simulator;
        ClassicalBits = classicalBits;
    }
}