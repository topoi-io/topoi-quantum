# Quantum Computer Simulator for .NET

**A C# / .NET 10 dense state-vector quantum circuit simulator for teaching, computer labs, OpenQASM experiments, and quantum-computing coursework.**

This repository provides a clear, test-driven quantum simulator written in C#. It is designed to help students and developers learn how quantum states, gates, circuits, measurement, entanglement, sampling, Pauli expectation values, and OpenQASM programs work internally.

The project is suitable for:

- university computer-lab exercises
- quantum-computing teaching demonstrations
- C#/.NET educational projects
- OpenQASM 3.x parsing and execution experiments
- learning dense state-vector simulation
- test-driven numerical software examples

It is intentionally focused on **clarity, correctness, inspectability, and educational value** rather than competing with high-performance production simulators.

---

## Contents

- [Key Features](#key-features)
- [Learning Outcomes](#learning-outcomes)
- [Requirements](#requirements)
- [Quick Start](#quick-start)
- [Command-Line Usage](#command-line-usage)
- [Interactive REPL](#interactive-repl)
- [OpenQASM Support](#openqasm-support)
- [Using the Simulator as a Library](#using-the-simulator-as-a-library)
- [Recommended Computer Lab Structure](#recommended-computer-lab-structure)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Testing](#testing)
- [Mathematical Model](#mathematical-model)
- [Qubit Indexing Convention](#qubit-indexing-convention)
- [Memory Model](#memory-model)
- [Limitations](#limitations)
- [Roadmap](#roadmap)
- [License](#license)

---

## Key Features

### Quantum simulation

- Dense state-vector simulation for N-qubit pure states
- Instance-based `QuantumSimulator`
- Static `Quantum` facade for simple interactive usage
- Full-register measurement with state collapse
- Single-qubit measurement with partial collapse
- Repeated sampling
- Pauli expectation values
- State-vector probability inspection
- State norm diagnostics
- Manual normalization
- Snapshot and restore support
- Seeded randomness for deterministic tests
- Cryptographically strong randomness by default

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

### Circuit and tooling support

- `QuantumCircuit` model
- Gate-only native `.qc` circuit loading
- Unicode circuit drawing
- CLI execution
- Interactive REPL
- Script execution
- NUnit regression tests

### OpenQASM support

- OpenQASM 3, 3.0, and 3.1 version declarations
- `include "stdgates.inc";`
- `qubit[n] q;`
- `bit[n] c;`
- standard gate calls
- parameterized standard gates
- measurement into classical bits
- legacy measurement syntax
- reset
- barrier as a no-op marker
- OpenQASM export from circuits / executable models where supported
- non-parameterized custom gate definitions
- parameterized custom gate definitions
- custom gate parameter substitution
- nested custom gate expansion
- recursive gate detection
- OpenQASM angle expressions using `pi`, arithmetic, parentheses, and gate parameters
- gate modifiers such as `ctrl @` and `inv @` where implemented

---

## Learning Outcomes

A student using this project should be able to:

1. Explain how an N-qubit pure quantum state is represented as `2^n` complex amplitudes.
2. Apply single-qubit and controlled gates to a state vector.
3. Prepare Bell and GHZ states.
4. Understand measurement collapse and repeated sampling.
5. Compute Pauli expectation values such as `Z0`, `ZZ 0 1`, and `XX 0 1`.
6. Understand the memory limits of dense state-vector simulation.
7. Load and execute OpenQASM circuits.
8. Write custom OpenQASM gates, including parameterized custom gates.
9. Compare native circuit definitions with OpenQASM circuits.
10. Read and extend a clean, test-driven C# simulator architecture.

---

## Requirements

- .NET 10 SDK
- Windows, macOS, or Linux
- A terminal or command prompt
- Optional: Visual Studio 2026, Visual Studio Code, or JetBrains Rider

Check your installed .NET version:

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

Run the test suite:

```bash
dotnet test
```

Run the interactive simulator:

```bash
dotnet run --project QuantumComputer
```

Run an OpenQASM Bell-state example:

```bash
dotnet run --project QuantumComputer -- --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
```

Expected output includes a Bell circuit and probabilities close to:

```text
|00>  P = 0.500000
|11>  P = 0.500000
```

---

## Command-Line Usage

The simulator can run directly from the command line without entering the REPL.

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

### Examples

Run an OpenQASM Bell circuit:

```bash
dotnet run --project QuantumComputer -- --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
```

Run a GHZ circuit:

```bash
dotnet run --project QuantumComputer -- --qasm examples/ghz3.qasm --draw --print --expect "ZZ 0 1" --expect "ZZ 1 2"
```

Run a controlled-modifier OpenQASM circuit:

```bash
dotnet run --project QuantumComputer -- --qasm examples/ctrl-modifier.qasm --draw --print --expect "ZZ 0 1"
```

Run a parameterized custom-gate example:

```bash
dotnet run --project QuantumComputer -- --qasm examples/parameterized-phase.qasm --draw --print --expect "Z 0"
```

Sample a circuit:

```bash
dotnet run --project QuantumComputer -- --qasm examples/bell.qasm --sample 1000
```

---

## Interactive REPL

Start the simulator:

```bash
dotnet run --project QuantumComputer
```

Example REPL session:

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

## OpenQASM Support

The project includes a focused OpenQASM 3.x importer, execution model, custom-gate expander, and exporter.

The aim is not full OpenQASM compliance. The aim is to support a practical subset that is useful for teaching, laboratories, and local simulation.

### Basic OpenQASM example

```qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;

h q[0];
cx q[0], q[1];
```

Run it:

```bash
dotnet run --project QuantumComputer -- --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
```

### Measurement example

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

### Reset example

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

The custom gate expands internally to:

```qasm
h q[0];
cx q[0], q[1];
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

The OpenQASM modifier syntax can be used for supported gates.

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

## Using the Simulator as a Library

The simulator can be used directly from C# without the CLI.

### Build and run a Bell circuit

```csharp
using QuantumComputer.Core;

var circuit = new QuantumCircuit(2);

circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));
circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 1 }));

var simulator = new QuantumSimulator(2);

circuit.Run(simulator);

double[] probabilities = simulator.Register.Probabilities();

Console.WriteLine(probabilities[0]); // |00>
Console.WriteLine(probabilities[3]); // |11>
```

### Compute an expectation value

```csharp
using QuantumComputer.Core;

var circuit = new QuantumCircuit(2);

circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));
circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 1 }));

var simulator = new QuantumSimulator(2);
circuit.Run(simulator);

var observable = new[]
{
    new PauliTerm('Z', 0),
    new PauliTerm('Z', 1)
};

double zz = simulator.ExpectPauliString(observable).Real;

Console.WriteLine(zz); // approximately +1
```

### Load OpenQASM from C#

```csharp
using QuantumComputer.Core;
using QuantumComputer.OpenQasm;

QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromFile("examples/bell.qasm");

var simulator = new QuantumSimulator(circuit.QubitCount);
circuit.Run(simulator);
```

### Export a circuit to OpenQASM

```csharp
using QuantumComputer.Core;
using QuantumComputer.OpenQasm;

var circuit = new QuantumCircuit(2);

circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));
circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 1 }));

string qasm = OpenQasmExporter.Export(circuit);

Console.WriteLine(qasm);
```

---

## Recommended Computer Lab Structure

This repository is suitable for a multi-session lab sequence.

### Lab 1 — State vectors and single-qubit gates

Suggested tasks:

- create a 1-qubit simulator
- apply `X`, `H`, `Z`, and `RZ`
- inspect amplitudes and probabilities
- compare probability with amplitude magnitude squared

Example commands:

```text
H 0
PRINT
EXPECT X 0
EXPECT Z 0
```

### Lab 2 — Measurement and sampling

Suggested tasks:

- prepare `H |0>`
- run repeated sampling
- compare measured frequencies with expected probabilities
- discuss randomness and seeded randomness

Example:

```text
RESET
H 0
SAMPLE 1000
```

### Lab 3 — Entanglement

Suggested tasks:

- prepare a Bell state
- compute `ZZ` and `XX` expectations
- explain why individual qubit measurements are random but correlations are structured

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

### Lab 4 — GHZ states and multi-qubit circuits

Suggested tasks:

- prepare a 3-qubit GHZ state
- inspect probabilities
- compute multi-qubit correlations

Example:

```text
RESET
H 0
CX 0 1
CX 1 2
PRINT
EXPECT ZZ 0 1
EXPECT ZZ 1 2
EXPECT XXX 0 1 2
```

### Lab 5 — OpenQASM circuits

Suggested tasks:

- write a Bell circuit in OpenQASM
- run it through the CLI
- draw the circuit
- compare native commands and OpenQASM syntax

Example:

```bash
dotnet run --project QuantumComputer -- --qasm examples/bell.qasm --draw --print
```

### Lab 6 — Custom gates and modifiers

Suggested tasks:

- define a custom Bell gate
- define a parameterized phase gate
- use `ctrl @` and `inv @` modifiers
- observe custom gate expansion results

### Lab 7 — Mini-project

Suggested project ideas:

- implement a new gate
- add a new OpenQASM construct
- create a new example circuit
- add tests for a known quantum algorithm fragment
- benchmark memory usage as qubit count increases

---

## Project Structure

```text
QuantumComputer.slnx

QuantumComputer.Core
  Core simulator, state-vector register, gates, circuits, observables, randomness

QuantumComputer.Cli
  Command-line and REPL behaviour

QuantumComputer.Parsing
  Native .qc parser and observable parser

QuantumComputer.Drawing
  Unicode circuit drawing

QuantumComputer.OpenQasm
  OpenQASM lexer, parser, AST, importer, exporter, executor, custom gate expansion

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

This keeps the simulator reusable in other .NET applications.

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
- circuit drawing
- OpenQASM parsing
- OpenQASM execution
- OpenQASM export
- custom gate expansion
- parameterized custom gates
- gate modifiers
- reset and barrier behaviour
- import/export round trips where implemented

For lab use, instructors should run the full test suite before distributing a release to students.

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

Bit strings are displayed in the conventional most-significant-bit to least-significant-bit order.

---

## Memory Model

Dense state-vector simulation grows exponentially.

Each amplitude is a `System.Numerics.Complex` value containing two `double` values:

```text
Real      = 8 bytes
Imaginary = 8 bytes
Total     = 16 bytes per amplitude
```

Approximate memory use:

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

This is why the simulator is intended primarily for small-to-medium educational circuits.

---

## Limitations

This project is a teaching and experimentation simulator. It is not a hardware runtime.

Current limitations include:

- dense state-vector simulation only
- pure states only
- no density matrix simulator
- no noise or decoherence model
- no hardware backend
- no circuit transpiler
- no gate optimization pass
- no tensor-network backend
- no GPU acceleration
- OpenQASM support is a practical subset, not full compliance
- OpenQASM timing, calibration, aliases, physical qubits, and classical control flow are not fully supported
- circuit drawing is currently Unicode text, not SVG/PNG

These limitations are intentional for the current educational scope.

---

## Roadmap

Possible future improvements:

- SDK-level Sampler and Estimator primitives
- fluent circuit builder API
- NuGet package publication
- GitHub Actions CI badge
- BenchmarkDotNet benchmark project
- SVG circuit rendering
- richer OpenQASM support
- OpenQASM gate modifiers such as `pow` and `negctrl`
- noise channels
- density matrix simulation
- Bloch sphere visualization
- simple transpilation and optimization passes
- expanded teaching worksheets

---

## Contributing

Contributions suitable for computer-lab and teaching use are welcome.

Good contribution areas include:

- additional tests
- more example circuits
- clearer lab exercises
- documentation improvements
- additional OpenQASM examples
- small well-tested simulator features
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

## Academic Use

This project may be used for teaching, demonstrations, coursework, and student labs. Instructors are encouraged to adapt the examples and lab structure to their own module learning outcomes.
