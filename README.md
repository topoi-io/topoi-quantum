# N-Qubit Quantum Computer Simulator  
### C# / .NET 10 Dense State-Vector Quantum Circuit Simulator

A small, educational N-qubit quantum computer simulator written in C# and .NET 10.

The simulator supports unitary gates, entanglement, measurement collapse, repeated sampling, Pauli expectation values, script execution, gate-only circuit loading, Unicode circuit drawing, command-line execution, OpenQASM 3.1 import, OpenQASM execution, OpenQASM export, seeded randomness for repeatable simulations, and an instance-based simulation API.

The project is intentionally focused on **clarity, correctness, and educational value** rather than high-performance quantum simulation.

---

## Overview

This project simulates pure quantum states using a dense state-vector model.

It can be used in four ways:

1. **Interactive REPL**
2. **Command-line execution**
3. **OpenQASM 3.1 import/export**
4. **Reusable simulator library API**

The solution is split into separate projects:

~~~text
QuantumComputer
QuantumComputer.Core
QuantumComputer.Cli
QuantumComputer.Parsing
QuantumComputer.Drawing
QuantumComputer.OpenQasm
QuantumComputer.Tests
~~~

This keeps the quantum simulation engine independent from CLI, parsing, drawing, OpenQASM import/export, and console output.

---

## Core Technology

The simulator uses a **C#/.NET 10 dense state-vector quantum circuit simulation model**, where an N-qubit quantum state is represented as `2^n` complex amplitudes and quantum gates are applied by updating that state vector.

---

## Features

- N-qubit dense state-vector simulation
- Instance-based `QuantumSimulator`
- Static `Quantum` facade for simple interactive usage
- Standard single-qubit gates:
  - I / ID
  - X, Y, Z
  - H
  - S, SDG
  - T, TDG
  - SX, SXDG
- Rotation gates:
  - RX, RY, RZ
- Controlled and entangling gates:
  - CX / CNOT
  - CY
  - CZ
  - CH
  - CP
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
- OpenQASM 3.1 import support
- OpenQASM circuit execution using `--qasm <path>`
- OpenQASM export from `QuantumCircuit`
- OpenQASM `stdgates.inc` subset support
- OpenQASM bit declarations
- OpenQASM measurement into classical bits
- OpenQASM reset support
- OpenQASM barrier support as a no-op marker
- OpenQASM non-parameterized custom gate definitions
- Cryptographically strong randomness by default
- Seeded randomness support for repeatable tests/simulations
- NUnit regression tests
- Comment support in script and circuit files using `#`
- OpenQASM comment support using `//`

---

## Requirements

- .NET 10
- Windows, macOS, or Linux

---

## Solution Structure

~~~text
QuantumComputer.slnx

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

QuantumComputer.OpenQasm
├── OpenQasmBitReference.cs
├── OpenQasmCircuitConverter.cs
├── OpenQasmCircuitLoader.cs
├── OpenQasmExecutableConverter.cs
├── OpenQasmExecutableOperation.cs
├── OpenQasmExecutableProgram.cs
├── OpenQasmExecutionResult.cs
├── OpenQasmExecutor.cs
├── OpenQasmExporter.cs
├── OpenQasmGateDefinitionExpander.cs
├── OpenQasmGateMapper.cs
├── OpenQasmLexer.cs
├── OpenQasmParseException.cs
├── OpenQasmParser.cs
├── OpenQasmProgram.cs
├── OpenQasmQubitOperand.cs
├── OpenQasmQubitReference.cs
├── OpenQasmStatement.cs
├── OpenQasmToken.cs
└── OpenQasmTokenKind.cs

QuantumComputer.Tests
├── AdditionalStandardGateTests.cs
├── CircuitDrawerTests.cs
├── ControlledRotationTests.cs
├── EntanglementTests.cs
├── ExpectationValueTests.cs
├── MeasurementTests.cs
├── OpenQasmExporterTests.cs
├── OpenQasmExecutableTests.cs
├── OpenQasmGateDefinitionTests.cs
├── OpenQasmParserTests.cs
├── PhaseGateTests.cs
├── QuantumCircuitTests.cs
├── QuantumSimulatorTests.cs
├── SingleQubitGateTests.cs
└── TestHelpers.cs

examples
├── bell.qasm
├── custom-bell.qasm
├── ghz3.qasm
└── rotations.qasm
~~~

---

## Project Responsibilities

### `QuantumComputer.Core`

Contains the core quantum simulation engine.

This project has no dependency on CLI, drawing, parsing, OpenQASM, or console output.

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

Contains text-to-model conversion logic for the native `.qc` format.

It contains:

- angle parsing
- Pauli observable parsing
- gate operation parsing
- native circuit file loading

### `QuantumComputer.Drawing`

Contains Unicode circuit drawing.

It depends on `QuantumComputer.Core`, but the core simulator does not depend on drawing.

### `QuantumComputer.OpenQasm`

Contains OpenQASM 3.1 import, execution-model support, custom-gate expansion, and export.

It contains:

- OpenQASM token model
- OpenQASM lexer
- OpenQASM parser
- OpenQASM AST models
- OpenQASM-to-`QuantumCircuit` converter
- OpenQASM executable-program model
- OpenQASM executor
- OpenQASM standard-gate mapper
- OpenQASM custom-gate definition expander
- OpenQASM exporter

### `QuantumComputer`

The console application startup project.

`Program.cs` is intentionally small and delegates most work to the CLI, parsing, drawing, OpenQASM, and core projects.

### `QuantumComputer.Tests`

NUnit test project covering the simulator, circuit model, drawing, measurement, entanglement, rotations, expectations, seeded randomness, OpenQASM import, OpenQASM execution, OpenQASM export, and custom-gate expansion.

---

## Architecture

The intended dependency direction is:

~~~text
QuantumComputer
    ↓
QuantumComputer.Cli
QuantumComputer.Drawing
QuantumComputer.Parsing
QuantumComputer.OpenQasm
    ↓
QuantumComputer.Core
~~~

The core rule is:

~~~text
Core must not depend on CLI, Drawing, Parsing, OpenQASM, or the console app.
~~~

The simulator engine is therefore reusable independently of the REPL, CLI, drawing system, native parser, and OpenQASM tooling.

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
--qasm <path>                Load and run an OpenQASM 3.1 circuit file
--openqasm <path>            Alias for --qasm
--print-circuit              Print loaded circuit operation list before execution
--draw                       Draw loaded circuit before execution
--print                      Print final state amplitudes and probabilities
--probs                      Print final basis-state probabilities
--expect "observable"        Print a Pauli expectation value
--sample <n>                 Sample the final state n times
~~~

### CLI Examples

Run a full native script:

~~~bash
dotnet run --project QuantumComputer -- --qubits 2 --run examples/bell.qc
~~~

Load and run a native gate-only circuit:

~~~bash
dotnet run --project QuantumComputer -- --qubits 2 --circuit circuits/bell.qc --print
~~~

Load and run an OpenQASM circuit:

~~~bash
dotnet run --project QuantumComputer -- --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
~~~

Load and run an OpenQASM custom-gate circuit:

~~~bash
dotnet run --project QuantumComputer -- --qasm examples/custom-bell.qasm --draw --print --expect "ZZ 0 1"
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

## OpenQASM 3.1 Support

The simulator includes OpenQASM 3.1 import and export support.

The importer currently supports a practical subset of OpenQASM 3.1 that can be mapped onto the simulator's dense state-vector execution model.

Currently supported:

- `OPENQASM 3;`
- `OPENQASM 3.0;`
- `OPENQASM 3.1;`
- `include "stdgates.inc";`
- `qubit[n] q;`
- `bit[n] c;`
- Standard gate calls
- Measurement into classical bits
- Reset
- Barrier as a no-op marker
- Non-parameterized custom gate definitions
- OpenQASM export from `QuantumCircuit`
- Line comments using `//`

---

## Supported OpenQASM Standard Gates

The following OpenQASM standard-library gates are currently supported:

### Single-qubit gates

~~~qasm
id q[0];
x q[0];
y q[0];
z q[0];
h q[0];
s q[0];
sdg q[0];
t q[0];
tdg q[0];
sx q[0];
sxdg q[0];
~~~

### Rotation gates

~~~qasm
rx(pi / 2) q[0];
ry(pi / 3) q[0];
rz(pi) q[0];
~~~

### Two-qubit gates

~~~qasm
cx q[0], q[1];
cy q[0], q[1];
cz q[0], q[1];
ch q[0], q[1];
cp(pi / 2) q[0], q[1];
swap q[0], q[1];
~~~

### Three-qubit gates

~~~qasm
ccx q[0], q[1], q[2];
~~~

### Controlled rotation gates

~~~qasm
crx(pi / 2) q[0], q[1];
cry(pi / 2) q[0], q[1];
crz(pi / 2) q[0], q[1];
~~~

---

## OpenQASM Angle Expressions

OpenQASM angle expressions currently support:

- numbers
- `pi`
- `+`
- `-`
- `*`
- `/`
- parentheses

Examples:

~~~qasm
rx(pi / 2) q[0];
ry(3 * pi / 4) q[0];
rz((pi + pi) / 2) q[0];
cp(pi / 2) q[0], q[1];
~~~

---

## OpenQASM Measurement Support

The executable OpenQASM model supports classical bit declarations and measurement into classical bits.

Supported declaration:

~~~qasm
bit[2] c;
~~~

Supported measurement assignment syntax:

~~~qasm
c[0] = measure q[0];
c[1] = measure q[1];
~~~

Legacy measurement syntax is also supported:

~~~qasm
measure q[0] -> c[0];
measure q[1] -> c[1];
~~~

Example:

~~~qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;
bit[2] c;

h q[0];
cx q[0], q[1];

c[0] = measure q[0];
c[1] = measure q[1];
~~~

Measurement collapses the quantum state and stores the result in the target classical bit.

---

## OpenQASM Reset Support

Reset is supported for individual qubits:

~~~qasm
reset q[0];
~~~

The simulator implements reset by measuring the qubit and applying `x` if the measured result is `1`.

Example:

~~~qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[1] q;

x q[0];
reset q[0];
~~~

After reset, `q[0]` is returned to `|0⟩`.

---

## OpenQASM Barrier Support

Barrier is supported as a no-op circuit marker.

Example:

~~~qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;

h q[0];
barrier q[0], q[1];
cx q[0], q[1];
~~~

In this simulator, `barrier` does not change the quantum state. It is currently preserved in the executable OpenQASM operation model but ignored during execution.

---

## OpenQASM Custom Gate Definitions

The simulator supports non-parameterized OpenQASM custom gate definitions.

Example:

~~~qasm
OPENQASM 3.1;
include "stdgates.inc";

gate bell a, b {
    h a;
    cx a, b;
}

qubit[2] q;

bell q[0], q[1];
~~~

This expands internally to:

~~~qasm
h q[0];
cx q[0], q[1];
~~~

Run from the command line:

~~~bash
dotnet run --project QuantumComputer -- --qasm examples/custom-bell.qasm --draw --print --expect "ZZ 0 1"
~~~

Expected drawing:

~~~text
q0: ─H──●─
        │
q1: ────X─
~~~

Nested custom gate definitions are also supported:

~~~qasm
OPENQASM 3.1;
include "stdgates.inc";

gate flip a {
    x a;
}

gate doubleflip a {
    flip a;
    flip a;
}

qubit[1] q;

doubleflip q[0];
~~~

The expander detects recursive gate definitions and throws an `OpenQasmParseException`.

Currently supported:

- non-parameterized custom gates
- custom gates that call standard gates
- custom gates that call earlier custom gates
- nested custom-gate expansion
- duplicate gate definition detection
- recursion detection

Not yet supported:

- parameterized custom gate definitions
- custom gate angle-parameter substitution
- custom gate bodies containing measurement/reset/barrier
- custom gate modifiers such as `ctrl`, `negctrl`, `inv`, or `pow`

---

## OpenQASM Export

The simulator can export a `QuantumCircuit` to OpenQASM 3.1.

Example:

~~~csharp
using QuantumComputer.Core;
using QuantumComputer.OpenQasm;

var circuit = new QuantumCircuit(2);

circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));
circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 1 }));

string qasm = OpenQasmExporter.Export(circuit);

Console.WriteLine(qasm);
~~~

Output:

~~~qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;

h q[0];
cx q[0], q[1];
~~~

The exporter currently targets gate-only `QuantumCircuit` instances.

It exports:

- standard gates
- rotation gates
- controlled gates
- controlled rotation gates
- qubit declarations

It does not yet export:

- bit declarations
- measurements
- reset
- barrier
- custom gate definitions
- comments
- formatting metadata

---

## OpenQASM Limitations

The OpenQASM support is intentionally a focused subset.

Currently not supported:

- parameterized custom gate definitions
- parameterized custom gate expansion
- OpenQASM aliases
- physical qubits
- timing and duration types
- `delay`
- `box`
- `def`
- `defcal`
- `cal`
- `if`
- `for`
- `while`
- arbitrary classical declarations
- arrays
- structs
- input/output declarations
- gate modifiers such as `ctrl`, `negctrl`, `inv`, `pow`
- full OpenQASM semantic validation
- full source-span diagnostics

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
I q                         Identity gate on qubit q
ID q                        Alias for I, if enabled in parser/CLI
X q                         Pauli-X / bit flip on qubit q
Y q                         Pauli-Y on qubit q
Z q                         Pauli-Z / phase flip on qubit q
H q                         Hadamard gate on qubit q
S q                         Phase gate on qubit q
SDG q                       Inverse S gate on qubit q
T q                         Pi-over-8 gate on qubit q
TDG q                       Inverse T gate on qubit q
SX q                        Square-root of X gate on qubit q
SXDG q                      Inverse square-root of X gate on qubit q
~~~

Examples:

~~~text
X 0
H 1
Z 2
SX 0
SDG 1
TDG 2
~~~

---

## Rotation Gates

~~~text
RX q theta                  Rotation about X axis on qubit q
RY q theta                  Rotation about Y axis on qubit q
RZ q theta                  Rotation about Z axis on qubit q
~~~

Angles are in radians.

Accepted formats in the native `.qc` command language:

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

OpenQASM angle expressions also support spaces and parentheses, for example:

~~~qasm
rx(pi / 2) q[0];
ry(3 * pi / 4) q[0];
rz((pi + pi) / 2) q[0];
~~~

---

## Controlled and Entangling Gates

~~~text
CX c t                      Controlled-X / CNOT
CNOT c t                    Alias for CX
CY c t                      Controlled-Y
CZ c t                      Controlled-Z
CH c t                      Controlled-H
CP c t theta                Controlled phase
SWAP q1 q2                  Swap two qubits
CCX c1 c2 t                 Toffoli gate
TOFFOLI c1 c2 t             Alias for CCX
~~~

Examples:

~~~text
CX 0 1
CY 0 1
CZ 0 1
CH 0 1
CP 0 1 pi/2
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

Gate-only native circuit files can be loaded into a `QuantumCircuit`.

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

## Native Circuit Examples

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

## OpenQASM Examples

### `examples/bell.qasm`

~~~qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;

h q[0];
cx q[0], q[1];
~~~

Run from the command line:

~~~bash
dotnet run --project QuantumComputer -- --qasm examples/bell.qasm --draw --print --expect "ZZ 0 1"
~~~

Expected drawing:

~~~text
q0: ─H──●─
        │
q1: ────X─
~~~

Expected probabilities:

~~~text
|00⟩  P = 0.500000
|11⟩  P = 0.500000
~~~

---

### `examples/custom-bell.qasm`

~~~qasm
OPENQASM 3.1;
include "stdgates.inc";

gate bell a, b {
    h a;
    cx a, b;
}

qubit[2] q;

bell q[0], q[1];
~~~

Run from the command line:

~~~bash
dotnet run --project QuantumComputer -- --qasm examples/custom-bell.qasm --draw --print --expect "ZZ 0 1"
~~~

Expected drawing:

~~~text
q0: ─H──●─
        │
q1: ────X─
~~~

Expected probabilities:

~~~text
|00⟩  P = 0.500000
|11⟩  P = 0.500000
~~~

---

### `examples/ghz3.qasm`

~~~qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[3] q;

h q[0];
cx q[0], q[1];
cx q[1], q[2];
~~~

Run from the command line:

~~~bash
dotnet run --project QuantumComputer -- --qasm examples/ghz3.qasm --draw --print --expect "ZZ 0 1" --expect "ZZ 1 2" --expect "XXX 0 1 2"
~~~

Expected probabilities:

~~~text
|000⟩  P = 0.500000
|111⟩  P = 0.500000
~~~

---

### `examples/rotations.qasm`

~~~qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[1] q;

ry(pi / 3) q[0];
~~~

Run from the command line:

~~~bash
dotnet run --project QuantumComputer -- --qasm examples/rotations.qasm --draw --print --expect "Z 0"
~~~

Expected probabilities:

~~~text
|0⟩  P = 0.750000
|1⟩  P = 0.250000
~~~

Expected Z expectation:

~~~text
⟨Z0⟩ = 0.5
~~~

---

### OpenQASM measurement example

~~~qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;
bit[2] c;

h q[0];
cx q[0], q[1];

c[0] = measure q[0];
c[1] = measure q[1];
~~~

This creates a Bell state and then measures both qubits into classical bits.

---

### OpenQASM reset example

~~~qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[1] q;

x q[0];
reset q[0];
~~~

After execution, the qubit has been reset to `|0⟩`.

---

### OpenQASM barrier example

~~~qasm
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;

h q[0];
barrier q[0], q[1];
cx q[0], q[1];
~~~

The barrier is accepted and treated as a no-op during simulation.

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

Loading an OpenQASM circuit:

~~~csharp
using QuantumComputer.Core;
using QuantumComputer.OpenQasm;

QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromFile("examples/bell.qasm");

var simulator = new QuantumSimulator(circuit.QubitCount);

circuit.Run(simulator);

double[] probabilities = simulator.Register.Probabilities();
~~~

Loading OpenQASM from a string:

~~~csharp
using QuantumComputer.Core;
using QuantumComputer.OpenQasm;

const string source = """
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;

h q[0];
cx q[0], q[1];
""";

QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);
~~~

Loading an OpenQASM custom-gate circuit:

~~~csharp
using QuantumComputer.Core;
using QuantumComputer.OpenQasm;

const string source = """
OPENQASM 3.1;
include "stdgates.inc";

gate bell a, b {
    h a;
    cx a, b;
}

qubit[2] q;

bell q[0], q[1];
""";

QuantumCircuit circuit = OpenQasmCircuitLoader.LoadFromString(source);
~~~

Executing an OpenQASM program with classical bits:

~~~csharp
using QuantumComputer.OpenQasm;

const string source = """
OPENQASM 3.1;
include "stdgates.inc";

qubit[2] q;
bit[2] c;

h q[0];
cx q[0], q[1];

c[0] = measure q[0];
c[1] = measure q[1];
""";

OpenQasmExecutableProgram program =
    OpenQasmCircuitLoader.LoadExecutableFromString(source);

OpenQasmExecutionResult result =
    OpenQasmExecutor.Execute(program);

int[] classicalBits = result.ClassicalBits;
~~~

Exporting a circuit to OpenQASM:

~~~csharp
using QuantumComputer.Core;
using QuantumComputer.OpenQasm;

var circuit = new QuantumCircuit(2);

circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));
circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 1 }));

string qasm = OpenQasmExporter.Export(circuit);
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

The OpenQASM importer maps:

~~~qasm
q[0]
~~~

to simulator qubit `0`, which is the least significant bit in the state-vector basis index.

---

## Testing

The project includes an NUnit test project.

Run all tests with:

~~~bash
dotnet test
~~~

The tests cover:

- Single-qubit gate behaviour
- Identity gate behaviour
- Phase gate amplitudes
- Inverse phase gates
- Square-root X gates
- Rotation gates
- Controlled gates
- Controlled rotation gates
- Controlled phase gate
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
- OpenQASM tokenization/parsing
- OpenQASM-to-`QuantumCircuit` conversion
- OpenQASM executable-program conversion
- OpenQASM measurement into classical bits
- OpenQASM reset
- OpenQASM barrier no-op behaviour
- OpenQASM Bell and GHZ circuit execution
- OpenQASM rotation angle parsing
- OpenQASM additional standard gates
- OpenQASM custom-gate expansion
- OpenQASM recursive gate detection
- OpenQASM export
- OpenQASM import/export round trips

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
- Native `.qc` text parsing is handled outside Core by `QuantumComputer.Parsing`
- OpenQASM import/export is handled outside Core by `QuantumComputer.OpenQasm`
- OpenQASM gate-only import can produce a `QuantumCircuit`
- OpenQASM programs containing measurement/reset/barrier use an executable program model
- OpenQASM custom gates are expanded before circuit conversion/execution
- OpenQASM export currently targets gate-only `QuantumCircuit` instances

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
- OpenQASM 3.1 support is currently a practical subset, not full compliance
- OpenQASM parameterized custom gate definitions are not supported yet
- OpenQASM custom gate parameter substitution is not supported yet
- OpenQASM aliases are not supported yet
- OpenQASM physical qubits are not supported yet
- OpenQASM timing/duration types are not supported yet
- OpenQASM classical control flow is not supported yet
- OpenQASM gate modifiers are not supported yet
- OpenQASM export currently supports gate-only `QuantumCircuit` instances
- No gate optimization
- No circuit transpilation
- No hardware backend abstraction
- Native circuit loading currently supports unitary gate operations only
- Circuit drawing is text/Unicode based, not graphical
- No plain ASCII fallback drawing mode yet
- No SVG/PNG circuit export yet

---

## Possible Extensions

- Parameterized OpenQASM custom gate definitions
- OpenQASM custom gate angle substitution
- More complete OpenQASM export
- OpenQASM aliases
- OpenQASM classical control flow
- OpenQASM `if`
- OpenQASM `for`
- OpenQASM `while`
- OpenQASM `delay`
- OpenQASM `box`
- OpenQASM `def`
- OpenQASM `defcal`
- OpenQASM `cal`
- OpenQASM gate modifiers:
  - `ctrl`
  - `negctrl`
  - `inv`
  - `pow`
- Native circuit export
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
  - P / Phase
  - U
  - U1
  - U2
  - U3
  - CY variants
  - CCZ
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
- dense state-vector simulation
- OpenQASM parsing
- OpenQASM custom-gate expansion
- OpenQASM export
- simulator architecture
- test-driven numerical software

The emphasis is on conceptual correctness, readable implementation, and a clear path from simple single-qubit behaviour to entanglement, circuit representation, command-line execution, testing, OpenQASM import/export, custom gate expansion, and multi-qubit simulation.

---

## License

MIT