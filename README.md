# Topoi Quantum

[![.NET CI](https://github.com/topoi-io/topoi-quantum/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/topoi-io/topoi-quantum/actions/workflows/dotnet.yml)

**Developer Hub:** [topoi-io.github.io/quantum](https://topoi-io.github.io/quantum/)

Topoi Quantum is a .NET 10 quantum-computing toolkit for building circuits, running local dense state-vector simulations, executing a practical subset of OpenQASM 3, sampling measurements, estimating Pauli observables, and drawing circuits in a terminal.

It is designed for education, demonstrations, testing, algorithm prototyping, and small-to-medium local experiments. It is not intended to replace GPU, distributed, tensor-network, cloud, or hardware-backed quantum platforms.

> **MVP status:** `0.1.0` is the first public MVP release of Topoi Quantum. APIs and package boundaries may evolve before `1.0.0`.

## Highlights

- Fluent C# circuit builder and reusable SDK
- Dense pure-state simulation with a 25-qubit safety limit
- Standard single-qubit, controlled, rotation, swap, and Toffoli gates
- Full-register and individual-qubit measurement with state collapse
- Sparse sampling that stores observed outcomes only
- Pauli expectation-value estimation
- Practical OpenQASM 3 parsing, execution, and export
- Custom and parameterised OpenQASM gates
- Measurement, reset, barrier, and classical-bit execution
- Unicode/text circuit drawing
- Interactive shell and installable `tq` .NET tool
- Seeded randomness for repeatable tests and cryptographic randomness by default
- NUnit regression tests, GitHub Actions CI, and BenchmarkDotNet benchmarks

## Requirements

- .NET 10 SDK
- Windows, macOS, or Linux

Check the installed SDK:

```bash
dotnet --version
```

## Installation

### Command-line tool

Install the Topoi Quantum command-line tool from NuGet.org:

```bash
dotnet tool install --global Topoi.Quantum.Tool --version 0.1.0
```

Run:

```bash
tq --help
```


### Core SDK

Install the core SDK from NuGet.org:

```bash
dotnet add package Topoi.Quantum --version 0.1.0
```

### OpenQASM support

Add OpenQASM support from NuGet.org:

```bash
dotnet add package Topoi.Quantum.OpenQasm --version 0.1.0
```

The remaining packages provide drawing, parsing, CLI support, and the packaged tool. See [Package structure](#package-structure).

## SDK quick start

Create a Bell circuit, sample it, and estimate the `ZZ` observable:

```csharp
using Topoi.Quantum;
using Topoi.Quantum.Primitives;

QuantumCircuit bell = QuantumToolkit
    .Circuit(2)
    .H(0)
    .CX(0, 1)
    .Build();

SamplerResult samples = QuantumToolkit.Sampler.Run(
    bell,
    shots: 1_000);

Console.WriteLine($"00: {samples.CountFor("00")}");
Console.WriteLine($"11: {samples.CountFor("11")}");
Console.WriteLine($"01: {samples.CountFor("01")}");
Console.WriteLine($"10: {samples.CountFor("10")}");

EstimatorResult estimate = QuantumToolkit.Estimator.Estimate(
    bell,
    new[]
    {
        new PauliTerm('Z', 0),
        new PauliTerm('Z', 1)
    });

Console.WriteLine($"<ZZ> = {estimate.Value}");
```

Expected behaviour:

- only `00` and `11` are observed
- `01` and `10` return zero counts
- `<ZZ>` is approximately `+1`

### Sampler result behaviour

`SamplerResult.Counts` and `SamplerResult.Probabilities` contain **observed outcomes only**. This prevents result storage from allocating all `2^n` possible bit strings.

Use these methods when querying a specific outcome:

```csharp
int count = samples.CountFor("01");
double probability = samples.ProbabilityFor("01");
```

They return zero when an outcome was not observed.

## Command-line quick start

Create `bell.qasm`:

```qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;

h q[0];
cx q[0], q[1];
```

Run it:

```bash
tq --qasm bell.qasm --draw --print --expect "ZZ 0 1"
```

The repository includes the same circuit at [`examples/bell.qasm`](examples/bell.qasm).

### Executable OpenQASM example

The CLI `--qasm` path uses the executable OpenQASM model and supports measurement, classical bits, reset, and barrier operations.

```qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;
bit[2] c;

h q[0];
cx q[0], q[1];

c[0] = measure q[0];
c[1] = measure q[1];
```

Run:

```bash
tq --qasm measured-bell.qasm
```

The classical result should be either `00` or `11`.

## CLI usage

```text
tq [options]
```

| Option | Description |
|---|---|
| `--help`, `-h` | Show command-line help |
| `--qubits <n>`, `-q <n>` | Set the initial qubit count; default is 1 |
| `--qasm <path>` | Load and run an executable OpenQASM 3 program |
| `--openqasm <path>` | Alias for `--qasm` |
| `--run <path>` | Run a native interpreter script |
| `--circuit <path>` | Load and run a gate-only native circuit file |
| `--print-circuit` | Print the loaded circuit before execution |
| `--draw` | Draw the loaded circuit before execution |
| `--print` | Print final amplitudes and probabilities |
| `--probs` | Print final basis-state probabilities |
| `--expect "observable"` | Calculate a Pauli expectation value |
| `--sample <n>` | Sample the final state `n` times |

The CLI currently uses option syntax. Command-style forms such as `tq run file.qasm` are not implemented in the MVP.

### Interactive shell

Start the tool without arguments:

```bash
tq
```

Example session:

```text
H 0
CX 0 1
PRINT
EXPECT ZZ 0 1
SAMPLE 1000
```

Interactive commands include gates, measurement, sampling, expectation values, state diagnostics, native script execution, circuit loading, circuit drawing, and quantum random-bit generation. Type `HELP` in the shell for the complete list.

## Supported gates

### Core SDK and OpenQASM model

Single-qubit gates:

```text
I, X, Y, Z, H, S, SDG, T, TDG, SX, SXDG, RX, RY, RZ
```

Controlled and multi-qubit gates:

```text
CX, CY, CZ, CH, CP, SWAP, CCX, CRX, CRY, CRZ
```

`CNOT` is available as an alias for `CX`, and `TOFFOLI` is available as an alias for `CCX` where supported.

The interactive shell and native `.qc` parser expose a smaller command-oriented subset than the core SDK and OpenQASM layer.

## OpenQASM support

Topoi Quantum implements a practical subset of OpenQASM 3 rather than complete specification compliance.

Supported MVP capabilities include:

- `OPENQASM 3`, `3.0`, and `3.1`
- `include "stdgates.inc";`
- quantum and classical register declarations
- standard and parameterised gate calls
- angle expressions using `pi`, arithmetic, signs, and parentheses
- measurement assignment and legacy arrow syntax
- reset and barrier operations
- custom gate declarations
- parameterised and nested custom gates
- recursive custom-gate detection
- supported `ctrl @` and `inv @` modifiers
- gate-only and executable program models
- export of supported circuits and executable programs

The `pow` and `negctrl` modifiers are not executable in the MVP. Timing, calibration, physical-qubit declarations, aliases, and general classical control flow are also outside the current scope.

## Package structure

| Package | Purpose |
|---|---|
| [`Topoi.Quantum`](https://www.nuget.org/packages/Topoi.Quantum/) | Core simulator, circuits, gates, sampler, estimator, observables, and randomness |
| [`Topoi.Quantum.OpenQasm`](https://www.nuget.org/packages/Topoi.Quantum.OpenQasm/) | OpenQASM parsing, conversion, execution, and export |
| [`Topoi.Quantum.Drawing`](https://www.nuget.org/packages/Topoi.Quantum.Drawing/) | Unicode/text circuit drawing |
| [`Topoi.Quantum.Parsing`](https://www.nuget.org/packages/Topoi.Quantum.Parsing/) | Native `.qc`, angle, gate, and observable parsing |
| [`Topoi.Quantum.Cli`](https://www.nuget.org/packages/Topoi.Quantum.Cli/) | CLI option parsing, interactive execution, help, and console output |
| [`Topoi.Quantum.Tool`](https://www.nuget.org/packages/Topoi.Quantum.Tool/) | Installable .NET tool exposing the `tq` command |

Most SDK users need `Topoi.Quantum`, adding `Topoi.Quantum.OpenQasm` when OpenQASM support is required. Tool users normally install only `Topoi.Quantum.Tool`; its required assemblies are bundled into the tool package.

## Dense state-vector limits

A dense simulator stores `2^n` complex amplitudes, so memory and gate execution grow exponentially with qubit count.

Topoi Quantum enforces a default safety limit of 25 qubits.

| Qubits | Approximate amplitude-array memory |
|---:|---:|
| 20 | 16 MiB |
| 21 | 32 MiB |
| 22 | 64 MiB |
| 23 | 128 MiB |
| 24 | 256 MiB |
| 25 | 512 MiB |

These figures cover the amplitude array only. Applications also require memory for the runtime, circuits, output, sampling results, and other objects.

## Current limitations

- Dense pure-state simulation only
- Default maximum of 25 qubits
- No density-matrix, noise, or decoherence model
- No transpiler or optimisation pass
- No GPU, distributed, or tensor-network backend
- No cloud-provider or quantum-hardware integration
- No SVG or PNG circuit rendering
- Practical OpenQASM subset rather than complete compliance
- No executable OpenQASM `pow` or `negctrl` modifiers
- No general OpenQASM classical control flow
- Command-line option syntax rather than subcommands

## Build from source

```bash
git clone https://github.com/topoi-io/topoi-quantum.git
cd topoi-quantum

dotnet restore Topoi.Quantum.slnx
dotnet build Topoi.Quantum.slnx --configuration Release --no-restore
dotnet test Topoi.Quantum.Tests/Topoi.Quantum.Tests.csproj --configuration Release --no-build
```

Run the tool from source:

```bash
dotnet run --project Topoi.Quantum.Tool -- --help
```

Run the Bell example from source:

```bash
dotnet run --project Topoi.Quantum.Tool -- --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
```

## Release packaging

Package versions and common metadata are managed in `Directory.Build.props`.

Create the release packages:

```bash
dotnet pack Topoi.Quantum.slnx \
  --configuration Release \
  --output artifacts/packages
```

Before publishing a release, validate the generated SDK packages from clean consumer projects and install `Topoi.Quantum.Tool` from an isolated local package source.

## Testing and CI

The NUnit test suite covers the core simulator, gates, circuit validation, measurement, sparse sampling, estimators, drawing, native parsing, and OpenQASM parsing and execution.

GitHub Actions currently performs:

```text
restore -> Release build -> NUnit tests
```

Run the full suite locally:

```bash
dotnet test Topoi.Quantum.slnx --configuration Release
```

## Benchmarks

Run BenchmarkDotNet:

```bash
dotnet run --project Topoi.Quantum.Benchmarks --configuration Release
```

Recorded results:

- [Windows ARM64 benchmarks](docs/benchmarks/BENCHMARKS-Windows-ARM64.md)
- [macOS ARM64 benchmarks](docs/benchmarks/BENCHMARKS-macOS-ARM64.md)

Benchmark results are machine-specific and should be interpreted as comparative measurements rather than universal performance guarantees.

## Versioning, changes, and security

Topoi Quantum follows Semantic Versioning.

- See [CHANGELOG.md](CHANGELOG.md) for notable release changes.
- See [SECURITY.md](SECURITY.md) for supported versions and private vulnerability reporting.

## Contributing

Contributions should preserve correctness, clarity, testability, package usability, and the independence of the core SDK from CLI and presentation concerns.

Before submitting a change:

```bash
dotnet build Topoi.Quantum.slnx --configuration Release
dotnet test Topoi.Quantum.slnx --configuration Release
```

Bug fixes and new gates should include regression tests. Security-sensitive issues should follow `SECURITY.md` rather than being disclosed in a public issue.

## License

Topoi Quantum is licensed under the [MIT License](LICENSE).
