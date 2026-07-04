using System.Numerics;
using System.Text;

using BenchmarkDotNet.Attributes;

using Topoi.Quantum.OpenQasm;
using Topoi.Quantum.Primitives;

namespace Topoi.Quantum.Benchmarks;

[MemoryDiagnoser]
public class ApplyHadamardBenchmarks
{
    [Params(20, 22, 24)]
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
    [Params(20, 22, 24)]
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
    [Params(20, 22)]
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
public class ControlledGateBenchmarks
{
    [Params(20, 22, 24)]
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
    public void ControlledRotationAcrossRegister()
    {
        for (int q = 0; q + 1 < QubitCount; q++)
            _simulator.CRY(q, q + 1, Math.PI / 5.0);
    }
}

[MemoryDiagnoser]
public class MeasurementBenchmarks
{
    [Params(20, 22, 24)]
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
    [Params(2, 20)]
    public int QubitCount { get; set; }

    private string _source = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var builder = new StringBuilder();

        builder.AppendLine("OPENQASM 3.1;");
        builder.AppendLine("include \"stdgates.inc\";");
        builder.AppendLine();
        builder.AppendLine($"qubit[{QubitCount}] q;");
        builder.AppendLine();
        builder.AppendLine("h q[0];");

        for (int q = 1; q < QubitCount; q++)
            builder.AppendLine($"cx q[{q - 1}], q[{q}];");

        _source = builder.ToString();
    }

    [Benchmark]
    public QuantumCircuit ParseOnly()
    {
        return OpenQasmCircuitLoader.LoadFromString(_source);
    }

    [Benchmark]
    public int ParseAndExecute()
    {
        QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(_source);

        var simulator = new QuantumSimulator(circuit.QubitCount);
        circuit.Run(simulator, resetFirst: true);

        return simulator.MeasureAll();
    }
}