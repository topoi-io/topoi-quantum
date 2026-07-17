# Topoi.Quantum Benchmarks - macOS ARM64

## Scope

Topoi.Quantum is a local dense state-vector simulator intended for learning,
experimentation, OpenQASM workflows, and small-to-medium circuit prototyping.
It is not positioned as a GPU, distributed, tensor-network, or cloud-hardware backend.

## Environment

- OS: macOS Sequoia 15.7.7 (24G720) [Darwin 24.6.0]
- CPU: Apple M2 Max, 1 CPU, 12 logical and 12 physical cores
- Architecture: macOS ARM64
- RAM: 64 GB
- .NET SDK: 10.0.103
- .NET Runtime: .NET 10.0.3 (10.0.3, 10.0.326.7603), Arm64 RyuJIT armv8.0-a
- BenchmarkDotNet: v0.15.8
- Commit:
- Build configuration: Release
- Power mode: Plugged in
- Plugged in: Yes

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

| Method              | QubitCount | Mean          | Error        | StdDev       | Allocated |
|-------------------- |----------- |--------------:|-------------:|-------------:|----------:|
| **ApplyHOverAllQubits** | **10**         |      **76.09 μs** |     **1.968 μs** |     **5.679 μs** |         **-** |
| **ApplyHOverAllQubits** | **12**         |     **204.63 μs** |     **4.401 μs** |    **12.557 μs** |         **-** |
| **ApplyHOverAllQubits** | **16**         |   **1,643.48 μs** |    **32.547 μs** |    **79.223 μs** |         **-** |
| **ApplyHOverAllQubits** | **20**         |  **24,126.82 μs** |   **476.893 μs** |   **783.549 μs** |         **-** |
| **ApplyHOverAllQubits** | **22**         | **108,889.14 μs** | **1,785.812 μs** | **1,670.450 μs** |         **-** |
| **ApplyHOverAllQubits** | **24**         | **456,288.32 μs** | **2,146.386 μs** | **1,902.715 μs** |         **-** |

## Gate benchmarks

Run commands:

```bash
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*SingleQubitGate*"
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*ControlledGate*"
```

Expected benchmark classes:

- `SingleQubitGateBenchmarks`
- `ControlledGateBenchmarks`

| Method                  | QubitCount | Gate | Mean          | Error        | StdDev       | Median        | Allocated |
|------------------------ |----------- |----- |--------------:|-------------:|-------------:|--------------:|----------:|
| **ApplyGateAcrossRegister** | **10**         | **X**    |      **73.16 μs** |     **1.929 μs** |     **5.565 μs** |      **71.44 μs** |         **-** |
| **ApplyGateAcrossRegister** | **10**         | **Z**    |      **75.98 μs** |     **2.190 μs** |     **6.285 μs** |      **75.75 μs** |         **-** |
| **ApplyGateAcrossRegister** | **10**         | **H**    |      **75.03 μs** |     **1.499 μs** |     **3.619 μs** |      **74.42 μs** |         **-** |
| **ApplyGateAcrossRegister** | **10**         | **RX**   |      **75.73 μs** |     **2.509 μs** |     **7.278 μs** |      **73.50 μs** |         **-** |
| **ApplyGateAcrossRegister** | **10**         | **RY**   |      **75.38 μs** |     **2.274 μs** |     **6.634 μs** |      **75.10 μs** |         **-** |
| **ApplyGateAcrossRegister** | **10**         | **RZ**   |      **75.47 μs** |     **2.398 μs** |     **6.996 μs** |      **73.13 μs** |         **-** |
| **ApplyGateAcrossRegister** | **12**         | **X**    |     **201.53 μs** |     **4.015 μs** |    **11.193 μs** |     **200.19 μs** |         **-** |
| **ApplyGateAcrossRegister** | **12**         | **Z**    |     **204.02 μs** |     **5.014 μs** |    **14.705 μs** |     **201.08 μs** |         **-** |
| **ApplyGateAcrossRegister** | **12**         | **H**    |     **203.01 μs** |     **5.093 μs** |    **14.776 μs** |     **202.17 μs** |         **-** |
| **ApplyGateAcrossRegister** | **12**         | **RX**   |     **200.71 μs** |     **3.996 μs** |    **10.598 μs** |     **199.31 μs** |         **-** |
| **ApplyGateAcrossRegister** | **12**         | **RY**   |     **202.04 μs** |     **4.166 μs** |    **12.153 μs** |     **201.02 μs** |         **-** |
| **ApplyGateAcrossRegister** | **12**         | **RZ**   |     **197.46 μs** |     **3.926 μs** |    **10.613 μs** |     **197.71 μs** |         **-** |
| **ApplyGateAcrossRegister** | **16**         | **X**    |   **1,364.78 μs** |    **95.660 μs** |   **282.056 μs** |   **1,478.75 μs** |         **-** |
| **ApplyGateAcrossRegister** | **16**         | **Z**    |   **1,427.84 μs** |    **76.017 μs** |   **224.137 μs** |   **1,501.10 μs** |         **-** |
| **ApplyGateAcrossRegister** | **16**         | **H**    |   **1,419.73 μs** |    **80.280 μs** |   **236.708 μs** |   **1,517.87 μs** |         **-** |
| **ApplyGateAcrossRegister** | **16**         | **RX**   |   **1,476.23 μs** |    **48.623 μs** |   **142.604 μs** |   **1,484.46 μs** |         **-** |
| **ApplyGateAcrossRegister** | **16**         | **RY**   |   **1,463.21 μs** |    **73.110 μs** |   **215.566 μs** |   **1,518.02 μs** |         **-** |
| **ApplyGateAcrossRegister** | **16**         | **RZ**   |   **1,429.32 μs** |    **76.130 μs** |   **224.472 μs** |   **1,504.48 μs** |         **-** |
| **ApplyGateAcrossRegister** | **20**         | **X**    |  **21,572.33 μs** |   **422.738 μs** |   **863.543 μs** |  **21,267.52 μs** |         **-** |
| **ApplyGateAcrossRegister** | **20**         | **Z**    |  **23,303.60 μs** |   **416.853 μs** |   **842.064 μs** |  **23,177.58 μs** |         **-** |
| **ApplyGateAcrossRegister** | **20**         | **H**    |  **23,812.93 μs** |   **474.783 μs** |   **818.977 μs** |  **23,697.62 μs** |         **-** |
| **ApplyGateAcrossRegister** | **20**         | **RX**   |  **27,543.43 μs** |   **514.331 μs** |   **481.106 μs** |  **27,496.96 μs** |         **-** |
| **ApplyGateAcrossRegister** | **20**         | **RY**   |  **25,483.09 μs** |   **498.639 μs** |   **873.327 μs** |  **25,239.00 μs** |         **-** |
| **ApplyGateAcrossRegister** | **20**         | **RZ**   |  **24,971.30 μs** |   **498.645 μs** |   **648.380 μs** |  **24,923.65 μs** |         **-** |
| **ApplyGateAcrossRegister** | **22**         | **X**    |  **98,567.04 μs** | **1,818.343 μs** | **1,700.879 μs** |  **98,635.38 μs** |         **-** |
| **ApplyGateAcrossRegister** | **22**         | **Z**    | **104,133.50 μs** |   **782.792 μs** |   **611.153 μs** | **104,104.44 μs** |         **-** |
| **ApplyGateAcrossRegister** | **22**         | **H**    | **107,166.66 μs** | **1,023.145 μs** |   **957.050 μs** | **107,338.83 μs** |         **-** |
| **ApplyGateAcrossRegister** | **22**         | **RX**   | **124,115.85 μs** | **1,238.120 μs** | **1,097.561 μs** | **124,150.56 μs** |         **-** |
| **ApplyGateAcrossRegister** | **22**         | **RY**   | **114,936.94 μs** | **1,650.085 μs** | **1,543.490 μs** | **115,060.46 μs** |         **-** |
| **ApplyGateAcrossRegister** | **22**         | **RZ**   | **113,068.46 μs** | **1,325.209 μs** | **1,239.601 μs** | **113,277.21 μs** |         **-** |
| **ApplyGateAcrossRegister** | **24**         | **X**    | **421,737.43 μs** | **6,418.634 μs** | **6,003.994 μs** | **419,658.00 μs** |         **-** |
| **ApplyGateAcrossRegister** | **24**         | **Z**    | **446,130.16 μs** | **2,865.849 μs** | **2,680.717 μs** | **446,704.50 μs** |         **-** |
| **ApplyGateAcrossRegister** | **24**         | **H**    | **467,364.93 μs** | **2,373.260 μs** | **2,103.833 μs** | **467,895.71 μs** |         **-** |
| **ApplyGateAcrossRegister** | **24**         | **RX**   | **541,955.62 μs** | **4,455.360 μs** | **4,167.546 μs** | **542,502.00 μs** |         **-** |
| **ApplyGateAcrossRegister** | **24**         | **RY**   | **486,894.27 μs** | **2,632.267 μs** | **2,462.224 μs** | **486,919.67 μs** |      **32 B** |
| **ApplyGateAcrossRegister** | **24**         | **RZ**   | **494,818.44 μs** | **6,342.284 μs** | **5,932.576 μs** | **494,301.96 μs** |         **-** |

| Method                           | QubitCount | Mean          | Error        | StdDev       | Median        | Allocated |
|--------------------------------- |----------- |--------------:|-------------:|-------------:|--------------:|----------:|
| **CXAcrossRegister**                 | **10**         |      **94.51 μs** |     **3.249 μs** |     **9.374 μs** |      **91.46 μs** |    **2808 B** |
| CZAcrossRegister                 | 10         |      78.39 μs |     1.959 μs |     5.525 μs |      77.38 μs |         - |
| CCXAcrossRegister                | 10         |      87.62 μs |     2.965 μs |     8.741 μs |      86.23 μs |    2496 B |
| ControlledRotationAcrossRegister | 10         |      96.13 μs |     3.167 μs |     9.188 μs |      94.25 μs |    2808 B |
| **CXAcrossRegister**                 | **12**         |     **157.49 μs** |     **4.029 μs** |    **11.623 μs** |     **154.62 μs** |    **3432 B** |
| CZAcrossRegister                 | 12         |     119.57 μs |     2.205 μs |     4.353 μs |     118.88 μs |         - |
| CCXAcrossRegister                | 12         |     141.67 μs |     4.171 μs |    11.969 μs |     138.08 μs |    3120 B |
| ControlledRotationAcrossRegister | 12         |     155.20 μs |     4.491 μs |    13.028 μs |     152.21 μs |    3432 B |
| **CXAcrossRegister**                 | **16**         |   **1,174.60 μs** |    **52.504 μs** |   **153.158 μs** |   **1,217.11 μs** |    **4680 B** |
| CZAcrossRegister                 | 16         |     786.32 μs |    25.249 μs |    72.850 μs |     780.61 μs |         - |
| CCXAcrossRegister                | 16         |     954.15 μs |    65.892 μs |   193.251 μs |     856.67 μs |    3472 B |
| ControlledRotationAcrossRegister | 16         |   1,103.82 μs |    50.626 μs |   148.477 μs |   1,051.88 μs |    4680 B |
| **CXAcrossRegister**                 | **20**         |  **21,270.65 μs** |   **389.393 μs** |   **382.436 μs** |  **21,258.62 μs** |    **5928 B** |
| CZAcrossRegister                 | 20         |  16,460.66 μs |   327.634 μs |   646.717 μs |  16,415.40 μs |         - |
| CCXAcrossRegister                | 20         |  16,366.17 μs |   276.620 μs |   230.990 μs |  16,338.54 μs |    4464 B |
| ControlledRotationAcrossRegister | 20         |  21,542.54 μs |   407.005 μs |   399.734 μs |  21,561.27 μs |    5928 B |
| **CXAcrossRegister**                 | **22**         |  **96,966.91 μs** | **1,364.483 μs** | **1,276.338 μs** |  **96,359.71 μs** |    **6552 B** |
| CZAcrossRegister                 | 22         |  78,446.58 μs | 1,532.187 μs | 1,937.727 μs |  78,054.96 μs |         - |
| CCXAcrossRegister                | 22         |  78,518.34 μs | 1,457.717 μs | 1,363.550 μs |  78,360.25 μs |    4960 B |
| ControlledRotationAcrossRegister | 22         |  98,995.44 μs | 1,621.396 μs | 1,516.655 μs |  99,450.46 μs |    6552 B |
| **CXAcrossRegister**                 | **24**         | **418,996.57 μs** | **4,212.701 μs** | **3,940.563 μs** | **418,227.42 μs** |    **7176 B** |
| CZAcrossRegister                 | 24         | 336,251.07 μs | 3,319.115 μs | 2,942.309 μs | 336,817.79 μs |         - |
| CCXAcrossRegister                | 24         | 340,344.43 μs | 3,898.522 μs | 3,646.680 μs | 340,975.00 μs |    5456 B |
| ControlledRotationAcrossRegister | 24         | 427,931.12 μs | 3,839.773 μs | 3,403.859 μs | 427,635.58 μs |    7176 B |

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

| Method             | Mean     | Error     | StdDev    | Allocated |
|------------------- |---------:|----------:|----------:|----------:|
| ExecuteBellCircuit | 2.527 μs | 0.2505 μs | 0.7188 μs |     904 B |

| Method            | QubitCount | Mean          | Error        | StdDev       | Allocated |
|------------------ |----------- |--------------:|-------------:|-------------:|----------:|
| **ExecuteGhzCircuit** | **8**          |      **33.46 μs** |     **1.691 μs** |     **4.879 μs** |   **4.45 KB** |
| **ExecuteGhzCircuit** | **12**         |     **173.09 μs** |     **4.889 μs** |    **13.948 μs** |   **6.82 KB** |
| **ExecuteGhzCircuit** | **16**         |   **1,297.95 μs** |    **61.403 μs** |   **177.162 μs** |    **9.2 KB** |
| **ExecuteGhzCircuit** | **20**         |  **21,930.91 μs** |   **310.869 μs** |   **501.995 μs** |  **11.57 KB** |
| **ExecuteGhzCircuit** | **22**         | **101,050.23 μs** | **1,058.605 μs** |   **990.220 μs** |  **12.76 KB** |
| **ExecuteGhzCircuit** | **24**         | **429,170.61 μs** | **6,032.801 μs** | **5,643.086 μs** |  **13.95 KB** |

| Method                      | QubitCount | Depth | Mean            | Error         | StdDev        | Median          | Allocated |
|---------------------------- |----------- |------ |----------------:|--------------:|--------------:|----------------:|----------:|
| **ExecuteRandomShallowCircuit** | **8**          | **2**     |        **67.19 μs** |      **2.107 μs** |      **6.078 μs** |        **65.75 μs** |   **9.38 KB** |
| **ExecuteRandomShallowCircuit** | **8**          | **4**     |       **120.00 μs** |      **4.271 μs** |     **12.527 μs** |       **116.42 μs** |  **18.75 KB** |
| **ExecuteRandomShallowCircuit** | **8**          | **8**     |       **215.73 μs** |      **5.303 μs** |     **14.869 μs** |       **215.67 μs** |   **37.5 KB** |
| **ExecuteRandomShallowCircuit** | **12**         | **2**     |       **573.04 μs** |     **11.201 μs** |     **28.101 μs** |       **571.06 μs** |  **14.06 KB** |
| **ExecuteRandomShallowCircuit** | **12**         | **4**     |     **1,110.60 μs** |     **21.965 μs** |     **55.908 μs** |     **1,113.17 μs** |  **28.13 KB** |
| **ExecuteRandomShallowCircuit** | **12**         | **8**     |     **2,267.51 μs** |     **43.855 μs** |     **81.289 μs** |     **2,270.46 μs** |  **56.25 KB** |
| **ExecuteRandomShallowCircuit** | **16**         | **2**     |     **4,352.42 μs** |     **85.275 μs** |     **98.202 μs** |     **4,374.48 μs** |  **18.75 KB** |
| **ExecuteRandomShallowCircuit** | **16**         | **4**     |     **6,595.47 μs** |    **104.530 μs** |    **213.527 μs** |     **6,538.33 μs** |   **37.5 KB** |
| **ExecuteRandomShallowCircuit** | **16**         | **8**     |    **13,564.02 μs** |    **271.131 μs** |    **535.187 μs** |    **13,493.58 μs** |     **75 KB** |
| **ExecuteRandomShallowCircuit** | **20**         | **2**     |    **69,499.79 μs** |  **1,383.225 μs** |  **2,153.514 μs** |    **68,982.23 μs** |  **23.44 KB** |
| **ExecuteRandomShallowCircuit** | **20**         | **4**     |   **139,491.32 μs** |  **2,780.443 μs** |  **3,987.628 μs** |   **139,684.88 μs** |  **46.88 KB** |
| **ExecuteRandomShallowCircuit** | **20**         | **8**     |   **293,507.91 μs** |  **2,305.163 μs** |  **2,156.251 μs** |   **294,203.67 μs** |  **93.75 KB** |
| **ExecuteRandomShallowCircuit** | **22**         | **2**     |   **319,449.90 μs** |  **6,226.734 μs** |  **5,519.838 μs** |   **318,732.58 μs** |  **25.78 KB** |
| **ExecuteRandomShallowCircuit** | **22**         | **4**     |   **646,216.81 μs** | **10,257.894 μs** |  **9,595.240 μs** |   **649,744.50 μs** |  **51.56 KB** |
| **ExecuteRandomShallowCircuit** | **22**         | **8**     | **1,311,104.14 μs** | **18,386.353 μs** | **17,198.606 μs** | **1,317,027.08 μs** | **103.13 KB** |

## OpenQASM benchmarks

Run command:

```bash
dotnet run -c Release --project Topoi.Quantum.Benchmarks -- --filter "*OpenQasm*"
```

Expected benchmark class:

- `OpenQasmBenchmarks`

| Method                          | QubitCount | Mean          | Error       | StdDev      | Gen0     | Gen1     | Gen2     | Allocated   |
|-------------------------------- |----------- |--------------:|------------:|------------:|---------:|---------:|---------:|------------:|
| **ParseGateOnlyQasm**               | **2**          |      **1.304 μs** |   **0.0050 μs** |   **0.0044 μs** |   **0.5951** |   **0.0019** |        **-** |     **4.87 KB** |
| ParseAndExecuteGateOnlyQasm     | 2          |      1.554 μs |   0.0098 μs |   0.0087 μs |   0.7229 |   0.0038 |        - |     5.91 KB |
| ParseExecutableQasm             | 2          |      1.971 μs |   0.0122 μs |   0.0114 μs |   0.8698 |   0.0076 |        - |     7.13 KB |
| ParseAndExecuteExecutableQasm   | 2          |      2.609 μs |   0.0079 μs |   0.0070 μs |   1.0490 |   0.0114 |        - |     8.58 KB |
| ParseAndExecuteCustomGateQasm   | 2          |      3.311 μs |   0.0113 μs |   0.0106 μs |   1.5068 |   0.0267 |        - |    12.32 KB |
| ParseAndExecuteFourBitAdderQasm | 2          |     83.618 μs |   0.3835 μs |   0.3587 μs |  11.7188 |   0.8545 |        - |    96.49 KB |
| **ParseGateOnlyQasm**               | **8**          |      **4.102 μs** |   **0.0191 μs** |   **0.0169 μs** |   **1.8845** |   **0.0381** |        **-** |    **15.45 KB** |
| ParseAndExecuteGateOnlyQasm     | 8          |      7.502 μs |   0.0273 μs |   0.0242 μs |   2.9297 |   0.0610 |        - |    23.99 KB |
| ParseExecutableQasm             | 8          |      6.219 μs |   0.0332 μs |   0.0310 μs |   2.7695 |   0.0992 |        - |    22.65 KB |
| ParseAndExecuteExecutableQasm   | 8          |     13.912 μs |   0.0532 μs |   0.0472 μs |   3.8605 |   0.1221 |        - |    31.62 KB |
| ParseAndExecuteCustomGateQasm   | 8          |      3.257 μs |   0.0112 μs |   0.0104 μs |   1.5068 |   0.0267 |        - |    12.32 KB |
| ParseAndExecuteFourBitAdderQasm | 8          |     82.005 μs |   0.2376 μs |   0.2223 μs |  11.7188 |   0.8545 |        - |    96.49 KB |
| **ParseGateOnlyQasm**               | **16**         |      **7.287 μs** |   **0.0445 μs** |   **0.0416 μs** |   **3.6163** |   **0.1450** |        **-** |    **29.58 KB** |
| ParseAndExecuteGateOnlyQasm     | 16         |  1,261.172 μs |   5.3388 μs |   4.4581 μs | 332.0313 | 332.0313 | 332.0313 |  1062.98 KB |
| ParseExecutableQasm             | 16         |     12.187 μs |   0.0685 μs |   0.0641 μs |   5.2948 |   0.3510 |        - |    43.38 KB |
| ParseAndExecuteExecutableQasm   | 16         |  3,388.664 μs |  40.4236 μs |  37.8123 μs | 332.0313 | 332.0313 | 332.0313 |  1077.23 KB |
| ParseAndExecuteCustomGateQasm   | 16         |      3.304 μs |   0.0201 μs |   0.0188 μs |   1.5068 |   0.0267 |        - |    12.32 KB |
| ParseAndExecuteFourBitAdderQasm | 16         |     84.180 μs |   0.2378 μs |   0.2108 μs |  11.7188 |   0.8545 |        - |    96.49 KB |
| **ParseGateOnlyQasm**               | **20**         |      **9.257 μs** |   **0.0596 μs** |   **0.0558 μs** |   **4.3488** |   **0.1984** |        **-** |    **35.57 KB** |
| ParseAndExecuteGateOnlyQasm     | 20         | 24,570.074 μs | 178.7385 μs | 167.1921 μs | 500.0000 | 500.0000 | 500.0000 | 16431.39 KB |
| ParseExecutableQasm             | 20         |     15.293 μs |   0.0883 μs |   0.0783 μs |   6.3171 |   0.5188 |        - |    51.62 KB |
| ParseAndExecuteExecutableQasm   | 20         | 67,888.867 μs | 517.1350 μs | 483.7284 μs | 500.0000 | 500.0000 | 500.0000 | 16447.91 KB |
| ParseAndExecuteCustomGateQasm   | 20         |      3.316 μs |   0.0133 μs |   0.0125 μs |   1.5068 |   0.0267 |        - |    12.32 KB |
| ParseAndExecuteFourBitAdderQasm | 20         |     84.575 μs |   0.3800 μs |   0.3554 μs |  11.7188 |   0.8545 |        - |    96.49 KB |

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

| Method       | QubitCount | Mean            | Error         | StdDev          | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------- |----------- |----------------:|--------------:|----------------:|------:|--------:|----------:|------------:|
| **RestoreOnly**  | **10**         |        **220.1 ns** |       **1.04 ns** |         **0.97 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| MeasureAll   | 10         |      1,871.9 ns |       3.73 ns |         3.30 ns |  8.51 |    0.04 |         - |          NA |
| MeasureQubit | 10         |      2,231.5 ns |       8.44 ns |         7.04 ns | 10.14 |    0.05 |         - |          NA |
|              |            |                 |               |                 |       |         |           |             |
| **RestoreOnly**  | **12**         |      **1,058.0 ns** |      **20.60 ns** |        **55.71 ns** |  **1.00** |    **0.07** |         **-** |          **NA** |
| MeasureAll   | 12         |      7,880.1 ns |     120.26 ns |       112.49 ns |  7.47 |    0.40 |         - |          NA |
| MeasureQubit | 12         |      9,013.7 ns |      66.05 ns |        58.55 ns |  8.54 |    0.44 |         - |          NA |
|              |            |                 |               |                 |       |         |           |             |
| **RestoreOnly**  | **16**         |     **21,894.8 ns** |     **243.15 ns** |       **227.45 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| MeasureAll   | 16         |    116,962.9 ns |     414.69 ns |       367.61 ns |  5.34 |    0.06 |         - |          NA |
| MeasureQubit | 16         |    137,451.9 ns |     587.07 ns |       549.15 ns |  6.28 |    0.07 |         - |          NA |
|              |            |                 |               |                 |       |         |           |             |
| **RestoreOnly**  | **20**         |    **408,339.5 ns** |   **8,145.41 ns** |    **14,894.34 ns** |  **1.00** |    **0.05** |         **-** |          **NA** |
| MeasureAll   | 20         |  2,028,995.1 ns |  40,174.60 ns |    47,825.00 ns |  4.98 |    0.21 |         - |          NA |
| MeasureQubit | 20         |  2,714,120.3 ns |  37,745.25 ns |    35,306.93 ns |  6.66 |    0.25 |         - |          NA |
|              |            |                 |               |                 |       |         |           |             |
| **RestoreOnly**  | **22**         |  **1,535,023.6 ns** |  **14,289.30 ns** |    **11,932.21 ns** |  **1.00** |    **0.01** |         **-** |          **NA** |
| MeasureAll   | 22         |  8,172,232.1 ns | 159,049.66 ns |   189,337.33 ns |  5.32 |    0.13 |         - |          NA |
| MeasureQubit | 22         |  9,384,314.7 ns | 138,389.86 ns |   129,449.96 ns |  6.11 |    0.09 |         - |          NA |
|              |            |                 |               |                 |       |         |           |             |
| **RestoreOnly**  | **24**         |  **6,206,817.3 ns** | **118,898.77 ns** |   **127,220.36 ns** |  **1.00** |    **0.03** |         **-** |          **NA** |
| MeasureAll   | 24         | 32,257,617.2 ns | 643,619.75 ns | 1,300,143.75 ns |  5.20 |    0.23 |         - |          NA |
| MeasureQubit | 24         | 36,795,555.0 ns | 439,392.69 ns |   411,008.19 ns |  5.93 |    0.14 |         - |          NA |

| Method            | Shots  | Mean        | Error    | StdDev   | Gen0       | Gen1   | Allocated   |
|------------------ |------- |------------:|---------:|---------:|-----------:|-------:|------------:|
| **SampleBellCircuit** | **1000**   |    **191.0 μs** |  **0.68 μs** |  **0.63 μs** |   **115.7227** | **0.2441** |   **946.88 KB** |
| **SampleBellCircuit** | **10000**  |  **1,921.9 μs** |  **8.82 μs** |  **7.82 μs** |  **1156.2500** | **3.9063** |   **9454.7 KB** |
| **SampleBellCircuit** | **100000** | **19,204.0 μs** | **42.23 μs** | **37.43 μs** | **11562.5000** |      **-** | **94532.82 KB** |

| Method         | Mean     | Error   | StdDev  | Gen0   | Allocated |
|--------------- |---------:|--------:|--------:|-------:|----------:|
| EstimateBellZZ | 244.7 ns | 2.00 ns | 1.67 ns | 0.1712 |    1.4 KB |

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
