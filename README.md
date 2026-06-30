# Topoi Quantum

**A professional .NET quantum toolkit for circuit building, dense state-vector simulation, OpenQASM workflows, sampling, estimation, and quantum-computing education.**

Topoi Quantum provides a local quantum-computing laboratory and SDK for learning, prototyping, testing, and demonstrating quantum circuits in C# and .NET.

It combines:

- a dense state-vector quantum simulator
- a fluent circuit builder
- SDK-level `Sampler` and `Estimator` primitives
- OpenQASM 3.x import/export workflows
- custom OpenQASM gate expansion
- circuit drawing
- command-line execution
- interactive REPL usage
- repeatable testing with seeded randomness
- NUnit regression coverage

The project is designed for users who want to understand and prototype quantum computation at the circuit, state-vector, measurement, and observable level while staying inside a professional .NET development workflow.

Topoi Quantum is intentionally focused on **clarity, correctness, educational value, professional API design, and inspectable simulator architecture**. It is not intended to replace high-performance production simulators, tensor-network simulators, or real quantum hardware backends.

---

## Naming

| Area | Name |
|---|---|
| Product | **Topoi Quantum** |
| Repository | `topoi-quantum` |
| Main SDK package | `Topoi.Quantum` |
| CLI tool package | `Topoi.Quantum.Tool` |
| CLI command | `tq` |

The short CLI command is designed to be easy to type:

```bash
tq --help
tq --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
tq --qasm examples/bell.qasm --sample 1000
```

A future command-oriented syntax may also be added:

```bash
tq run examples/bell.qasm
tq draw examples/bell.qasm
tq sample examples/bell.qasm --shots 1000
tq expect examples/bell.qasm "ZZ 0 1"
```

---

## Intended Use

Topoi Quantum is suitable for:

- quantum-computing lab exercises
- university quantum-computing modules
- C# and .NET quantum software teaching
- OpenQASM experimentation
- local simulation of small-to-medium quantum circuits
- algorithm prototyping before using cloud quantum services
- teaching Bell states, GHZ states, measurement, sampling, and Pauli observables
- demonstrating how simulator internals work
- test-driven quantum software development

---

## Contents

- [Highlights](#highlights)
- [Requirements](#requirements)
- [Quick Start](#quick-start)
- [SDK Quick Start](#sdk-quick-start)
- [Sampler Primitive](#sampler-primitive)
- [Estimator Primitive](#estimator-primitive)
- [Fluent Circuit Builder](#fluent-circuit-builder)
- [Command-Line Usage](#command-line-usage)
- [Interactive REPL](#interactive-repl)
- [OpenQASM Workflows](#openqasm-workflows)
- [Recommended Lab Exercises](#recommended-lab-exercises)
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

## Highlights

### Professional .NET quantum API

- `QuantumCircuit`
- `QuantumCircuitBuilder`
- `QuantumToolkit`
- `Sampler`
- `SamplerResult`
- `Estimator`
- `EstimatorResult`
- `QuantumSimulator`
- `PauliTerm`
- `GateOperation`

### Quantum simulation

- N-qubit dense state-vector simulation
- pure-state simulation using complex amplitudes
- instance-based simulator model
- static `Quantum` facade for interactive experiments
- full-register measurement with state collapse
- single-qubit measurement with partial collapse
- repeated sampling
- Pauli expectation values
- state-vector probability inspection
- state norm diagnostics
- manual normalization
- snapshot and restore support
- cryptographically strong randomness by default
- seeded randomness for repeatable lab runs and tests

### Supported gates

Single-qubit gates:

- `I` / `ID`
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

### OpenQASM support

- OpenQASM 3.x version declarations
- `include "stdgates.inc";`
- `qubit[n] q;`
- `bit[n] c;`
- standard gate calls
- parameterized gate calls
- angle expressions using `pi`, arithmetic, parentheses, and custom-gate parameters
- measurement into classical bits
- legacy measurement syntax
- reset
- barrier as a no-op marker
- custom gate definitions
- parameterized custom gate definitions
- nested custom gate expansion
- recursive custom gate detection
- gate modifiers such as `ctrl @` and `inv @` where supported
- OpenQASM export from supported circuit/executable models

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
dotnet build
```

Run all tests:

```bash
dotnet test
```

Run the command-line tool locally after the project rename is complete:

```bash
dotnet run --project Topoi.Quantum.Tool -- --help
```

Run an OpenQASM Bell-state experiment:

```bash
dotnet run --project Topoi.Quantum.Tool -- --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
```

During the rename branch migration, if the executable project has not yet been renamed, use the current project name temporarily:

```bash
dotnet run --project QuantumComputer -- --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
```

Expected Bell-state behaviour:

```text
|00>  P ≈ 0.5
|11>  P ≈ 0.5
<Z0 ⊗ Z1> ≈ +1
```

---

## SDK Quick Start

The SDK layer provides a professional way to build circuits, sample outcomes, and estimate observables from C#.

Target namespace after the project rename:

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

During the rename branch migration, existing namespaces may temporarily remain:

```csharp
using QuantumComputer.Core;
using QuantumComputer.Core.Primitives;
```

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

## Fluent Circuit Builder

`QuantumCircuitBuilder` provides a clean fluent API for constructing circuits.

```csharp
using Topoi.Quantum;

QuantumCircuit bell = QuantumCircuitBuilder
    .WithQubits(2)
    .H(0)
    .CX(0, 1)
    .Build();
```

Parameterized gates:

```csharp
QuantumCircuit circuit = QuantumCircuitBuilder
    .WithQubits(1)
    .H(0)
    .RZ(0, Math.PI / 2)
    .Build();
```

Controlled rotations:

```csharp
QuantumCircuit circuit = QuantumCircuitBuilder
    .WithQubits(2)
    .H(0)
    .CRZ(0, 1, Math.PI / 4)
    .Build();
```

Toffoli:

```csharp
QuantumCircuit circuit = QuantumCircuitBuilder
    .WithQubits(3)
    .H(0)
    .H(1)
    .CCX(0, 1, 2)
    .Build();
```

---

## Command-Line Usage

The `tq` command is the intended Topoi Quantum CLI.

Target global-tool usage:

```bash
tq --help
tq --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
tq --qasm examples/ghz3.qasm --draw --print --expect "ZZ 0 1" --expect "ZZ 1 2"
tq --qasm examples/ctrl-modifier.qasm --draw --print --expect "ZZ 0 1"
tq --qasm examples/parameterized-phase.qasm --draw --print --expect "Z 0"
tq --qasm examples/bell.qasm --sample 1000
```

Current option set:

```text
--help, -h                  Show command-line help
--qubits <n>, -q <n>         Number of qubits to initialise
--run <path>                 Run a full interpreter script and exit
--circuit <path>             Load and run a gate-only native circuit file
--qasm <path>                Load and run an OpenQASM 3.x circuit file
--openqasm <path>            Alias for --qasm
--print-circuit              Print loaded circuit operation list before execution
--draw                       Draw loaded circuit before execution
--print                      Print final state amplitudes and probabilities
--probs                      Print final basis-state probabilities
--expect "observable"        Print a Pauli expectation value
--sample <n>                 Sample the final state n times
```

---

## Interactive REPL

Start the interactive lab locally:

```bash
dotnet run --project Topoi.Quantum.Tool
```

During the rename branch migration, if the executable project has not yet been renamed:

```bash
dotnet run --project QuantumComputer
```

Example session:

```text
Topoi Quantum CLI
Number of qubits n (e.g. 1,2,3): 2

H 0
CX 0 1
PRINT
EXPECT ZZ 0 1
EXPECT XX 0 1
SAMPLE 1000
```

Useful REPL commands:

```text
RESET                       Reset the register to |00..0>
PRINT                       Print top basis amplitudes and probabilities
PROBS                       Show basis-state probabilities
NORM                        Show the current state norm
NORMALIZE                   Manually normalize the state vector
MEM                         Show estimated dense state-vector memory usage
MEASURE q                   Measure one qubit
MEASUREALL                  Measure the full register
SAMPLE n                    Repeatedly sample the current state
EXPECT ...                  Compute a Pauli expectation value
LOAD <path>                 Load a native gate-only circuit
DRAW                        Draw the currently loaded circuit
RUNCIRCUIT                  Execute the loaded circuit
RUN <path>                  Run a command script
```

---

## OpenQASM Workflows

Topoi Quantum includes a focused OpenQASM 3.x importer, execution model, custom-gate expander, and exporter.

The OpenQASM implementation is intended for practical quantum-lab experimentation rather than full language compliance.

### Basic Bell circuit

```qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;

h q[0];
cx q[0], q[1];
```

Run:

```bash
tq --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
```

### Measurement

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

Legacy measurement syntax is also supported:

```qasm
measure q[0] -> c[0];
```

### Reset

```qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[1] q;

x q[0];
reset q[0];
```

### Barrier

```qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;

h q[0];
barrier q[0], q[1];
cx q[0], q[1];
```

`barrier` is accepted as a circuit marker and treated as a no-op during simulation.

### Custom gate

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

### Parameterized custom gate

```qasm
OPENQASM 3.1;
include "stdgates.inc";

gate phase(theta) a {
    rz(theta) a;
}

qubit[1] q;

phase(pi / 2) q[0];
```

### Gate modifiers

```qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;

h q[0];
ctrl @ x q[0], q[1];
```

```qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[1] q;

s q[0];
inv @ s q[0];
```

---

## Recommended Lab Exercises

### Lab 1 — State vectors and single-qubit gates

Objectives:

- inspect amplitudes
- compare amplitudes and probabilities
- apply `X`, `H`, `Z`, and rotation gates

Example:

```text
H 0
PRINT
EXPECT X 0
EXPECT Z 0
```

### Lab 2 — Sampling and measurement

Objectives:

- prepare `H |0>`
- sample repeatedly
- compare counts with theoretical probabilities
- discuss seeded versus cryptographic randomness

Example:

```text
RESET
H 0
SAMPLE 1000
```

### Lab 3 — Bell states and correlations

Objectives:

- create a Bell state
- compare `ZZ` and `XX` expectation values
- explain correlated measurement outcomes

Example:

```text
RESET
H 0
CX 0 1
PRINT
EXPECT ZZ 0 1
EXPECT XX 0 1
SAMPLE 1000
```

### Lab 4 — GHZ states

Objectives:

- prepare a 3-qubit GHZ state
- inspect the final probability distribution
- compute pairwise correlations

Example:

```text
RESET
H 0
CX 0 1
CX 1 2
PRINT
EXPECT ZZ 0 1
EXPECT ZZ 1 2
```

### Lab 5 — OpenQASM execution

Objectives:

- write a circuit in OpenQASM
- run it through the CLI
- compare native commands, SDK circuits, and OpenQASM syntax

Example:

```bash
tq --qasm examples/bell.qasm --draw --print
```

---

## Project Structure

Target structure after the rename is complete:

```text
Topoi.Quantum.slnx

Topoi.Quantum
  Core simulator, register, gates, circuits, builder, toolkit facade, primitives, observables, randomness

Topoi.Quantum.OpenQasm
  OpenQASM lexer, parser, AST, importer, exporter, executor, custom-gate expansion

Topoi.Quantum.Parsing
  Native .qc parser and observable parser

Topoi.Quantum.Drawing
  Unicode circuit drawing

Topoi.Quantum.Cli
  Command-line and REPL behaviour

Topoi.Quantum.Tool
  Packaged executable entry point for the `tq` command

Topoi.Quantum.Tests
  NUnit regression tests

examples
  Example OpenQASM and native circuit files
```

---

## Architecture

The intended dependency direction is:

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
Topoi.Quantum must not depend on CLI, Drawing, Parsing, OpenQASM, or the console/tool project.
```

This keeps the simulator engine and professional SDK primitives reusable from other .NET applications.

---

## Testing

Run all tests:

```bash
dotnet test
```

The test suite covers:

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
- OpenQASM execution
- OpenQASM export
- custom gate expansion
- parameterized custom gates
- OpenQASM gate modifiers
- reset and barrier behaviour
- import/export round trips where implemented

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

Approximate state-vector memory use:

```text
20 qubits = 16 MiB
21 qubits = 32 MiB
22 qubits = 64 MiB
23 qubits = 128 MiB
24 qubits = 256 MiB
25 qubits = 512 MiB
26 qubits = 1 GiB
27 qubits = 2 GiB
28 qubits = 4 GiB
```

This makes the project ideal for laboratory-scale experiments and small-to-medium educational circuits.

---

## Limitations

Topoi Quantum is a local quantum toolkit and ideal state-vector simulator, not a quantum hardware runtime.

Current limitations include:

- dense state-vector simulation only
- pure states only
- no density matrix simulator
- no noise or decoherence model
- no hardware backend
- no cloud quantum provider integration
- no circuit transpiler
- no gate optimization pass
- no tensor-network backend
- no GPU acceleration
- OpenQASM support is a practical subset, not full compliance
- OpenQASM timing, calibration, aliases, physical qubits, and classical control flow are not fully supported
- circuit drawing is currently Unicode text, not SVG/PNG

These limitations are deliberate for the current professional lab and educational scope.

---

## Roadmap

Possible future improvements:

- NuGet package publication as `Topoi.Quantum`
- .NET global tool publication as `Topoi.Quantum.Tool` with command `tq`
- GitHub Actions CI badge
- BenchmarkDotNet benchmark project
- SVG circuit rendering
- richer OpenQASM support
- OpenQASM gate modifiers such as `pow` and `negctrl`
- additional algorithm examples
- teaching worksheets and lab notebooks
- noise channels
- density matrix simulation
- Bloch sphere visualization
- simple transpilation and optimization passes
- optional cloud/hardware backend abstraction

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
dotnet build
dotnet test
```

---

## License

MIT

---

## Academic and Quantum Lab Use

Topoi Quantum may be used for professional quantum-computing demonstrations, teaching, coursework, workshops, and local quantum-circuit experimentation.

Instructors and lab leads are encouraged to adapt the examples, SDK snippets, and lab structure to their own module or workshop learning outcomes.
