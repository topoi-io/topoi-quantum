# Quantum Computer Lab for .NET

**A professional C# / .NET 10 quantum computer laboratory toolkit for state-vector simulation, circuit experimentation, OpenQASM workflows, sampling, expectation estimation, and quantum-computing education.**

This repository provides a complete local **Quantum Computer Lab** environment for learning, experimenting, teaching, and prototyping quantum circuits in C#.

It combines:

- a dense state-vector quantum simulator
- a fluent circuit builder
- SDK-level `Sampler` and `Estimator` primitives
- OpenQASM 3.x import/export support
- custom OpenQASM gate expansion
- circuit drawing
- command-line execution
- interactive REPL usage
- repeatable testing with seeded randomness
- NUnit regression coverage

The project is designed for users who want to understand how quantum computation works at the circuit, state-vector, measurement, and observable level while staying inside a professional .NET development workflow.

---

## Intended Use

This project is suitable for:

- Quantum Computer Lab exercises
- university quantum-computing modules
- C# and .NET quantum software teaching
- OpenQASM experimentation
- local simulation of small-to-medium quantum circuits
- algorithm prototyping before using cloud quantum services
- teaching Bell states, GHZ states, measurement, sampling, and Pauli observables
- demonstrating how simulator internals work
- test-driven quantum software development

It is intentionally focused on **clarity, correctness, educational value, professional API design, and inspectable simulator architecture**.

It is not intended to replace high-performance production simulators, tensor-network simulators, or real quantum hardware backends.

---

## Contents

- [Highlights](#highlights)
- [Learning Outcomes](#learning-outcomes)
- [Requirements](#requirements)
- [Quick Start](#quick-start)
- [SDK Quick Start](#sdk-quick-start)
- [Sampler Primitive](#sampler-primitive)
- [Estimator Primitive](#estimator-primitive)
- [Fluent Circuit Builder](#fluent-circuit-builder)
- [Command-Line Quantum Lab](#command-line-quantum-lab)
- [Interactive REPL](#interactive-repl)
- [OpenQASM Laboratory Workflows](#openqasm-laboratory-workflows)
- [Custom OpenQASM Gates](#custom-openqasm-gates)
- [OpenQASM Gate Modifiers](#openqasm-gate-modifiers)
- [Recommended Quantum Computer Lab Exercises](#recommended-quantum-computer-lab-exercises)
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

### Professional .NET quantum lab API

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
- Pure-state simulation using complex amplitudes
- Instance-based simulator model
- Static `Quantum` facade for interactive experiments
- Full-register measurement with state collapse
- Single-qubit measurement with partial collapse
- Repeated sampling
- Pauli expectation values
- State-vector probability inspection
- State norm diagnostics
- Manual normalization
- Snapshot and restore support
- Cryptographically strong randomness by default
- Seeded randomness for repeatable lab runs and tests

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
- OpenQASM angle expressions using `pi`, arithmetic, parentheses, and custom-gate parameters
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

## Learning Outcomes

A user working through this Quantum Computer Lab should be able to:

1. Represent an N-qubit quantum state as `2^n` complex amplitudes.
2. Apply single-qubit gates and controlled gates to a dense state vector.
3. Construct Bell states and GHZ states.
4. Explain measurement collapse and repeated sampling.
5. Use a `Sampler` primitive to estimate measurement distributions.
6. Use an `Estimator` primitive to compute Pauli expectation values.
7. Understand the relationship between amplitudes, probabilities, and observables.
8. Write and execute OpenQASM circuits.
9. Define custom OpenQASM gates, including parameterized custom gates.
10. Use `ctrl @` and `inv @` OpenQASM-style gate modifier workflows where supported.
11. Understand the exponential memory cost of dense state-vector simulation.
12. Extend a clean, test-driven C# quantum software architecture.

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
git clone https://github.com/topoi-io/Quantum-Computer-Simulator.git
cd Quantum-Computer-Simulator
```

Build the solution:

```bash
dotnet build
```

Run all tests:

```bash
dotnet test
```

Run the command-line Quantum Computer Lab:

```bash
dotnet run --project QuantumComputer
```

Run an OpenQASM Bell-state experiment:

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

```csharp
using QuantumComputer.Core;
using QuantumComputer.Core.Primitives;

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

This is the recommended entry point for professional Quantum Computer Lab exercises.

---

## Sampler Primitive

The `Sampler` primitive executes a circuit many times and returns measurement counts and probabilities.

It is useful for laboratory work involving:

- Bell-state sampling
- GHZ-state sampling
- comparing theoretical probabilities with measured frequencies
- demonstrating quantum randomness
- deterministic seeded simulations for tests and coursework

Example:

```csharp
using QuantumComputer.Core;
using QuantumComputer.Core.Primitives;

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
using QuantumComputer.Core;
using QuantumComputer.Core.Primitives;

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
using QuantumComputer.Core;

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

## Command-Line Quantum Lab

The console project can run quantum experiments from the command line.

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

Examples:

```bash
dotnet run --project QuantumComputer -- --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
```

```bash
dotnet run --project QuantumComputer -- --qasm examples/ghz3.qasm --draw --print --expect "ZZ 0 1" --expect "ZZ 1 2"
```

```bash
dotnet run --project QuantumComputer -- --qasm examples/ctrl-modifier.qasm --draw --print --expect "ZZ 0 1"
```

```bash
dotnet run --project QuantumComputer -- --qasm examples/parameterized-phase.qasm --draw --print --expect "Z 0"
```

```bash
dotnet run --project QuantumComputer -- --qasm examples/bell.qasm --sample 1000
```

---

## Interactive REPL

Start the interactive lab:

```bash
dotnet run --project QuantumComputer
```

Example session:

```text
N-Qubit Gate Interpreter (state-vector)
Number of qubits n (e.g. 1,2,3): 2

H 0
CX 0 1
PRINT
EXPECT ZZ 0 1
EXPECT XX 0 1
SAMPLE 1000
```

Useful commands:

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

## OpenQASM Laboratory Workflows

The project includes a focused OpenQASM 3.x importer, execution model, custom-gate expander, and exporter.

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
dotnet run --project QuantumComputer -- --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
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

---

## Custom OpenQASM Gates

### Non-parameterized custom gate

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

### Parameter expressions

```qasm
OPENQASM 3.1;
include "stdgates.inc";

gate half_phase(theta) a {
    rz(theta / 2) a;
}

qubit[1] q;

half_phase(pi) q[0];
```

### Nested parameterized gates

```qasm
OPENQASM 3.1;
include "stdgates.inc";

gate phase(theta) a {
    rz(theta) a;
}

gate double_phase(theta) a {
    phase(theta) a;
    phase(theta) a;
}

qubit[1] q;

double_phase(pi / 4) q[0];
```

---

## OpenQASM Gate Modifiers

Supported OpenQASM modifier syntax can be used for laboratory experiments.

### Controlled modifier

```qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;

h q[0];
ctrl @ x q[0], q[1];
```

This maps to a controlled-X operation and creates a Bell state.

### Inverse modifier

```qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[1] q;

s q[0];
inv @ s q[0];
```

Supported inverse mappings include common self-inverse gates and inverse phase/rotation operations where implemented.

---

## Recommended Quantum Computer Lab Exercises

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

SDK version:

```csharp
QuantumCircuit circuit = QuantumToolkit
    .Circuit(1)
    .H(0)
    .Build();

SamplerResult result = QuantumToolkit.Sampler.Run(circuit, 1000);
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

SDK version:

```csharp
QuantumCircuit circuit = QuantumToolkit
    .Circuit(2)
    .H(0)
    .CX(0, 1)
    .Build();

EstimatorResult zz = QuantumToolkit.Estimator.Estimate(
    circuit,
    new[]
    {
        new PauliTerm('Z', 0),
        new PauliTerm('Z', 1)
    });
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
dotnet run --project QuantumComputer -- --qasm examples/bell.qasm --draw --print
```

### Lab 6 — Parameterized gates

Objectives:

- define a parameterized custom gate
- substitute symbolic angle parameters
- verify phase behaviour with expectations

Example:

```qasm
gate phase(theta) a {
    rz(theta) a;
}
```

### Lab 7 — Circuit design mini-project

Suggested tasks:

- implement a new example algorithm fragment
- add a new gate with tests
- create a new OpenQASM example
- add a new observable experiment
- benchmark qubit count versus memory use

---

## Project Structure

```text
QuantumComputer.slnx

QuantumComputer.Core
  Core simulator, register, gates, circuits, builder, toolkit facade, primitives, observables, randomness

QuantumComputer.Core/Primitives
  Sampler, SamplerResult, Estimator, EstimatorResult

QuantumComputer.Cli
  Command-line and REPL behaviour

QuantumComputer.Parsing
  Native .qc parser and observable parser

QuantumComputer.Drawing
  Unicode circuit drawing

QuantumComputer.OpenQasm
  OpenQASM lexer, parser, AST, importer, exporter, executor, custom-gate expansion

QuantumComputer.Tests
  NUnit regression tests

examples
  Example OpenQASM and native circuit files
```

---

## Architecture

The intended dependency direction is:

```text
QuantumComputer
    ↓
QuantumComputer.Cli
QuantumComputer.Drawing
QuantumComputer.Parsing
QuantumComputer.OpenQasm
    ↓
QuantumComputer.Core
```

The core rule is:

```text
QuantumComputer.Core must not depend on CLI, Drawing, Parsing, OpenQASM, or the console app.
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

A professional lab release should pass the full test suite before being distributed.

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

This project is a local quantum computer laboratory simulator, not a quantum hardware runtime.

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

- NuGet package publication
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

Contributions should preserve the project goals of clarity, correctness, testability, and professional Quantum Computer Lab usability.

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

This project may be used for professional Quantum Computer Lab demonstrations, teaching, coursework, workshops, and local quantum-circuit experimentation.

Instructors and lab leads are encouraged to adapt the examples, SDK snippets, and lab structure to their own module or workshop learning outcomes.
