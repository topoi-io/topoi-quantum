# N-Qubit Quantum Computer Simulator  
### C# / .NET 10 Dense State-Vector Quantum Circuit Simulator

A small, educational N-qubit quantum computer simulator written in C# and .NET 10.

The simulator supports unitary gates, entanglement, measurement collapse, repeated sampling, Pauli expectation values, script execution, gate-only circuit loading, Unicode circuit drawing, command-line execution, seeded randomness for repeatable simulations, and an instance-based simulation API.

The project is intentionally focused on **clarity, correctness, and educational value** rather than high-performance quantum simulation.

---

## Overview

This project simulates pure quantum states using a dense state-vector model.

It can be used in three ways:

1. **Interactive REPL**
2. **Command-line execution**
3. **Reusable simulator library API**

The solution is now split into separate projects:

~~~text
QuantumComputer
QuantumComputer.Core
QuantumComputer.Cli
QuantumComputer.Parsing
QuantumComputer.Drawing
QuantumComputer.Tests
~~~

This keeps the quantum simulation engine independent from CLI, parsing, drawing, and console output.

---

## Features

- N-qubit dense state-vector simulation
- Instance-based `QuantumSimulator`
- Static `Quantum` facade for simple interactive usage
- Standard single-qubit gates:
  - X, Y, Z, H, S, T
- Rotation gates:
  - RX, RY, RZ
- Controlled and entangling gates:
  - CX / CNOT
  - CZ
  - SWAP
  - CCX / Toffoli
- Controlled rotation gates:
  - CRX, CRY, CRZ
- Bell-state and GHZ-state capable
- Full-register measurement with state collapse
- Single-qubit measurement with partial collapse
- Repeated sampling using `SAMPLE`
- Basis-state probability display using `PROBS`
- Pauli expectation values using `EXPECT`
- Dense state-vector memory estimation using `MEM`
- State norm diagnostics using `NORM`
- Manual state normalization using `NORMALIZE`
- Script execution using `RUN <path>`
- Gate-only circuit loading using `LOAD <path>`
- Circuit inspection using `CIRCUIT`
- Circuit execution using `RUNCIRCUIT`
- Unicode circuit drawing using `DRAW`
- Command-line script and circuit execution
- Command-line circuit drawing using `--draw`
- Cryptographically strong randomness by default
- Seeded randomness support for repeatable tests/simulations
- NUnit regression tests
- Comment support in script and circuit files using `#`

---

## Requirements

- .NET 10
- Windows, macOS, or Linux

---

## Solution Structure

~~~text
QuantumComputer.sln

QuantumComputer
├── Program.cs

QuantumComputer.Core
├── CryptoRandomSource.cs
├── IRandomSource.cs
├── SeededRandomSource.cs
├── GateKind.cs
├── GateOperation.cs
├── GateSpecs.cs
├── PauliTerm.cs
├── Quantum.cs
├── QuantumCircuit.cs
├── QuantumRegister.cs
└── QuantumSimulator.cs

QuantumComputer.Cli
├── CliOptions.cs
├── CliOptionsParser.cs
├── CommandExecutor.cs
├── HelpPrinter.cs
└── QuantumConsolePrinter.cs

QuantumComputer.Parsing
├── AngleParser.cs
├── CircuitFileLoader.cs
├── GateOperationParser.cs
└── ObservableParser.cs

QuantumComputer.Drawing
└── CircuitDrawer.cs

QuantumComputer.Tests
├── CircuitDrawerTests.cs
├── ControlledRotationTests.cs
├── EntanglementTests.cs
├── ExpectationValueTests.cs
├── MeasurementTests.cs
├── PhaseGateTests.cs
├── QuantumCircuitTests.cs
├── QuantumSimulatorTests.cs
├── SingleQubitGateTests.cs
└── TestHelpers.cs
~~~

---

## Project Responsibilities

### `QuantumComputer.Core`

Contains the core quantum simulation engine.

This project has no dependency on CLI, drawing, parsing, or console output.

It contains:

- `QuantumSimulator`
- `QuantumRegister`
- `QuantumCircuit`
- `GateOperation`
- `GateKind`
- `GateSpecs`
- `PauliTerm`
- `Quantum`
- `IRandomSource`
- `CryptoRandomSource`
- `SeededRandomSource`

### `QuantumComputer.Cli`

Contains command-line and REPL behaviour.

It contains:

- interactive command execution
- command-line option parsing
- help text
- console output formatting
- sampling output
- expectation value output
- memory/norm display

### `QuantumComputer.Parsing`

Contains text-to-model conversion logic.

It contains:

- angle parsing
- Pauli observable parsing
- gate operation parsing
- circuit file loading

### `QuantumComputer.Drawing`

Contains Unicode circuit drawing.

It depends on `QuantumComputer.Core` but the core simulator does not depend on drawing.

### `QuantumComputer`

The console application startup project.

`Program.cs` is intentionally small and delegates most work to the CLI project.

### `QuantumComputer.Tests`

NUnit test project covering the simulator, circuit model, drawing, measurement, entanglement, rotations, and expectations.

---

## Architecture

The intended dependency direction is:

~~~text
QuantumComputer
    ↓
QuantumComputer.Cli
    ↓
QuantumComputer.Parsing
QuantumComputer.Drawing
    ↓
QuantumComputer.Core
~~~

The core rule is:

~~~text
Core must not depend on CLI, Drawing, Parsing, or the console app.
~~~

The simulator engine is therefore reusable independently of the REPL or CLI.

---

## Build

From the solution directory:

~~~bash
dotnet build
~~~

Run all tests:

~~~bash
dotnet test
~~~

Run the interactive simulator:

~~~bash
dotnet run --project QuantumComputer
~~~

---

## Interactive Usage

When the simulator starts, enter the number of qubits:

~~~text
N-Qubit Gate Interpreter (state-vector)
Number of qubits n (e.g. 1,2,3): 2
~~~

Then enter commands:

~~~text
H 0
CX 0 1
PRINT
EXPECT ZZ 0 1
EXPECT XX 0 1
SAMPLE 1000
~~~

---

## Command-Line Usage

The simulator can also run directly from the command line without entering the REPL.

### CLI Options

~~~text
--help, -h                  Show command-line help
--qubits <n>, -q <n>         Number of qubits to initialise
--run <path>                 Run a full interpreter script and exit
--circuit <path>             Load and run a gate-only QuantumCircuit file and exit
--print-circuit              Print loaded circuit operation list before execution
--draw                       Draw loaded circuit before execution
--print                      Print final state amplitudes and probabilities
--probs                      Print final basis-state probabilities
--expect "observable"        Print a Pauli expectation value
--sample <n>                 Sample the final state n times
~~~

### CLI Examples

Run a full script:

~~~bash
dotnet run --project QuantumComputer -- --qubits 2 --run examples/bell.qc
~~~

Load and run a gate-only circuit:

~~~bash
dotnet run --project QuantumComputer -- --qubits 2 --circuit circuits/bell.qc --print
~~~

Draw a Bell circuit and compute expectations:

~~~bash
dotnet run --project QuantumComputer -- --qubits 2 --circuit circuits/bell.qc --draw --expect "ZZ 0 1" --expect "XX 0 1"
~~~

Run and sample a GHZ circuit:

~~~bash
dotnet run --project QuantumComputer -- --qubits 3 --circuit circuits/ghz3.qc --draw --probs --sample 1000
~~~

Run a Toffoli circuit:

~~~bash
dotnet run --project QuantumComputer -- --qubits 3 --circuit circuits/toffoli.qc --draw --print
~~~

---

## Commands

### State Control

~~~text
RESET                       Reset the register to |00..0>
PRINT                       Print top basis amplitudes and probabilities
PROBS                       Show top basis-state probabilities without measurement
PROBABILITIES               Alias for PROBS
NORM                        Show the current state norm
NORMALIZE                   Manually renormalize the state vector
MEM                         Show estimated dense state-vector memory usage
~~~

---

## Single-Qubit Gates

All single-qubit gates require a target qubit index.

~~~text
X q                         Pauli-X / bit flip on qubit q
Y q                         Pauli-Y on qubit q
Z q                         Pauli-Z / phase flip on qubit q
H q                         Hadamard gate on qubit q
S q                         Phase gate on qubit q
T q                         Pi-over-8 gate on qubit q
~~~

Examples:

~~~text
X 0
H 1
Z 2
~~~

---

## Rotation Gates

~~~text
RX q theta                  Rotation about X axis on qubit q
RY q theta                  Rotation about Y axis on qubit q
RZ q theta                  Rotation about Z axis on qubit q
~~~

Angles are in radians.

Accepted formats:

~~~text
pi
+pi/2
-pi/8
3*pi/4
-3*pi/4
0.5*pi
1.57079632679
~~~

Examples:

~~~text
RX 0 pi/2
RY 1 -pi/8
RZ 2 3*pi/4
~~~

---

## Controlled and Entangling Gates

~~~text
CX c t                      Controlled-X / CNOT
CNOT c t                    Alias for CX
CZ c t                      Controlled-Z
SWAP q1 q2                  Swap two qubits
CCX c1 c2 t                 Toffoli gate
TOFFOLI c1 c2 t             Alias for CCX
~~~

Examples:

~~~text
CX 0 1
CZ 0 1
SWAP 0 2
CCX 0 1 2
~~~

---

## Controlled Rotation Gates

~~~text
CRX c t theta               Controlled RX rotation
CRY c t theta               Controlled RY rotation
CRZ c t theta               Controlled RZ rotation
~~~

Examples:

~~~text
CRX 0 1 pi/2
CRY 1 2 -pi/4
CRZ 0 2 pi
~~~

---

## Measurement and Sampling

~~~text
MEASURE q                   Measure one qubit and partially collapse the state
MEASUREALL                  Measure full computational basis and collapse state
SAMPLE n                    Repeatedly measure current state n times
~~~

Examples:

~~~text
MEASURE 0
MEASUREALL
SAMPLE 1000
~~~

`SAMPLE` restores the prepared state after each trial, so it does not permanently collapse the state between samples.

---

## Expectation Values

`EXPECT` computes Pauli observable expectation values.

Supported forms include:

~~~text
EXPECT Z 0
EXPECT X 1
EXPECT Y 2
EXPECT ZZ 0 1
EXPECT XX 0 1
EXPECT XXX 0 1 2
EXPECT Z0 Z1
~~~

Examples:

~~~text
EXPECT Z 0
EXPECT ZZ 0 1
EXPECT XX 0 1
~~~

For a Bell state, individual Z expectations are zero, but two-qubit correlations are non-zero:

~~~text
EXPECT Z 0
EXPECT Z 1
EXPECT ZZ 0 1
EXPECT XX 0 1
~~~

---

## Randomness

The simulator supports two randomness modes.

### Default randomness

By default, measurements use cryptographically strong randomness through `CryptoRandomSource`.

### Seeded randomness

For repeatable simulations and tests, use `SeededRandomSource`.

Example:

~~~csharp
using QuantumComputer.Core;

var simulator = new QuantumSimulator(1, new SeededRandomSource(123));

simulator.H(0);

int outcome = simulator.MeasureAll();
~~~

This is useful when you want deterministic test behaviour.

### QRAND

The REPL command:

~~~text
QRAND
~~~

or:

~~~text
QRAND k
~~~

generates random bits by:

1. resetting the register
2. applying Hadamard gates
3. measuring the full register
4. returning the lowest `k` bits

Examples:

~~~text
QRAND
QRAND 4
~~~

---

## Script Execution

Run commands from a script file:

~~~text
RUN examples/bell.qc
~~~

Script files support blank lines and comments using `#`.

Example:

~~~text
# Bell state: (|00> + |11>) / sqrt(2)

RESET
H 0
CX 0 1

PRINT

EXPECT Z 0
EXPECT Z 1
EXPECT ZZ 0 1
EXPECT XX 0 1

SAMPLE 1000
~~~

Run from the command line:

~~~bash
dotnet run --project QuantumComputer -- --qubits 2 --run examples/bell.qc
~~~

---

## Circuit Loading and Drawing

Gate-only circuit files can be loaded into a `QuantumCircuit`.

~~~text
LOAD path                   Load gate operations from a .qc file
CIRCUIT                     Print currently loaded circuit operation list
DRAW                        Draw loaded circuit as a Unicode diagram
RUNCIRCUIT                  Execute loaded circuit
CLEARCIRCUIT                Clear loaded circuit
~~~

Example:

~~~text
LOAD circuits/bell.qc
CIRCUIT
DRAW
RUNCIRCUIT
PRINT
~~~

Gate-only circuit files should contain unitary gate commands only.

`RESET` is allowed and ignored when loading because `RUNCIRCUIT` resets before execution.

Example circuit file:

~~~text
# Bell circuit only

H 0
CX 0 1
~~~

---

## Example Usage

### Single-Qubit Superposition

~~~text
RESET
H 0
PRINT
PROBS
~~~

### Sampling

~~~text
RESET
H 0
SAMPLE 10000
~~~

### Biased Rotation

~~~text
RESET
RY 0 pi/3
PROBS
SAMPLE 2000
~~~

### Bell State

~~~text
RESET
H 0
CX 0 1
PRINT
EXPECT ZZ 0 1
EXPECT XX 0 1
~~~

Expected Bell-state probabilities:

~~~text
|00> : 0.5
|11> : 0.5
~~~

### GHZ State

~~~text
RESET
H 0
CX 0 1
CX 1 2
PRINT
EXPECT ZZ 0 1
EXPECT ZZ 1 2
EXPECT XXX 0 1 2
~~~

Expected GHZ-state probabilities:

~~~text
|000> : 0.5
|111> : 0.5
~~~

### SWAP

~~~text
RESET
X 0
PRINT
SWAP 0 1
PRINT
~~~

### Toffoli / CCX

~~~text
RESET
X 0
X 1
CCX 0 1 2
PRINT
~~~

Expected result:

~~~text
|111> : 1.0
~~~

---

## Circuit Examples

### `circuits/bell.qc`

~~~text
# Bell circuit: (|00> + |11>) / sqrt(2)

H 0
CX 0 1
~~~

Run in the REPL:

~~~text
LOAD circuits/bell.qc
DRAW
RUNCIRCUIT
PRINT
EXPECT ZZ 0 1
EXPECT XX 0 1
~~~

Run from the command line:

~~~bash
dotnet run --project QuantumComputer -- --qubits 2 --circuit circuits/bell.qc --draw --expect "ZZ 0 1" --expect "XX 0 1"
~~~

Expected drawing:

~~~text
q0: ─H──●─
        │
q1: ────X─
~~~

---

### `circuits/ghz3.qc`

~~~text
# GHZ3 circuit: (|000> + |111>) / sqrt(2)

H 0
CX 0 1
CX 1 2
~~~

Run in the REPL:

~~~text
LOAD circuits/ghz3.qc
DRAW
RUNCIRCUIT
PRINT
EXPECT ZZ 0 1
EXPECT ZZ 1 2
EXPECT XXX 0 1 2
~~~

Run from the command line:

~~~bash
dotnet run --project QuantumComputer -- --qubits 3 --circuit circuits/ghz3.qc --draw --print
~~~

Expected drawing:

~~~text
q0: ─H──●────
        │
q1: ────X──●─
           │
q2: ───────X─
~~~

---

### `circuits/toffoli.qc`

~~~text
# Toffoli circuit

X 0
X 1
CCX 0 1 2
~~~

Run from the command line:

~~~bash
dotnet run --project QuantumComputer -- --qubits 3 --circuit circuits/toffoli.qc --draw --print
~~~

Expected result:

~~~text
|111> : 1.0
~~~

---

## Using the Simulator as a Library

The core simulator can be used without the CLI.

Example:

~~~csharp
using QuantumComputer.Core;

var simulator = new QuantumSimulator(2);

simulator.H(0);
simulator.CX(0, 1);

double[] probabilities = simulator.Register.Probabilities();

Console.WriteLine(probabilities[0]); // |00>
Console.WriteLine(probabilities[3]); // |11>
~~~

Using a circuit:

~~~csharp
using QuantumComputer.Core;

var circuit = new QuantumCircuit(2);

circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));
circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 1 }));

var simulator = new QuantumSimulator(2);

circuit.Run(simulator);

double[] probabilities = simulator.Register.Probabilities();
~~~

Drawing a circuit:

~~~csharp
using QuantumComputer.Core;
using QuantumComputer.Drawing;

var circuit = new QuantumCircuit(2);

circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));
circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 1 }));

string drawing = CircuitDrawer.Draw(circuit);

Console.WriteLine(drawing);
~~~

Parsing a Pauli observable:

~~~csharp
using QuantumComputer.Core;
using QuantumComputer.Parsing;

PauliTerm[] terms = ObservableParser.Parse("ZZ 0 1");
~~~

---

## Mathematical Model

The simulator represents an N-qubit pure quantum state as a dense state vector:

~~~text
|ψ⟩ = Σᵢ αᵢ |i⟩
~~~

The vector contains `2^n` complex amplitudes.

The state is normalized according to:

~~~text
Σᵢ |αᵢ|² = 1
~~~

Single-qubit gates are represented as 2x2 unitary matrices applied to a target qubit:

~~~text
|ψ'⟩ = U |ψ⟩
~~~

Controlled gates apply a target operation only when one or more control qubits are in state `|1⟩`.

Measurement follows the Born rule:

~~~text
P(i) = |αᵢ|²
~~~

After full-register measurement, the state collapses to the measured computational basis state.

After single-qubit measurement, the state collapses to the surviving subspace and is renormalized.

Pauli expectation values are computed as:

~~~text
⟨ψ|O|ψ⟩ / ⟨ψ|ψ⟩
~~~

where `O` is a Pauli observable such as:

~~~text
Z0
X1
Z0 ⊗ Z1
X0 ⊗ X1 ⊗ X2
~~~

---

## Qubit Indexing Convention

The simulator uses the following convention:

~~~text
Qubit 0 is the least significant bit.
~~~

Basis-state indices are stored in little-endian qubit order internally.

However, bit strings are displayed most-significant bit to least-significant bit.

Example:

~~~text
X 0
~~~

on a 2-qubit register prepares:

~~~text
|01⟩
~~~

not:

~~~text
|10⟩
~~~

This is intentional and is common in many state-vector simulators.

---

## Testing

The project includes an NUnit test project.

Run all tests with:

~~~bash
dotnet test
~~~

The tests cover:

- Single-qubit gate behaviour
- Phase gate amplitudes
- Rotation gates
- Controlled rotation gates
- Bell-state preparation
- GHZ-state preparation
- SWAP behaviour
- CCX / Toffoli behaviour
- Pauli expectation values
- Measurement collapse
- Snapshot and restore
- Seeded simulator behaviour
- QuantumCircuit execution
- Circuit validation
- Unicode circuit drawing

---

## Design Notes

- Uses `System.Numerics.Complex` for amplitudes
- Uses a dense state-vector representation of size `2^n`
- Uses `QuantumSimulator` as the main instance-based simulator
- Keeps `Quantum` as a static convenience facade
- Uses `IRandomSource` to abstract measurement randomness
- Uses `CryptoRandomSource` by default
- Supports `SeededRandomSource` for deterministic tests
- `QuantumRegister.State` is exposed as read-only state
- Internal mutation is handled by the simulator
- Qubit 0 is the least significant bit
- Bit strings are printed most-significant bit to least-significant bit
- Measurement follows the Born rule
- Single-qubit measurement performs partial collapse
- Full-register measurement collapses to one computational-basis state
- Debug builds can assert that the state remains normalized
- Periodic normalization is used as a numerical safety mechanism
- Expectation values are computed from Pauli observables
- `QuantumCircuit` stores validated gate operations before execution
- Circuit drawing is handled outside Core by `QuantumComputer.Drawing`
- CLI and console output are handled outside Core by `QuantumComputer.Cli`
- Text parsing is handled outside Core by `QuantumComputer.Parsing`

---

## Memory Model

The simulator uses a dense state vector with `2^n` complex amplitudes.

Each amplitude is a `System.Numerics.Complex` value containing two `double` values:

~~~text
Real      = 8 bytes
Imaginary = 8 bytes
Total     = 16 bytes per amplitude
~~~

Approximate state-vector memory usage:

~~~text
20 qubits = 16 MiB
21 qubits = 32 MiB
22 qubits = 64 MiB
23 qubits = 128 MiB
24 qubits = 256 MiB
25 qubits = 512 MiB
26 qubits = 1 GiB
27 qubits = 2 GiB
28 qubits = 4 GiB
~~~

The default safety limit is 25 qubits.

This limit is intentional because dense state-vector simulators grow exponentially in memory and time.

---

## Limitations

- Dense state-vector simulation only
- Exponential memory growth with qubit count
- Pure states only
- No density matrix representation
- No mixed states
- No noise model
- No decoherence model
- No Bloch sphere visualization
- No OpenQASM import/export yet
- No gate optimization
- No circuit transpilation
- No hardware backend abstraction
- Circuit loading currently supports unitary gate operations only
- Circuit drawing is text/Unicode based, not graphical
- No plain ASCII fallback drawing mode yet
- No SVG/PNG circuit export yet

---

## Possible Extensions

- OpenQASM import
- OpenQASM export
- Circuit export
- Plain ASCII drawing fallback
- SVG circuit drawing
- PNG circuit drawing
- Bloch sphere visualization for single-qubit states
- Density matrix simulator
- Noise channels
- Decoherence models
- Sparse-state simulation
- Tensor-network simulation
- Additional gates:
  - CY
  - CH
  - CS
  - CT
  - phase gates
  - arbitrary unitary gates
- Controlled arbitrary single-qubit gates exposed at command level
- Named commands such as `BELL` and `GHZ`
- More command-line execution options
- More formal circuit validation
- Circuit optimization passes
- Circuit transpilation passes
- Benchmark project
- Documentation site

---

## Educational Purpose

This project is a learning tool for:

- linear algebra
- complex numbers
- quantum mechanics
- quantum gates
- measurement
- entanglement
- quantum circuits
- simulator architecture
- test-driven numerical software

The emphasis is on conceptual correctness, readable implementation, and a clear path from simple single-qubit behaviour to entanglement, circuit representation, command-line execution, testing, and multi-qubit simulation.

---

## License

MIT