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
- Build configuration: Release
- Power mode: Plugged In

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

| Method             | Mean     | Error     | StdDev    | Allocated |
|------------------- |---------:|----------:|----------:|----------:|
| ExecuteBellCircuit | 1.835 μs | 0.1057 μs | 0.2785 μs |     904 B |

| Method            | QubitCount | Mean          | Error        | StdDev        | Allocated |
|------------------ |----------- |--------------:|-------------:|--------------:|----------:|
| **ExecuteGhzCircuit** | **8**          |      **37.55 μs** |     **1.221 μs** |      **3.560 μs** |   **4.45 KB** |
| **ExecuteGhzCircuit** | **12**         |     **146.10 μs** |     **2.922 μs** |      **7.276 μs** |   **6.82 KB** |
| **ExecuteGhzCircuit** | **16**         |   **1,307.84 μs** |    **23.207 μs** |     **20.572 μs** |    **9.2 KB** |
| **ExecuteGhzCircuit** | **20**         |  **24,449.77 μs** |   **474.731 μs** |    **766.602 μs** |  **11.57 KB** |
| **ExecuteGhzCircuit** | **22**         | **103,419.92 μs** |   **968.150 μs** |    **905.608 μs** |  **12.76 KB** |
| **ExecuteGhzCircuit** | **24**         | **451,866.22 μs** | **8,330.454 μs** | **12,969.514 μs** |  **13.95 KB** |

| Method                      | QubitCount | Depth | Mean            | Error         | StdDev       | Allocated |
|---------------------------- |----------- |------ |----------------:|--------------:|-------------:|----------:|
| **ExecuteRandomShallowCircuit** | **8**          | **2**     |        **84.98 μs** |      **1.651 μs** |     **2.027 μs** |   **9.38 KB** |
| **ExecuteRandomShallowCircuit** | **8**          | **4**     |       **140.03 μs** |      **2.689 μs** |     **5.845 μs** |  **18.75 KB** |
| **ExecuteRandomShallowCircuit** | **8**          | **8**     |       **271.82 μs** |      **4.852 μs** |     **7.553 μs** |   **37.5 KB** |
| **ExecuteRandomShallowCircuit** | **12**         | **2**     |       **557.86 μs** |      **7.327 μs** |     **6.496 μs** |  **14.06 KB** |
| **ExecuteRandomShallowCircuit** | **12**         | **4**     |     **1,104.07 μs** |     **13.584 μs** |    **12.707 μs** |  **28.13 KB** |
| **ExecuteRandomShallowCircuit** | **12**         | **8**     |     **2,150.28 μs** |     **41.873 μs** |    **48.221 μs** |  **56.25 KB** |
| **ExecuteRandomShallowCircuit** | **16**         | **2**     |     **4,163.26 μs** |     **79.067 μs** |    **77.654 μs** |  **18.75 KB** |
| **ExecuteRandomShallowCircuit** | **16**         | **4**     |     **6,204.45 μs** |     **81.866 μs** |   **196.145 μs** |   **37.5 KB** |
| **ExecuteRandomShallowCircuit** | **16**         | **8**     |    **12,948.27 μs** |    **215.112 μs** |   **287.169 μs** |     **75 KB** |
| **ExecuteRandomShallowCircuit** | **20**         | **2**     |    **75,662.20 μs** |  **1,458.537 μs** | **1,621.159 μs** |  **23.44 KB** |
| **ExecuteRandomShallowCircuit** | **20**         | **4**     |   **139,457.80 μs** |  **1,748.721 μs** | **1,635.754 μs** |  **46.88 KB** |
| **ExecuteRandomShallowCircuit** | **20**         | **8**     |   **276,032.11 μs** |  **2,621.111 μs** | **2,451.789 μs** |  **93.75 KB** |
| **ExecuteRandomShallowCircuit** | **22**         | **2**     |   **309,913.71 μs** |  **3,562.260 μs** | **3,332.141 μs** |  **25.78 KB** |
| **ExecuteRandomShallowCircuit** | **22**         | **4**     |   **627,758.76 μs** |  **4,920.668 μs** | **4,602.795 μs** |  **51.56 KB** |
| **ExecuteRandomShallowCircuit** | **22**         | **8**     | **1,268,470.00 μs** | **10,653.674 μs** | **9,444.204 μs** | **103.13 KB** |


## OpenQASM benchmarks

Run command:

```powershell
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*OpenQasm*"
```

Expected benchmark class:

- `OpenQasmBenchmarks`

| Method                          | QubitCount | Mean          | Error         | StdDev        | Gen0     | Gen1     | Gen2     | Allocated   |
|-------------------------------- |----------- |--------------:|--------------:|--------------:|---------:|---------:|---------:|------------:|
| **ParseGateOnlyQasm**               | **2**          |      **1.239 μs** |     **0.0155 μs** |     **0.0137 μs** |   **1.1902** |        **-** |        **-** |     **4.87 KB** |
| ParseAndExecuteGateOnlyQasm     | 2          |      1.496 μs |     0.0297 μs |     0.0397 μs |   1.4458 |        - |        - |     5.91 KB |
| ParseExecutableQasm             | 2          |      1.985 μs |     0.0387 μs |     0.0613 μs |   1.7433 |        - |        - |     7.13 KB |
| ParseAndExecuteExecutableQasm   | 2          |      2.525 μs |     0.0496 μs |     0.0662 μs |   2.0981 |        - |        - |     8.58 KB |
| ParseAndExecuteCustomGateQasm   | 2          |      3.329 μs |     0.0656 μs |     0.1060 μs |   3.0136 |        - |        - |    12.32 KB |
| ParseAndExecuteFourBitAdderQasm | 2          |     79.922 μs |     0.8393 μs |     0.7851 μs |  23.5596 |   0.1221 |        - |    96.49 KB |
| **ParseGateOnlyQasm**               | **8**          |      **3.810 μs** |     **0.0739 μs** |     **0.0986 μs** |   **3.7804** |        **-** |        **-** |    **15.45 KB** |
| ParseAndExecuteGateOnlyQasm     | 8          |      7.302 μs |     0.1395 μs |     0.1661 μs |   5.8670 |        - |        - |    23.99 KB |
| ParseExecutableQasm             | 8          |      6.221 μs |     0.1245 μs |     0.1222 μs |   5.5389 |        - |        - |    22.65 KB |
| ParseAndExecuteExecutableQasm   | 8          |     13.236 μs |     0.2639 μs |     0.3524 μs |   7.7362 |   0.0153 |        - |    31.62 KB |
| ParseAndExecuteCustomGateQasm   | 8          |      3.391 μs |     0.0665 μs |     0.0817 μs |   3.0136 |        - |        - |    12.32 KB |
| ParseAndExecuteFourBitAdderQasm | 8          |     80.226 μs |     1.5629 μs |     1.6050 μs |  23.5596 |   0.1221 |        - |    96.49 KB |
| **ParseGateOnlyQasm**               | **16**         |      **7.177 μs** |     **0.1424 μs** |     **0.1950 μs** |   **7.2403** |        **-** |        **-** |    **29.58 KB** |
| ParseAndExecuteGateOnlyQasm     | 16         |  1,385.036 μs |    24.4493 μs |    20.4163 μs | 332.0313 | 332.0313 | 332.0313 |  1062.98 KB |
| ParseExecutableQasm             | 16         |     11.666 μs |     0.2289 μs |     0.3283 μs |  10.6049 |   0.0153 |        - |    43.38 KB |
| ParseAndExecuteExecutableQasm   | 16         |  3,316.568 μs |    62.3825 μs |    69.3380 μs | 332.0313 | 332.0313 | 332.0313 |  1077.23 KB |
| ParseAndExecuteCustomGateQasm   | 16         |      3.361 μs |     0.0670 μs |     0.0939 μs |   3.0136 |        - |        - |    12.32 KB |
| ParseAndExecuteFourBitAdderQasm | 16         |     80.514 μs |     1.5589 μs |     1.5311 μs |  23.5596 |   0.1221 |        - |    96.49 KB |
| **ParseGateOnlyQasm**               | **20**         |      **9.044 μs** |     **0.1753 μs** |     **0.2830 μs** |   **8.6975** |        **-** |        **-** |    **35.57 KB** |
| ParseAndExecuteGateOnlyQasm     | 20         | 27,205.657 μs |   360.1881 μs |   336.9202 μs | 500.0000 | 500.0000 | 500.0000 | 16431.63 KB |
| ParseExecutableQasm             | 20         |     14.546 μs |     0.2828 μs |     0.3576 μs |  12.6343 |        - |        - |    51.62 KB |
| ParseAndExecuteExecutableQasm   | 20         | 66,652.234 μs | 1,293.9472 μs | 1,210.3590 μs | 500.0000 | 500.0000 | 500.0000 | 16447.91 KB |
| ParseAndExecuteCustomGateQasm   | 20         |      3.438 μs |     0.0603 μs |     0.0564 μs |   3.0136 |        - |        - |    12.32 KB |
| ParseAndExecuteFourBitAdderQasm | 20         |     80.931 μs |     1.5894 μs |     2.2282 μs |  23.5596 |   0.1221 |        - |    96.49 KB |


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

| Method       | QubitCount | Mean            | Error         | StdDev          | Median          | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------- |----------- |----------------:|--------------:|----------------:|----------------:|------:|--------:|----------:|------------:|
| **RestoreOnly**  | **10**         |        **200.1 ns** |       **0.70 ns** |         **0.65 ns** |        **200.2 ns** |  **1.00** |    **0.00** |         **-** |          **NA** |
| MeasureAll   | 10         |      1,840.8 ns |       4.88 ns |         4.33 ns |      1,840.6 ns |  9.20 |    0.04 |         - |          NA |
| MeasureQubit | 10         |      1,982.7 ns |       8.74 ns |         8.17 ns |      1,980.1 ns |  9.91 |    0.05 |         - |          NA |
|              |            |                 |               |                 |                 |       |         |           |             |
| **RestoreOnly**  | **12**         |      **1,328.1 ns** |      **14.76 ns** |        **13.80 ns** |      **1,323.3 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| MeasureAll   | 12         |      7,586.5 ns |      38.85 ns |        36.34 ns |      7,573.4 ns |  5.71 |    0.06 |         - |          NA |
| MeasureQubit | 12         |      8,366.4 ns |     108.68 ns |       101.66 ns |      8,337.3 ns |  6.30 |    0.10 |         - |          NA |
|              |            |                 |               |                 |                 |       |         |           |             |
| **RestoreOnly**  | **16**         |     **21,934.4 ns** |     **229.54 ns** |       **203.48 ns** |     **21,951.6 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| MeasureAll   | 16         |    120,833.0 ns |     799.64 ns |       708.86 ns |    120,986.8 ns |  5.51 |    0.06 |         - |          NA |
| MeasureQubit | 16         |    139,284.9 ns |   1,811.42 ns |     1,694.41 ns |    138,892.9 ns |  6.35 |    0.09 |         - |          NA |
|              |            |                 |               |                 |                 |       |         |           |             |
| **RestoreOnly**  | **20**         |    **663,583.7 ns** |  **11,715.77 ns** |    **10,958.94 ns** |    **666,313.9 ns** |  **1.00** |    **0.02** |         **-** |          **NA** |
| MeasureAll   | 20         |  2,361,168.9 ns |  45,786.33 ns |    44,968.30 ns |  2,359,414.6 ns |  3.56 |    0.09 |         - |          NA |
| MeasureQubit | 20         |  2,689,046.2 ns |  53,099.56 ns |    41,456.64 ns |  2,682,363.7 ns |  4.05 |    0.09 |         - |          NA |
|              |            |                 |               |                 |                 |       |         |           |             |
| **RestoreOnly**  | **22**         |  **3,085,113.6 ns** |  **58,796.46 ns** |    **54,998.25 ns** |  **3,082,384.4 ns** |  **1.00** |    **0.02** |         **-** |          **NA** |
| MeasureAll   | 22         | 10,108,830.6 ns | 193,750.82 ns |   171,755.06 ns | 10,132,674.2 ns |  3.28 |    0.08 |         - |          NA |
| MeasureQubit | 22         | 10,444,341.6 ns | 203,890.30 ns |   340,654.91 ns | 10,618,582.0 ns |  3.39 |    0.12 |         - |          NA |
|              |            |                 |               |                 |                 |       |         |           |             |
| **RestoreOnly**  | **24**         | **12,628,385.9 ns** | **250,709.52 ns** |   **317,067.30 ns** | **12,502,851.6 ns** |  **1.00** |    **0.03** |         **-** |          **NA** |
| MeasureAll   | 24         | 41,262,240.7 ns | 823,563.90 ns | 2,240,566.49 ns | 41,101,666.7 ns |  3.27 |    0.19 |         - |          NA |
| MeasureQubit | 24         | 45,722,716.1 ns | 384,335.67 ns |   359,507.83 ns | 45,707,950.0 ns |  3.62 |    0.09 |         - |          NA |

| Method            | Shots  | Mean        | Error     | StdDev    | Gen0       | Allocated   |
|------------------ |------- |------------:|----------:|----------:|-----------:|------------:|
| **SampleBellCircuit** | **1000**   |    **193.1 μs** |   **3.64 μs** |   **3.90 μs** |   **231.6895** |   **946.88 KB** |
| **SampleBellCircuit** | **10000**  |  **1,919.8 μs** |  **36.35 μs** |  **38.90 μs** |  **2314.4531** |   **9454.7 KB** |
| **SampleBellCircuit** | **100000** | **20,183.9 μs** | **392.57 μs** | **367.21 μs** | **23125.0000** | **94532.82 KB** |

| Method         | Mean     | Error   | StdDev  | Gen0   | Allocated |
|--------------- |---------:|--------:|--------:|-------:|----------:|
| EstimateBellZZ | 253.4 ns | 2.87 ns | 2.69 ns | 0.3424 |    1.4 KB |


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
