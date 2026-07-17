# Topoi.Quantum Benchmarks - macOS ARM64

## Scope

Topoi.Quantum is a local dense state-vector simulator intended for learning,
experimentation, OpenQASM workflows, and small-to-medium circuit prototyping.
It is not positioned as a GPU, distributed, tensor-network, or cloud-hardware backend.

## Environment

OS:
CPU:
Architecture: macOS ARM64
RAM:
.NET SDK:
.NET Runtime:
BenchmarkDotNet:
Commit:
Build configuration: Release
Power mode:
Plugged in:

## Summary

- Recommended comfortable range: up to 22 qubits
- Practical high-end local test: 24 qubits
- Default safety limit: 25 qubits
- Hot gate operations should allocate 0 B after setup
- These results are macOS ARM64 developer-machine benchmarks, not cloud/server benchmarks

## Benchmark execution plan

Run each category independently so the output can be copied into the matching section below.

```bash
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*ApplyHadamard*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*SingleQubitGate*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*ControlledGate*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*Bell*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*Ghz*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*RandomShallow*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*OpenQasm*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*Measurement*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*Sampler*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*Expectation*"
```

## Benchmark category map

| BENCHMARKS.md category | BenchmarkDotNet filter | Benchmark class | Purpose |
|---|---|---|---|
| State-vector scaling | `*ApplyHadamard*` | `ApplyHadamardBenchmarks` | Measures dense state-vector scaling by applying `H` across all qubits. |
| Gate benchmarks | `*SingleQubitGate*` | `SingleQubitGateBenchmarks` | Measures core single-qubit gates: `H`, `X`, `Z`, `RX`, `RY`, `RZ`. |
| Gate benchmarks | `*ControlledGate*` | `ControlledGateBenchmarks` | Measures controlled and multi-qubit gates: `CX`, `CZ`, `CCX`, controlled rotations. |
| Circuit benchmarks | `*Bell*` | `BellCircuitBenchmarks` | Measures small introductory circuit execution. |
| Circuit benchmarks | `*Ghz*` | `GhzCircuitBenchmarks` | Measures entanglement circuit scaling across qubit counts. |
| Circuit benchmarks | `*RandomShallow*` | `RandomShallowCircuitBenchmarks` | Measures mixed-gate shallow circuit execution. |
| OpenQASM benchmarks | `*OpenQasm*` | `OpenQasmBenchmarks` | Measures OpenQASM parse-only, parse-and-execute, executable QASM, custom gates, and adder QASM. |
| Measurement and sampling | `*Measurement*` | `MeasurementBenchmarks` | Measures restore, full-register measurement, and single-qubit measurement. |
| Measurement and sampling | `*Sampler*` | `SamplerBenchmarks` | Measures repeated sampling at different shot counts. |
| Measurement and sampling | `*Expectation*` | `ExpectationValueBenchmarks` | Measures estimator/Pauli expectation workflow. |

## State-vector scaling

Run command:

```bash
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*ApplyHadamard*"
```

Expected benchmark class:

- `ApplyHadamardBenchmarks`

| Method | Qubits | Mean | Error | StdDev | Allocated |
|---|---:|---:|---:|---:|---:|

## Gate benchmarks

Run commands:

```bash
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*SingleQubitGate*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*ControlledGate*"
```

Expected benchmark classes:

- `SingleQubitGateBenchmarks`
- `ControlledGateBenchmarks`

| Method | Gate | Qubits | Mean | Error | StdDev | Allocated |
|---|---|---:|---:|---:|---:|---:|

## Circuit benchmarks

Run commands:

```bash
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*Bell*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*Ghz*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*RandomShallow*"
```

Expected benchmark classes:

- `BellCircuitBenchmarks`
- `GhzCircuitBenchmarks`
- `RandomShallowCircuitBenchmarks`

| Circuit | Qubits | Depth | Mean | Error | StdDev | Allocated |
|---|---:|---:|---:|---:|---:|---:|

## OpenQASM benchmarks

Run command:

```bash
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*OpenQasm*"
```

Expected benchmark class:

- `OpenQasmBenchmarks`

| Method | Qubits | Mean | Error | StdDev | Allocated |
|---|---:|---:|---:|---:|---:|

## Measurement and sampling

Run commands:

```bash
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*Measurement*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*Sampler*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*Expectation*"
```

Expected benchmark classes:

- `MeasurementBenchmarks`
- `SamplerBenchmarks`
- `ExpectationValueBenchmarks`

| Method | Qubits/Shots | Mean | Error | StdDev | Allocated |
|---|---:|---:|---:|---:|---:|

## Release checklist

- [ ] Run all commands on this macOS ARM64 machine using the same commit as the Windows report
- [ ] Confirm `dotnet test Topoi.Quantum.slnx -c Release` passes before benchmarking
- [ ] Confirm benchmark output reports Release configuration
- [ ] Confirm hot gate benchmarks do not allocate after setup
- [ ] Copy BenchmarkDotNet Markdown output into the matching sections above
- [ ] Compare with the Windows ARM64 report for obvious regressions or platform-specific anomalies

## Notes

These benchmarks are intended to detect regressions and communicate practical
limits, not to claim parity with production quantum simulators.
