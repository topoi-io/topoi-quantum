# Topoi.Quantum Benchmarks - Windows ARM64

## Scope

Topoi.Quantum is a local dense state-vector simulator intended for learning,
experimentation, OpenQASM workflows, and small-to-medium circuit prototyping.
It is not positioned as a GPU, distributed, tensor-network, or cloud-hardware backend.

## Environment

- OS: Windows 11 (10.0.26200.8875/25H2/2025Update/HudsonValley2)
- CPU: Snapdragon X 10-core X1P64100 3.40 GHz (Max: 3.42GHz), 1 CPU, 10 logical and 10 physical cores
- Architecture: Windows Arm64 RyuJIT armv8.0-a
- RAM: 16 GB
- .NET SDK: 10.0.301
- .NET Runtime: .NET 10.0.9 (10.0.9, 10.0.926.27113)
- BenchmarkDotNet: v0.15.8
- Commit:
- Build configuration: Release
- Power mode: Plugged In
- Plugged in: Yes

## Summary

- Recommended comfortable range: up to 22 qubits
- Practical high-end local test: 24 qubits
- Default safety limit: 25 qubits
- Hot gate operations should allocate 0 B after setup
- These results are Windows ARM64 developer-machine benchmarks, not cloud/server benchmarks

## Benchmark execution plan

Run each category independently so the output can be copied into the matching section below.

```powershell
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

```powershell
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*ApplyHadamard*"
```

Expected benchmark class:

- `ApplyHadamardBenchmarks`

| Method              | QubitCount | Mean          | Error        | StdDev       | Allocated |
|-------------------- |----------- |--------------:|-------------:|-------------:|----------:|
| **ApplyHOverAllQubits** | **10**         |      **84.81 μs** |     **1.674 μs** |     **2.117 μs** |         **-** |
| **ApplyHOverAllQubits** | **12**         |     **200.42 μs** |     **3.132 μs** |     **2.615 μs** |         **-** |
| **ApplyHOverAllQubits** | **16**         |   **1,414.89 μs** |    **28.227 μs** |    **31.374 μs** |         **-** |
| **ApplyHOverAllQubits** | **20**         |  **23,812.78 μs** |   **468.524 μs** |   **820.582 μs** |         **-** |
| **ApplyHOverAllQubits** | **22**         | **108,227.88 μs** | **2,133.282 μs** | **2,371.137 μs** |         **-** |
| **ApplyHOverAllQubits** | **24**         | **446,584.87 μs** | **4,853.303 μs** | **4,302.327 μs** |         **-** |

## Gate benchmarks

Run commands:

```powershell
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*SingleQubitGate*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*ControlledGate*"
```

Expected benchmark classes:

- `SingleQubitGateBenchmarks`
- `ControlledGateBenchmarks`

| Method                  | QubitCount | Gate | Mean          | Error        | StdDev       | Allocated |
|------------------------ |----------- |----- |--------------:|-------------:|-------------:|----------:|
| **ApplyGateAcrossRegister** | **10**         | **X**    |      **85.59 μs** |     **1.599 μs** |     **2.671 μs** |         **-** |
| **ApplyGateAcrossRegister** | **10**         | **Z**    |     **142.64 μs** |     **1.322 μs** |     **1.172 μs** |         **-** |
| **ApplyGateAcrossRegister** | **10**         | **H**    |      **97.58 μs** |     **1.348 μs** |     **1.126 μs** |         **-** |
| **ApplyGateAcrossRegister** | **10**         | **RX**   |      **84.98 μs** |     **1.588 μs** |     **3.022 μs** |         **-** |
| **ApplyGateAcrossRegister** | **10**         | **RY**   |      **84.43 μs** |     **1.688 μs** |     **3.706 μs** |         **-** |
| **ApplyGateAcrossRegister** | **10**         | **RZ**   |      **84.74 μs** |     **1.614 μs** |     **2.652 μs** |         **-** |
| **ApplyGateAcrossRegister** | **12**         | **X**    |     **203.03 μs** |     **2.457 μs** |     **2.298 μs** |         **-** |
| **ApplyGateAcrossRegister** | **12**         | **Z**    |     **204.52 μs** |     **2.887 μs** |     **2.700 μs** |         **-** |
| **ApplyGateAcrossRegister** | **12**         | **H**    |     **202.03 μs** |     **0.842 μs** |     **0.658 μs** |         **-** |
| **ApplyGateAcrossRegister** | **12**         | **RX**   |     **201.55 μs** |     **2.532 μs** |     **2.245 μs** |         **-** |
| **ApplyGateAcrossRegister** | **12**         | **RY**   |     **209.37 μs** |     **4.052 μs** |     **3.592 μs** |         **-** |
| **ApplyGateAcrossRegister** | **12**         | **RZ**   |     **202.39 μs** |     **1.130 μs** |     **0.944 μs** |         **-** |
| **ApplyGateAcrossRegister** | **16**         | **X**    |   **1,437.55 μs** |    **24.341 μs** |    **21.578 μs** |         **-** |
| **ApplyGateAcrossRegister** | **16**         | **Z**    |   **1,407.94 μs** |    **25.587 μs** |    **26.276 μs** |         **-** |
| **ApplyGateAcrossRegister** | **16**         | **H**    |   **1,408.29 μs** |    **28.034 μs** |    **24.852 μs** |         **-** |
| **ApplyGateAcrossRegister** | **16**         | **RX**   |   **1,412.15 μs** |    **27.454 μs** |    **26.964 μs** |         **-** |
| **ApplyGateAcrossRegister** | **16**         | **RY**   |   **1,407.17 μs** |    **27.767 μs** |    **27.271 μs** |         **-** |
| **ApplyGateAcrossRegister** | **16**         | **RZ**   |   **1,412.61 μs** |    **27.817 μs** |    **26.020 μs** |         **-** |
| **ApplyGateAcrossRegister** | **20**         | **X**    |  **21,337.63 μs** |   **420.871 μs** |   **758.917 μs** |         **-** |
| **ApplyGateAcrossRegister** | **20**         | **Z**    |  **22,702.95 μs** |   **453.623 μs** |   **424.320 μs** |         **-** |
| **ApplyGateAcrossRegister** | **20**         | **H**    |  **23,969.15 μs** |   **478.618 μs** |   **977.690 μs** |         **-** |
| **ApplyGateAcrossRegister** | **20**         | **RX**   |  **30,557.48 μs** |   **608.179 μs** | **1,269.495 μs** |         **-** |
| **ApplyGateAcrossRegister** | **20**         | **RY**   |  **26,465.60 μs** |   **424.845 μs** |   **376.614 μs** |         **-** |
| **ApplyGateAcrossRegister** | **20**         | **RZ**   |  **25,971.02 μs** |   **456.342 μs** |   **468.630 μs** |         **-** |
| **ApplyGateAcrossRegister** | **22**         | **X**    |  **93,664.62 μs** | **1,793.610 μs** | **1,589.989 μs** |         **-** |
| **ApplyGateAcrossRegister** | **22**         | **Z**    |  **98,947.73 μs** | **1,290.969 μs** | **1,207.573 μs** |         **-** |
| **ApplyGateAcrossRegister** | **22**         | **H**    | **100,665.73 μs** | **1,821.930 μs** | **1,704.234 μs** |         **-** |
| **ApplyGateAcrossRegister** | **22**         | **RX**   | **136,105.77 μs** | **1,283.133 μs** | **1,200.244 μs** |         **-** |
| **ApplyGateAcrossRegister** | **22**         | **RY**   | **115,499.69 μs** | **1,343.531 μs** | **1,256.739 μs** |         **-** |
| **ApplyGateAcrossRegister** | **22**         | **RZ**   | **114,295.79 μs** | **1,305.477 μs** | **1,221.144 μs** |         **-** |
| **ApplyGateAcrossRegister** | **24**         | **X**    | **408,662.02 μs** | **6,428.298 μs** | **5,367.920 μs** |         **-** |
| **ApplyGateAcrossRegister** | **24**         | **Z**    | **439,697.59 μs** | **8,383.853 μs** | **7,432.067 μs** |      **32 B** |
| **ApplyGateAcrossRegister** | **24**         | **H**    | **457,727.51 μs** | **6,913.403 μs** | **6,128.552 μs** |      **32 B** |
| **ApplyGateAcrossRegister** | **24**         | **RX**   | **596,738.04 μs** | **4,480.954 μs** | **3,741.800 μs** |         **-** |
| **ApplyGateAcrossRegister** | **24**         | **RY**   | **507,116.59 μs** | **4,568.780 μs** | **4,050.105 μs** |         **-** |
| **ApplyGateAcrossRegister** | **24**         | **RZ**   | **497,208.18 μs** | **5,243.522 μs** | **4,648.246 μs** |         **-** |

| Method                           | QubitCount | Mean          | Error        | StdDev       | Median        | Allocated |
|--------------------------------- |----------- |--------------:|-------------:|-------------:|--------------:|----------:|
| **CXAcrossRegister**                 | **10**         |      **66.39 μs** |     **1.324 μs** |     **3.690 μs** |      **64.80 μs** |    **2808 B** |
| CZAcrossRegister                 | 10         |      52.99 μs |     1.013 μs |     0.846 μs |      52.65 μs |         - |
| CCXAcrossRegister                | 10         |      59.42 μs |     2.200 μs |     6.382 μs |      57.35 μs |    2496 B |
| ControlledRotationAcrossRegister | 10         |      67.15 μs |     1.589 μs |     4.295 μs |      64.95 μs |    2808 B |
| **CXAcrossRegister**                 | **12**         |     **125.28 μs** |     **2.353 μs** |     **6.031 μs** |     **123.80 μs** |    **3432 B** |
| CZAcrossRegister                 | 12         |      81.21 μs |     1.380 μs |     2.023 μs |      81.20 μs |         - |
| CCXAcrossRegister                | 12         |     109.82 μs |     2.124 μs |     5.484 μs |     108.30 μs |    3120 B |
| ControlledRotationAcrossRegister | 12         |     121.51 μs |     2.312 μs |     6.406 μs |     118.30 μs |    3432 B |
| **CXAcrossRegister**                 | **16**         |   **1,206.95 μs** |    **21.344 μs** |    **29.216 μs** |   **1,203.85 μs** |    **4680 B** |
| CZAcrossRegister                 | 16         |     688.11 μs |    13.712 μs |    14.082 μs |     687.50 μs |         - |
| CCXAcrossRegister                | 16         |   1,140.73 μs |    17.584 μs |    18.057 μs |   1,137.90 μs |    4368 B |
| ControlledRotationAcrossRegister | 16         |   1,196.65 μs |    20.537 μs |    28.790 μs |   1,192.20 μs |    4680 B |
| **CXAcrossRegister**                 | **20**         |  **25,196.12 μs** |   **490.727 μs** |   **638.084 μs** |  **25,202.45 μs** |    **5928 B** |
| CZAcrossRegister                 | 20         |  19,318.72 μs |   374.369 μs |   560.337 μs |  19,295.75 μs |         - |
| CCXAcrossRegister                | 20         |  19,861.05 μs |   319.426 μs |   266.735 μs |  19,854.00 μs |    4464 B |
| ControlledRotationAcrossRegister | 20         |  24,386.49 μs |   454.861 μs |   446.735 μs |  24,280.20 μs |    5928 B |
| **CXAcrossRegister**                 | **22**         | **103,117.15 μs** | **1,480.713 μs** | **1,385.060 μs** | **103,061.00 μs** |    **6552 B** |
| CZAcrossRegister                 | 22         |  81,493.04 μs | 1,443.019 μs | 2,246.607 μs |  80,933.60 μs |         - |
| CCXAcrossRegister                | 22         |  87,659.30 μs | 1,748.916 μs | 1,796.008 μs |  88,319.60 μs |    4960 B |
| ControlledRotationAcrossRegister | 22         | 107,090.97 μs | 1,064.604 μs |   995.832 μs | 107,345.40 μs |    6552 B |
| **CXAcrossRegister**                 | **24**         | **447,391.29 μs** | **4,646.595 μs** | **4,346.428 μs** | **447,350.70 μs** |    **7176 B** |
| CZAcrossRegister                 | 24         | 349,867.77 μs | 5,512.343 μs | 4,886.548 μs | 348,752.10 μs |         - |
| CCXAcrossRegister                | 24         | 378,559.50 μs | 5,475.503 μs | 5,121.788 μs | 377,535.70 μs |    5456 B |
| ControlledRotationAcrossRegister | 24         | 470,700.10 μs | 5,895.028 μs | 5,225.789 μs | 471,387.30 μs |    7176 B |


## Circuit benchmarks

Run commands:

```powershell
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

```powershell
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*OpenQasm*"
```

Expected benchmark class:

- `OpenQasmBenchmarks`

| Method | Qubits | Mean | Error | StdDev | Allocated |
|---|---:|---:|---:|---:|---:|

## Measurement and sampling

Run commands:

```powershell
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

- [ ] Run all commands on this Windows ARM64 machine using the same commit as the macOS report
- [ ] Confirm `dotnet test Topoi.Quantum.slnx -c Release` passes before benchmarking
- [ ] Confirm benchmark output reports Release configuration
- [ ] Confirm hot gate benchmarks do not allocate after setup
- [ ] Copy BenchmarkDotNet Markdown output into the matching sections above
- [ ] Compare with the macOS ARM64 report for obvious regressions or platform-specific anomalies

## Notes

These benchmarks are intended to detect regressions and communicate practical
limits, not to claim parity with production quantum simulators.
