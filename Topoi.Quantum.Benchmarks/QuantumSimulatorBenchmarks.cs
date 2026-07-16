using System.Numerics;
using System.Text;

using BenchmarkDotNet.Attributes;

using Topoi.Quantum.OpenQasm;
using Topoi.Quantum.Primitives;

namespace Topoi.Quantum.Benchmarks;

[MemoryDiagnoser]
public class SimulatorLifecycleBenchmarks
{
    [Params(10, 12, 16, 20, 22, 24)]
    public int QubitCount { get; set; }

    private QuantumSimulator _simulator = null!;
    private Complex[] _snapshot = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _simulator = new QuantumSimulator(QubitCount);

        for (int q = 0; q < QubitCount; q++)
            _simulator.H(q);

        _snapshot = _simulator.SnapshotState();
    }

    [Benchmark]
    public QuantumSimulator CreateSimulator()
    {
        return new QuantumSimulator(QubitCount);
    }

    [Benchmark]
    public void Reset()
    {
        _simulator.Reset();
    }

    [Benchmark]
    public Complex[] SnapshotState()
    {
        return _simulator.SnapshotState();
    }

    [Benchmark]
    public void RestoreState()
    {
        _simulator.RestoreState(_snapshot);
    }
}

[MemoryDiagnoser]
public class SingleQubitGateBenchmarks
{
    [Params(10, 12, 16, 20, 22, 24)]
    public int QubitCount { get; set; }

    [Params(
        GateKind.H,
        GateKind.X,
        GateKind.Z,
        GateKind.RX,
        GateKind.RY,
        GateKind.RZ)]
    public GateKind Gate { get; set; }

    private QuantumSimulator _simulator = null!;

    [IterationSetup]
    public void Setup()
    {
        _simulator = new QuantumSimulator(QubitCount);
    }

    [Benchmark]
    public void ApplyGateAcrossRegister()
    {
        for (int q = 0; q < QubitCount; q++)
        {
            switch (Gate)
            {
                case GateKind.H:
                    _simulator.H(q);
                    break;

                case GateKind.X:
                    _simulator.X(q);
                    break;

                case GateKind.Z:
                    _simulator.Z(q);
                    break;

                case GateKind.RX:
                    _simulator.RX(q, Math.PI / 7.0);
                    break;

                case GateKind.RY:
                    _simulator.RY(q, Math.PI / 7.0);
                    break;

                case GateKind.RZ:
                    _simulator.RZ(q, Math.PI / 7.0);
                    break;

                default:
                    throw new NotSupportedException($"Unsupported benchmark gate: {Gate}");
            }
        }
    }
}

[MemoryDiagnoser]
public class ApplyHadamardBenchmarks
{
    [Params(10, 12, 16, 20, 22, 24)]
    public int QubitCount { get; set; }

    private QuantumSimulator _simulator = null!;

    [IterationSetup]
    public void Setup()
    {
        _simulator = new QuantumSimulator(QubitCount);
    }

    [Benchmark]
    public void ApplyHOverAllQubits()
    {
        for (int q = 0; q < QubitCount; q++)
            _simulator.H(q);
    }
}

[MemoryDiagnoser]
public class ControlledGateBenchmarks
{
    [Params(10, 12, 16, 20, 22, 24)]
    public int QubitCount { get; set; }

    private QuantumSimulator _simulator = null!;

    [IterationSetup]
    public void Setup()
    {
        _simulator = new QuantumSimulator(QubitCount);

        for (int q = 0; q < QubitCount; q++)
            _simulator.H(q);
    }

    [Benchmark]
    public void CXAcrossRegister()
    {
        for (int q = 0; q + 1 < QubitCount; q++)
            _simulator.CX(q, q + 1);
    }

    [Benchmark]
    public void CZAcrossRegister()
    {
        for (int q = 0; q + 1 < QubitCount; q++)
            _simulator.CZ(q, q + 1);
    }

    [Benchmark]
    public void CCXAcrossRegister()
    {
        for (int q = 0; q + 2 < QubitCount; q++)
            _simulator.CCX(q, q + 1, q + 2);
    }

    [Benchmark]
    public void ControlledRotationAcrossRegister()
    {
        for (int q = 0; q + 1 < QubitCount; q++)
            _simulator.CRY(q, q + 1, Math.PI / 5.0);
    }
}

[MemoryDiagnoser]
public class BellCircuitBenchmarks
{
    private QuantumCircuit _circuit = null!;
    private QuantumSimulator _simulator = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _circuit = QuantumCircuitBuilder
            .WithQubits(2)
            .H(0)
            .CX(0, 1)
            .Build();
    }

    [IterationSetup]
    public void Setup()
    {
        _simulator = new QuantumSimulator(2);
    }

    [Benchmark]
    public void ExecuteBellCircuit()
    {
        _circuit.Run(_simulator, resetFirst: true);
    }
}

[MemoryDiagnoser]
public class GhzCircuitBenchmarks
{
    [Params(8, 12, 16, 20, 22, 24)]
    public int QubitCount { get; set; }

    private QuantumCircuit _circuit = null!;
    private QuantumSimulator _simulator = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var circuit = new QuantumCircuit(QubitCount);

        circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));

        for (int q = 1; q < QubitCount; q++)
            circuit.Add(new GateOperation(GateKind.CX, new[] { q - 1, q }));

        _circuit = circuit;
    }

    [IterationSetup]
    public void Setup()
    {
        _simulator = new QuantumSimulator(QubitCount);
    }

    [Benchmark]
    public void ExecuteGhzCircuit()
    {
        _circuit.Run(_simulator, resetFirst: true);
    }
}

[MemoryDiagnoser]
public class RandomShallowCircuitBenchmarks
{
    [Params(8, 12, 16, 20, 22)]
    public int QubitCount { get; set; }

    [Params(2, 4, 8)]
    public int Depth { get; set; }

    private QuantumCircuit _circuit = null!;
    private QuantumSimulator _simulator = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var random = new Random(123);
        var circuit = new QuantumCircuit(QubitCount);

        for (int layer = 0; layer < Depth; layer++)
        {
            for (int q = 0; q < QubitCount; q++)
            {
                int gate = random.Next(4);

                switch (gate)
                {
                    case 0:
                        circuit.Add(new GateOperation(GateKind.H, new[] { q }));
                        break;

                    case 1:
                        circuit.Add(new GateOperation(GateKind.X, new[] { q }));
                        break;

                    case 2:
                        circuit.Add(new GateOperation(GateKind.Z, new[] { q }));
                        break;

                    case 3:
                        circuit.Add(new GateOperation(GateKind.RY, new[] { q }, Math.PI / 7.0));
                        break;
                }
            }

            for (int q = 0; q + 1 < QubitCount; q += 2)
                circuit.Add(new GateOperation(GateKind.CX, new[] { q, q + 1 }));
        }

        _circuit = circuit;
    }

    [IterationSetup]
    public void Setup()
    {
        _simulator = new QuantumSimulator(QubitCount);
    }

    [Benchmark]
    public void ExecuteRandomShallowCircuit()
    {
        _circuit.Run(_simulator, resetFirst: true);
    }
}

[MemoryDiagnoser]
public class MeasurementBenchmarks
{
    [Params(10, 12, 16, 20, 22, 24)]
    public int QubitCount { get; set; }

    private QuantumSimulator _simulator = null!;
    private Complex[] _snapshot = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _simulator = new QuantumSimulator(QubitCount);

        for (int q = 0; q < QubitCount; q++)
            _simulator.H(q);

        _snapshot = _simulator.SnapshotState();
    }

    [Benchmark(Baseline = true)]
    public void RestoreOnly()
    {
        _simulator.RestoreState(_snapshot);
    }

    [Benchmark]
    public int MeasureAll()
    {
        _simulator.RestoreState(_snapshot);
        return _simulator.MeasureAll();
    }

    [Benchmark]
    public int MeasureQubit()
    {
        _simulator.RestoreState(_snapshot);
        return _simulator.Measure(0);
    }
}

[MemoryDiagnoser]
public class SamplerBenchmarks
{
    [Params(1_000, 10_000, 100_000)]
    public int Shots { get; set; }

    private QuantumCircuit _bellCircuit = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _bellCircuit = QuantumCircuitBuilder
            .WithQubits(2)
            .H(0)
            .CX(0, 1)
            .Build();
    }

    [Benchmark]
    public SamplerResult SampleBellCircuit()
    {
        var sampler = new Sampler(new SeededRandomSource(123));
        return sampler.Run(_bellCircuit, Shots);
    }
}

[MemoryDiagnoser]
public class ExpectationValueBenchmarks
{
    private QuantumCircuit _circuit = null!;
    private Estimator _estimator = null!;
    private PauliTerm[] _observable = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _circuit = QuantumCircuitBuilder
            .WithQubits(2)
            .H(0)
            .CX(0, 1)
            .Build();

        _observable =
        [
            new PauliTerm('Z', 0),
            new PauliTerm('Z', 1)
        ];

        _estimator = new Estimator();
    }

    [Benchmark]
    public EstimatorResult EstimateBellZZ()
    {
        return _estimator.Estimate(_circuit, _observable);
    }
}

[MemoryDiagnoser]
public class OpenQasmBenchmarks
{
    [Params(2, 8, 16, 20)]
    public int QubitCount { get; set; }

    private string _gateOnlySource = null!;
    private string _executableSource = null!;
    private string _customGateSource = null!;
    private string _adder4Source = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _gateOnlySource = BuildGhzGateOnlyQasm(QubitCount);
        _executableSource = BuildGhzExecutableQasm(QubitCount);
        _customGateSource = BuildCustomGateBellQasm();
        _adder4Source = BuildFourBitAdderQasm();
    }

    [Benchmark]
    public QuantumCircuit ParseGateOnlyQasm()
    {
        return OpenQasmCircuitLoader.LoadFromString(_gateOnlySource);
    }

    [Benchmark]
    public int ParseAndExecuteGateOnlyQasm()
    {
        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(_gateOnlySource);

        var simulator = new QuantumSimulator(circuit.QubitCount);
        circuit.Run(simulator, resetFirst: true);

        return simulator.MeasureAll();
    }

    [Benchmark]
    public OpenQasmExecutableProgram ParseExecutableQasm()
    {
        return OpenQasmCircuitLoader.LoadExecutableFromString(_executableSource);
    }

    [Benchmark]
    public int ParseAndExecuteExecutableQasm()
    {
        OpenQasmExecutableProgram program =
            OpenQasmCircuitLoader.LoadExecutableFromString(_executableSource);

        OpenQasmExecutionResult result =
            OpenQasmExecutor.Execute(program, new SeededRandomSource(123));

        return EncodeBits(result.ClassicalBits);
    }

    [Benchmark]
    public int ParseAndExecuteCustomGateQasm()
    {
        OpenQasmExecutableProgram program =
            OpenQasmCircuitLoader.LoadExecutableFromString(_customGateSource);

        OpenQasmExecutionResult result =
            OpenQasmExecutor.Execute(program, new SeededRandomSource(123));

        return EncodeBits(result.ClassicalBits);
    }

    [Benchmark]
    public int ParseAndExecuteFourBitAdderQasm()
    {
        OpenQasmExecutableProgram program =
            OpenQasmCircuitLoader.LoadExecutableFromString(_adder4Source);

        OpenQasmExecutionResult result =
            OpenQasmExecutor.Execute(program, new SeededRandomSource(123));

        return EncodeBits(result.ClassicalBits);
    }

    private static string BuildGhzGateOnlyQasm(int qubitCount)
    {
        var builder = new StringBuilder();

        builder.AppendLine("OPENQASM 3.1;");
        builder.AppendLine("include \"stdgates.inc\";");
        builder.AppendLine();
        builder.AppendLine($"qubit[{qubitCount}] q;");
        builder.AppendLine();
        builder.AppendLine("h q[0];");

        for (int q = 1; q < qubitCount; q++)
            builder.AppendLine($"cx q[{q - 1}], q[{q}];");

        return builder.ToString();
    }

    private static string BuildGhzExecutableQasm(int qubitCount)
    {
        var builder = new StringBuilder();

        builder.AppendLine("OPENQASM 3.1;");
        builder.AppendLine("include \"stdgates.inc\";");
        builder.AppendLine();
        builder.AppendLine($"qubit[{qubitCount}] q;");
        builder.AppendLine($"bit[{qubitCount}] c;");
        builder.AppendLine();
        builder.AppendLine("h q[0];");

        for (int q = 1; q < qubitCount; q++)
            builder.AppendLine($"cx q[{q - 1}], q[{q}];");

        builder.AppendLine();

        for (int q = 0; q < qubitCount; q++)
            builder.AppendLine($"measure q[{q}] -> c[{q}];");

        return builder.ToString();
    }

    private static string BuildCustomGateBellQasm()
    {
        return """
        OPENQASM 3.1;
        include "stdgates.inc";

        gate bell a, b {
            h a;
            cx a, b;
        }

        qubit[2] q;
        bit[2] c;

        bell q[0], q[1];

        measure q[0] -> c[0];
        measure q[1] -> c[1];
        """;
    }

    private static string BuildFourBitAdderQasm()
    {
        return """
        OPENQASM 3.1;
        include "stdgates.inc";

        gate majority a, b, c {
            cx c, b;
            cx c, a;
            ccx a, b, c;
        }

        gate unmaj a, b, c {
            ccx a, b, c;
            cx c, a;
            cx a, b;
        }

        qubit[1] cin;
        qubit[4] a;
        qubit[4] b;
        qubit[1] cout;
        bit[5] ans;

        reset cin[0];

        reset a[0];
        reset a[1];
        reset a[2];
        reset a[3];

        reset b[0];
        reset b[1];
        reset b[2];
        reset b[3];

        reset cout[0];

        x a[0];

        x b[0];
        x b[1];
        x b[2];
        x b[3];

        majority cin[0], b[0], a[0];

        majority a[0], b[1], a[1];
        majority a[1], b[2], a[2];
        majority a[2], b[3], a[3];

        cx a[3], cout[0];

        unmaj a[2], b[3], a[3];
        unmaj a[1], b[2], a[2];
        unmaj a[0], b[1], a[1];

        unmaj cin[0], b[0], a[0];

        measure b[0] -> ans[0];
        measure b[1] -> ans[1];
        measure b[2] -> ans[2];
        measure b[3] -> ans[3];
        measure cout[0] -> ans[4];
        """;
    }

    private static int EncodeBits(IReadOnlyList<int> bits)
    {
        int value = 0;

        for (int i = 0; i < bits.Count && i < 31; i++)
            value |= bits[i] << i;

        return value;
    }
}