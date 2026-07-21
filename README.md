# Topoi Quantum

[![.NET CI](https://github.com/topoi-io/topoi-quantum/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/topoi-io/topoi-quantum/actions/workflows/dotnet.yml)

**A professional .NET quantum toolkit for circuit building, dense state-vector simulation, OpenQASM workflows, sampling, estimation, circuit drawing, command-line execution, and quantum-computing education.**

Topoi Quantum provides a local quantum-computing laboratory and SDK for learning, prototyping, testing, and demonstrating quantum circuits in C# and .NET.

The current implementation is intentionally focused on **clarity, correctness, educational value, professional API design, and inspectable simulator architecture**. It is not intended to replace high-performance production simulators, tensor-network simulators, GPU simulators, distributed simulators, or real quantum hardware backends.

---

## Naming

| Area | Current name |
|---|---|
| Product | **Topoi Quantum** |
| Repository | `topoi-quantum` |
| Solution | `Topoi.Quantum.slnx` |
| Main SDK project/package ID | `Topoi.Quantum` |
| CLI tool project/package ID | `Topoi.Quantum.Tool` |
| Packaged tool command | `tq` |

The global-tool project is already configured in source as `Topoi.Quantum.Tool` with `PackAsTool=true` and `ToolCommandName=tq`.

---

## Current Implementation Status

| Area | Status in the codebase |
|---|---|
| Core SDK project | Implemented as `Topoi.Quantum` targeting `net10.0` |
| SDK package metadata | Implemented: package ID, version, authors, MIT license expression, repository URL, tags, and XML documentation generation |
| CLI/global tool project | Implemented as `Topoi.Quantum.Tool`; packable as a .NET global tool command named `tq` |
| CLI option syntax | Implemented with `tq [options]`; command-style syntax such as `tq run ...` is not currently implemented |
| Dense state-vector simulator | Implemented with a default safety limit of 25 qubits |
| Core gate model | Implemented through `GateKind`, `GateOperation`, `GateSpecs`, `QuantumSimulator`, and `QuantumRegister` |
| Fluent circuit builder | Implemented through `QuantumCircuitBuilder` |
| SDK primitives | Implemented through `Sampler`, `SamplerResult`, `Estimator`, and `EstimatorResult` |
| OpenQASM parser/importer | Implemented for a practical OpenQASM 3.x subset |
| OpenQASM custom gates | Implemented, including parameterized and nested custom-gate expansion with recursive-definition detection |
| OpenQASM executable model | Implemented for measurement, reset, barrier, classical-bit storage, and executable export |
| Current CLI `--qasm` path | Loads OpenQASM into a gate-only `QuantumCircuit`; full executable OpenQASM behaviour is available through the SDK executable API, not yet wired into the CLI path |
| Circuit drawing | Implemented as Unicode/text drawing through `Topoi.Quantum.Drawing` |
| SVG/PNG drawing | Not implemented |
| Tests | Implemented with NUnit, NUnit analyzers, NUnit3 test adapter, and coverlet collector |
| GitHub Actions CI | Implemented with `.NET CI` workflow for restore, Release build, and NUnit tests |
| Benchmarks | Not currently present in the repository |
| Noise/density-matrix simulation | Not currently implemented |
| Hardware/cloud backend integration | Not currently implemented |

---

## Intended Use

Topoi Quantum is suitable for:

- quantum-computing lab exercises
- university quantum-computing modules
- C# and .NET quantum software teaching
- OpenQASM experimentation
- local simulation of small-to-medium quantum circuits
- algorithm prototyping before moving to cloud quantum services
- teaching Bell states, GHZ states, measurement, sampling, and Pauli observables
- demonstrating how simulator internals work
- test-driven quantum software development

---

## Contents

- [Naming](#naming)
- [Current Implementation Status](#current-implementation-status)
- [Intended Use](#intended-use)
- [Requirements](#requirements)
- [Quick Start](#quick-start)
- [SDK Quick Start](#sdk-quick-start)
- [Core SDK Features](#core-sdk-features)
- [Supported Gates](#supported-gates)
- [Sampler Primitive](#sampler-primitive)
- [Estimator Primitive](#estimator-primitive)
- [Command-Line Usage](#command-line-usage)
- [Interactive REPL](#interactive-repl)
- [OpenQASM Workflows](#openqasm-workflows)
- [Circuit Drawing](#circuit-drawing)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Testing](#testing)
- [Mathematical Model](#mathematical-model)
- [Qubit Indexing Convention](#qubit-indexing-convention)
- [Memory Model](#memory-model)
- [Limitations](#limitations)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)

---

## Requirements

- .NET 10 SDK
- Windows, macOS, or Linux
- Terminal, PowerShell, Command Prompt, or shell
- Optional: Visual Studio, Visual Studio Code, or JetBrains Rider

Check your installed SDK:

```bash
dotnet --version
```

---

## Quick Start

Clone the repository:

```bash
git clone https://github.com/topoi-io/topoi-quantum.git
cd topoi-quantum
```

Build the solution:

```bash
dotnet build Topoi.Quantum.slnx
```

Run all tests:

```bash
dotnet test Topoi.Quantum.slnx
```

Run the command-line tool locally:

```bash
dotnet run --project Topoi.Quantum.Tool -- --help
```

Run an OpenQASM Bell-state experiment:

```bash
dotnet run --project Topoi.Quantum.Tool -- --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
```

Expected Bell-state behaviour:

```text
|00>  P ≈ 0.5
|11>  P ≈ 0.5
<Z0 ⊗ Z1> ≈ +1
```

Pack the CLI as a local .NET tool package:

```bash
dotnet pack Topoi.Quantum.Tool -c Release
```

The tool project is configured with package ID `Topoi.Quantum.Tool` and command name `tq`.

---

## SDK Quick Start

The SDK layer provides a C# API for building circuits, sampling outcomes, and estimating observables.

```csharp
using Topoi.Quantum;
using Topoi.Quantum.Primitives;

QuantumCircuit circuit = QuantumToolkit
    .Circuit(2)
    .H(0)
    .CX(0, 1)
    .Build();

SamplerResult samples = QuantumToolkit.Sampler.Run(circuit, shots: 1000);

Console.WriteLine($"00 count = {samples.CountFor("00")}");
Console.WriteLine($"11 count = {samples.CountFor("11")}");

EstimatorResult estimate = QuantumToolkit.Estimator.Estimate(
    circuit,
    new[]
    {
        new PauliTerm('Z', 0),
        new PauliTerm('Z', 1)
    });

Console.WriteLine($"<ZZ> = {estimate.Value}");
```

Fluent builder example:

```csharp
using Topoi.Quantum;

QuantumCircuit bell = QuantumCircuitBuilder
    .WithQubits(2)
    .H(0)
    .CX(0, 1)
    .Build();
```

Parameterized-gate example:

```csharp
QuantumCircuit circuit = QuantumCircuitBuilder
    .WithQubits(1)
    .H(0)
    .RZ(0, Math.PI / 2)
    .Build();
```

Controlled-rotation example:

```csharp
QuantumCircuit circuit = QuantumCircuitBuilder
    .WithQubits(2)
    .H(0)
    .CRZ(0, 1, Math.PI / 4)
    .Build();
```

Toffoli example:

```csharp
QuantumCircuit circuit = QuantumCircuitBuilder
    .WithQubits(3)
    .H(0)
    .H(1)
    .CCX(0, 1, 2)
    .Build();
```

---

## Core SDK Features

The core SDK currently includes:

- `QuantumCircuit`
- `QuantumCircuitBuilder`
- `QuantumToolkit`
- `QuantumSimulator`
- `QuantumRegister`
- `Quantum`
- `GateOperation`
- `GateKind`
- `PauliTerm`
- `IRandomSource`
- `CryptoRandomSource`
- `SeededRandomSource`
- `Sampler`
- `SamplerResult`
- `Estimator`
- `EstimatorResult`

The simulator supports:

- dense state-vector simulation
- pure-state simulation using `System.Numerics.Complex`
- default dense state-vector safety limit of 25 qubits
- instance-based simulator use through `QuantumSimulator`
- static interactive use through the `Quantum` facade
- full-register measurement with state collapse
- single-qubit measurement with partial collapse
- repeated sampling
- Pauli expectation values
- state-vector probability inspection
- state norm diagnostics
- manual normalization
- snapshot and restore support
- cryptographically strong randomness by default
- seeded randomness for repeatable tests and lab runs

---

## Supported Gates

### Core SDK gate model

The core `GateKind` / `GateOperation` model supports:

Single-qubit gates:

- `I`
- `X`, `Y`, `Z`
- `H`
- `S`, `SDG`
- `T`, `TDG`
- `SX`, `SXDG`
- `RX`, `RY`, `RZ`

Controlled and multi-qubit gates:

- `CX` / `CNOT`
- `CY`
- `CZ`
- `CH`
- `CP`
- `SWAP`
- `CCX` / Toffoli
- `CRX`, `CRY`, `CRZ`

### Interactive and native `.qc` command support

The interactive command executor and native `.qc` gate loader currently support a smaller command-oriented subset:

- `X`, `Y`, `Z`
- `H`, `S`, `T`
- `RX`, `RY`, `RZ`
- `CX` / `CNOT`
- `CZ`
- `SWAP`
- `CCX` / `TOFFOLI`
- `CRX`, `CRY`, `CRZ`

This means some gates supported by the core SDK and OpenQASM layer, such as `I`, `SDG`, `TDG`, `SX`, `SXDG`, `CY`, `CH`, and `CP`, are not currently exposed through the interactive/native `.qc` command parser.

---

## Sampler Primitive

The `Sampler` primitive executes a circuit many times and returns measurement counts and probabilities.

It is useful for:

- Bell-state sampling
- GHZ-state sampling
- comparing theoretical probabilities with measured frequencies
- demonstrating quantum randomness
- deterministic seeded simulations for tests and coursework

Example:

```csharp
using Topoi.Quantum;
using Topoi.Quantum.Primitives;

QuantumCircuit circuit = QuantumToolkit
    .Circuit(1)
    .H(0)
    .Build();

var sampler = new Sampler(new SeededRandomSource(123));

SamplerResult result = sampler.Run(circuit, shots: 1000);

Console.WriteLine(result.CountFor("0"));
Console.WriteLine(result.CountFor("1"));
Console.WriteLine(result.ProbabilityFor("0"));
Console.WriteLine(result.ProbabilityFor("1"));
```

`SamplerResult` contains:

- `QubitCount`
- `Shots`
- `Counts`
- `Probabilities`
- `CountFor(bitString)`
- `ProbabilityFor(bitString)`

---

## Estimator Primitive

The `Estimator` primitive computes expectation values for Pauli observables.

It is useful for:

- quantum algorithm analysis
- Bell-state correlation experiments
- GHZ-state correlation experiments
- validating simulator behaviour against known analytic results
- introducing observables and expectation values

Example:

```csharp
using Topoi.Quantum;
using Topoi.Quantum.Primitives;

QuantumCircuit circuit = QuantumToolkit
    .Circuit(2)
    .H(0)
    .CX(0, 1)
    .Build();

var estimator = new Estimator();

EstimatorResult result = estimator.Estimate(
    circuit,
    new[]
    {
        new PauliTerm('Z', 0),
        new PauliTerm('Z', 1)
    });

Console.WriteLine(result.Value); // approximately +1
```

`EstimatorResult` contains:

- `QubitCount`
- `Observable`
- `Value`

---

## Command-Line Usage

The implemented CLI uses option syntax:

```bash
tq [options]
```

When running from source:

```bash
dotnet run --project Topoi.Quantum.Tool -- [options]
```

Implemented options:

```text
--help, -h                  Show command-line help
--qubits <n>, -q <n>         Number of qubits to initialise; default 1
--qasm <path>                Load and run an OpenQASM 3 circuit file
--openqasm <path>            Alias for --qasm
--run <path>                 Run a full interpreter script and exit
--circuit <path>             Load and run a gate-only native circuit file and exit
--print-circuit              Print loaded circuit before execution
--draw                       Draw loaded circuit before execution
--print                      Print final state amplitudes and probabilities
--probs                      Print final basis-state probabilities
--expect "observable"        Print a Pauli expectation value, e.g. --expect "ZZ 0 1"
--sample <n>                 Sample the final state n times
```

Examples:

```bash
dotnet run --project Topoi.Quantum.Tool -- --qubits 2 --run examples/bell.qc
dotnet run --project Topoi.Quantum.Tool -- --qubits 2 --circuit circuits/bell.qc --print-circuit --print
dotnet run --project Topoi.Quantum.Tool -- --qubits 2 --circuit circuits/bell.qc --expect "ZZ 0 1" --expect "XX 0 1"
dotnet run --project Topoi.Quantum.Tool -- --qubits 3 --circuit circuits/ghz3.qc --probs --sample 1000
dotnet run --project Topoi.Quantum.Tool -- --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
```

Command-oriented syntax such as `tq run examples/bell.qasm`, `tq draw examples/bell.qasm`, or `tq sample examples/bell.qasm --shots 1000` is not currently implemented.

---

## Interactive REPL

Start the interactive lab locally:

```bash
dotnet run --project Topoi.Quantum.Tool
```

Example session:

```text
Topoi Quantum CLI (state-vector simulator)
Number of qubits n (e.g. 1,2,3): 2

H 0
CX 0 1
PRINT
EXPECT ZZ 0 1
EXPECT XX 0 1
SAMPLE 1000
```

Implemented interactive commands include:

```text
HELP                        Show interactive help
PRINT                       Show top amplitudes and probabilities
PROBS                       Show top basis probabilities without measurement
EXPECT <observable>         Compute a Pauli expectation value
RESET                       Reset to |00..0>
MEASURE <q>                 Measure one qubit and partially collapse state
MEASUREALL                  Measure full register and collapse state
SAMPLE <n>                  Repeatedly sample the current state, restoring state each trial
NORM                        Show current state norm
NORMALIZE                   Manually normalize the state vector
MEM                         Show estimated dense state-vector memory usage
QRAND [k]                   Generate k random bits; default k = number of qubits
RUN <path>                  Run commands from a .qc script file
LOAD <path>                 Load gate operations from a .qc file into a QuantumCircuit
CIRCUIT                     Print the currently loaded circuit
DRAW                        Draw the currently loaded circuit
RUNCIRCUIT                  Execute the currently loaded circuit
CLEARCIRCUIT                Clear the currently loaded circuit
QUIT / EXIT                 Exit
```

Supported angle formats include:

```text
1.57079632679   pi   +pi/2   -pi/8   3*pi/4   -3*pi/4   0.5*pi
```

---

## OpenQASM Workflows

Topoi Quantum includes a focused OpenQASM 3.x implementation for practical quantum-lab experimentation. It is a useful subset, not a full OpenQASM 3 compliance implementation.

### Implemented OpenQASM features

The parser/importer currently supports:

- `OPENQASM 3`, `OPENQASM 3.0`, and `OPENQASM 3.1`
- `include "stdgates.inc";`
- `qubit[n] q;`
- `bit[n] c;`
- standard gate calls
- parameterized gate calls
- angle expressions using `pi`, numeric literals, unary `+`/`-`, arithmetic, parentheses, and custom-gate parameters
- measurement assignment syntax, e.g. `c[0] = measure q[0];`
- legacy measurement syntax, e.g. `measure q[0] -> c[0];`
- `reset q[i];`
- `barrier q[0], q[1];`
- custom gate definitions
- parameterized custom gate definitions
- nested custom gate expansion
- recursive custom gate detection
- `ctrl @` for supported gates
- `inv @` for supported gates
- export from supported circuit and executable models

### OpenQASM gate support

Standard OpenQASM gate calls currently mapped to the core gate model:

```text
id
x, y, z, h
s, sdg, t, tdg, sx, sxdg
rx, ry, rz
cx, cy, cz, ch, swap, ccx
crx, cry, crz, cp
```

Implemented `ctrl @` mappings:

```text
ctrl @ x   -> CX
ctrl @ y   -> CY
ctrl @ z   -> CZ
ctrl @ h   -> CH
ctrl @ rx  -> CRX
ctrl @ ry  -> CRY
ctrl @ rz  -> CRZ
```

Implemented `inv @` mappings:

```text
inv @ s     -> SDG
inv @ sdg   -> S
inv @ t     -> TDG
inv @ tdg   -> T
inv @ sx    -> SXDG
inv @ sxdg  -> SX
inv @ x/y/z/h -> same self-inverse gate
inv @ rx/ry/rz(theta) -> rx/ry/rz(-theta)
```

`pow` is tokenized and parsed as a modifier, but it is not currently mapped to executable gate behaviour. `negctrl` is tokenized but is not currently parsed into executable modifier behaviour.

### Circuit import path

`OpenQasmCircuitLoader.LoadFromString()` and `LoadFromFile()` parse and expand supported OpenQASM, then convert gate calls into a `QuantumCircuit`.

This gate-only circuit path is what the current CLI `--qasm` option uses.

### Executable OpenQASM path

`OpenQasmCircuitLoader.LoadExecutableFromString()` and `LoadExecutableFromFile()` return an `OpenQasmExecutableProgram` that can preserve measurement, classical bits, reset, and barrier operations.

`OpenQasmExecutor.Execute()` executes this executable model and returns an `OpenQasmExecutionResult` containing the simulator and classical bits.

At present, this executable OpenQASM path exists in the SDK layer, but the CLI `--qasm` path does not yet use it.

### Basic Bell circuit

```qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;

h q[0];
cx q[0], q[1];
```

Run through the current CLI gate-only QASM path:

```bash
dotnet run --project Topoi.Quantum.Tool -- --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
```

### Measurement example for SDK executable path

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

### Reset example for SDK executable path

```qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[1] q;

x q[0];
reset q[0];
```

### Barrier example

```qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;

h q[0];
barrier q[0], q[1];
cx q[0], q[1];
```

`barrier` is accepted as a circuit marker and treated as a no-op during simulation.

### Custom gate example

```qasm
OPENQASM 3.1;
include "stdgates.inc";

gate bell a, b {
    h a;
    cx a, b;
}

qubit[2] q;

bell q[0], q[1];
```

### Parameterized custom gate example

```qasm
OPENQASM 3.1;
include "stdgates.inc";

gate phase(theta) a {
    rz(theta) a;
}

qubit[1] q;

phase(pi / 2) q[0];
```

---

## Circuit Drawing

`Topoi.Quantum.Drawing` currently provides Unicode/text circuit rendering through `CircuitDrawer.Draw(circuit)`.

It supports drawing the implemented core gate kinds, including single-qubit gates, controlled gates, controlled rotations, swap, and Toffoli.

SVG and PNG rendering are not currently implemented.

---

## Project Structure

Current solution structure:

```text
Topoi.Quantum.slnx

Topoi.Quantum
  Core simulator, register, gates, circuits, builder, toolkit facade, primitives, observables, randomness

Topoi.Quantum.OpenQasm
  OpenQASM lexer, parser, AST, circuit loader, executable loader, exporter, executor, custom-gate expansion

Topoi.Quantum.Parsing
  Native .qc parser and observable parser

Topoi.Quantum.Drawing
  Unicode/text circuit drawing

Topoi.Quantum.Cli
  Command-line option parsing, interactive command execution, console help, and console printing

Topoi.Quantum.Tool
  Packaged executable entry point for the `tq` command

Topoi.Quantum.Tests
  NUnit regression tests

examples
  Example OpenQASM and native circuit files
```

---

## Architecture

The current dependency direction is:

```text
Topoi.Quantum.Tool
    ↓
Topoi.Quantum.Cli
Topoi.Quantum.Drawing
Topoi.Quantum.Parsing
Topoi.Quantum.OpenQasm
    ↓
Topoi.Quantum
```

The core rule is:

```text
Topoi.Quantum should remain independent of CLI, Drawing, Parsing, OpenQASM, and the console/tool project.
```

This keeps the simulator engine and SDK primitives reusable from other .NET applications.

---

## Testing

Run all tests:

```bash
dotnet test Topoi.Quantum.slnx
```

The test project uses:

- NUnit
- NUnit analyzers
- NUnit3 test adapter
- Microsoft.NET.Test.Sdk
- coverlet collector

The test suite includes coverage for areas such as:

- single-qubit gates
- inverse gates
- phase gates
- rotation gates
- controlled gates
- controlled rotations
- Bell and GHZ states
- measurement collapse
- sampling behaviour
- Pauli expectation values
- circuit validation
- fluent circuit builder usage
- SDK primitives
- circuit drawing
- OpenQASM parsing
- OpenQASM export
- OpenQASM round trips where implemented
- custom gate expansion
- parameterized custom gates
- OpenQASM gate modifiers
- reset and barrier model/export behaviour

A professional release should pass the full test suite before being distributed.

---

## Mathematical Model

The simulator represents an N-qubit pure quantum state as a dense vector of complex amplitudes:

```text
|psi> = sum_i alpha_i |i>
```

The vector contains `2^n` amplitudes and is normalized according to:

```text
sum_i |alpha_i|^2 = 1
```

Single-qubit gates apply 2x2 unitary matrices to a target qubit.

Controlled gates apply the target operation only when the control qubits are in state `|1>`.

Measurement follows the Born rule:

```text
P(i) = |alpha_i|^2
```

After measurement, the state collapses to the measured outcome or subspace.

The `Sampler` primitive estimates measurement distributions by repeated circuit execution and measurement.

The `Estimator` primitive computes expectation values of Pauli observables against the final simulated state.

---

## Qubit Indexing Convention

The simulator uses this convention:

```text
Qubit 0 is the least significant bit in the basis-state index.
```

For a 2-qubit register:

```text
X 0
```

prepares:

```text
|01>
```

not:

```text
|10>
```

Bit strings are displayed in conventional most-significant-bit to least-significant-bit order.

---

## Memory Model

Dense state-vector simulation grows exponentially.

Each amplitude is a `System.Numerics.Complex` value containing two `double` values:

```text
Real      = 8 bytes
Imaginary = 8 bytes
Total     = 16 bytes per amplitude
```

Topoi Quantum currently enforces a **default dense state-vector safety limit of 25 qubits**. This is an intentional MVP guardrail that prevents accidental allocation of very large state vectors during interactive use, test runs, and local development.

Approximate state-vector memory use within the supported default range:

```text
20 qubits = 16 MiB
21 qubits = 32 MiB
22 qubits = 64 MiB
23 qubits = 128 MiB
24 qubits = 256 MiB
25 qubits = 512 MiB
```

Attempts to create a dense register above 25 qubits currently throw a clear validation error explaining the memory requirement and the default safety limit.

This makes the project ideal for laboratory-scale experiments, educational circuits, and small-to-medium local simulations. Larger dense simulations, explicit advanced overrides, tensor-network simulation, GPU acceleration, and distributed simulation are outside the current implementation scope.

---

## Limitations

Topoi Quantum is a local quantum toolkit and ideal state-vector simulator, not a quantum hardware runtime.

Current limitations include:

- dense state-vector simulation only
- default dense simulator safety limit of 25 qubits
- pure states only
- no density matrix simulator
- no noise or decoherence model
- no hardware backend
- no cloud quantum provider integration
- no circuit transpiler
- no gate optimization pass
- no tensor-network backend
- no GPU acceleration
- no distributed simulation
- no SVG or PNG circuit rendering
- no GitHub Actions CI workflow currently present
- no BenchmarkDotNet benchmark project currently present
- OpenQASM support is a practical subset, not full compliance
- OpenQASM timing, calibration, aliases, physical qubits, and classical control flow are not fully supported
- `pow` and `negctrl` OpenQASM modifiers are not currently executable
- CLI `--qasm` currently uses the gate-only circuit loader rather than the executable OpenQASM model

These limitations are deliberate for the current professional lab and educational scope, except where called out in the roadmap as future work.

---

## Roadmap

The following items are still future work based on the current codebase:

- publish/release `Topoi.Quantum` to NuGet
- publish/release the already-packaged `Topoi.Quantum.Tool` global tool package
- add GitHub Actions CI and status badge
- add package validation to the release pipeline
- add BenchmarkDotNet benchmark project
- add command-oriented CLI syntax such as `tq run`, `tq draw`, `tq sample`, and `tq expect`
- wire CLI `--qasm` to the executable OpenQASM model where measurement/reset/barrier/classical bits are required
- expose the full core gate set through the REPL/native `.qc` command parser
- add SVG and/or PNG circuit rendering
- expand OpenQASM support, including executable `pow` and `negctrl` modifiers if desired
- add additional algorithm examples, such as teleportation, Deutsch-Jozsa, Grover, QFT, and small VQE-style expectation examples
- add teaching worksheets and lab notebooks
- add explicit advanced override for dense simulations above 25 qubits
- add noise channels
- add density matrix simulation
- add Bloch sphere visualization
- add simple transpilation and optimization passes
- add optional cloud/hardware backend abstraction

Items already implemented and therefore no longer listed as future-only roadmap work:

- `Topoi.Quantum` SDK project and package metadata
- `Topoi.Quantum.Tool` executable/global-tool project
- `tq` tool command configuration
- Unicode/text circuit drawing
- OpenQASM custom-gate expansion
- OpenQASM `ctrl @` and `inv @` support for the mapped gates listed above
- SDK-level `Sampler` and `Estimator` primitives
- seeded randomness for repeatable tests and lab runs
- NUnit regression test project

---

## Contributing

Contributions should preserve the project goals of clarity, correctness, testability, and professional Topoi Quantum usability.

Good contribution areas include:

- additional simulator tests
- new example circuits
- improved OpenQASM examples
- clearer lab exercises
- documentation improvements
- additional SDK examples
- well-tested gate additions
- benchmark coverage
- bug fixes with regression tests

Before submitting changes, run:

```bash
dotnet build Topoi.Quantum.slnx
dotnet test Topoi.Quantum.slnx
```

---

## License

MIT

---

## Academic and Quantum Lab Use

Topoi Quantum may be used for professional quantum-computing demonstrations, teaching, coursework, workshops, and local quantum-circuit experimentation.
