# N-Qubit Quantum Gate Interpreter (C# / .NET 10)

A minimal N-qubit quantum computer simulator written in C# (.NET 10).

It simulates unitary quantum gates, entanglement, probabilistic measurement, sampling, Pauli expectation values, script execution, circuit loading, circuit execution, command-line execution, and Unicode circuit drawing using a dense state-vector model.

This project is intentionally small and educational, focusing on clarity and correctness rather than high-performance simulation.

## Features

- N-qubit dense state-vector simulation
- Standard single-qubit gates: X, Y, Z, H, S, T
- Continuous rotation gates: RX, RY, RZ
- Controlled and entangling gates: CX / CNOT, CZ, SWAP, CCX / Toffoli
- Controlled rotation gates: CRX, CRY, CRZ
- Bell-state and GHZ-state capable
- Cryptographically strong randomness for measurement
- Full-register measurement with state collapse
- Single-qubit measurement with partial collapse
- Repeated sampling using SAMPLE
- Basis-state probability display using PROBS
- Real Pauli expectation values using EXPECT
- Dense state-vector memory estimation using MEM
- State norm diagnostics using NORM
- Manual state normalization using NORMALIZE
- Script execution using RUN <path>
- Gate-only circuit loading using LOAD <path>
- Circuit inspection using CIRCUIT
- Circuit execution using RUNCIRCUIT
- Unicode circuit drawing using DRAW
- Command-line script and circuit execution
- Command-line circuit drawing using --draw
- NUnit regression tests
- Comment support in script and circuit files using #

## Mathematical Model

The simulator represents an N-qubit pure quantum state as a dense state vector:

|psi> = sum_i alpha_i |i>

where the vector contains 2^n complex amplitudes.

The state is normalized according to:

sum_i |alpha_i|^2 = 1

Single-qubit gates are represented as 2x2 unitary matrices applied to a target qubit:

|psi'> = U |psi>

Controlled gates apply a target operation only when one or more control qubits are in state |1>.

Measurement follows the Born rule:

P(i) = |alpha_i|^2

After measurement, the state collapses to the measured basis state, or to the surviving subspace in the case of single-qubit measurement.

Pauli expectation values are computed as:

<psi|O|psi> / <psi|psi>

where O is a Pauli observable such as:

Z0  
X1  
Z0 ⊗ Z1  
X0 ⊗ X1 ⊗ X2

## Requirements

- .NET 10
- Windows, macOS, or Linux

## Build and Run

dotnet build  
dotnet run

When the interpreter starts, enter the number of qubits:

Number of qubits n (e.g. 1,2,3): 2

## Command-Line Usage

The interpreter can also run directly from the command line without entering the interactive REPL.

### CLI Options

- --help, -h – Show command-line help
- --qubits n, -q n – Number of qubits to initialise
- --run path – Run a full interpreter script and exit
- --circuit path – Load and run a gate-only QuantumCircuit file and exit
- --print-circuit – Print the loaded circuit operation list before execution
- --draw – Draw the loaded circuit before execution
- --print – Print the final state amplitudes and probabilities
- --probs – Print the final basis-state probabilities
- --expect "observable" – Print a Pauli expectation value
- --sample n – Sample the final state n times

### CLI Examples

Run a full script:

dotnet run -- --qubits 2 --run examples/bell.qc

Load and run a gate-only circuit:

dotnet run -- --qubits 2 --circuit circuits/bell.qc --print

Draw a Bell circuit and compute expectations:

dotnet run -- --qubits 2 --circuit circuits/bell.qc --draw --expect "ZZ 0 1" --expect "XX 0 1"

Run and sample a GHZ circuit:

dotnet run -- --qubits 3 --circuit circuits/ghz3.qc --draw --probs --sample 1000

Run a Toffoli circuit:

dotnet run -- --qubits 3 --circuit circuits/toffoli.qc --draw --print

## Commands

### State Control

- RESET – Reset the register to |00..0>
- PRINT – Print the top basis amplitudes and probabilities
- PROBS – Show top basis-state probabilities without measurement
- PROBABILITIES – Alias for PROBS
- NORM – Show the current state norm
- NORMALIZE – Manually renormalize the state vector
- MEM – Show estimated dense state-vector memory usage

### Single-Qubit Gates

All single-qubit gates require a target qubit index.

- X q – Pauli-X / bit flip on qubit q
- Y q – Pauli-Y on qubit q
- Z q – Pauli-Z / phase flip on qubit q
- H q – Hadamard gate on qubit q
- S q – Phase gate on qubit q
- T q – Pi-over-8 gate on qubit q

Examples:

X 0  
H 1  
Z 2

### Rotation Gates

- RX q theta – Rotation about X axis on qubit q
- RY q theta – Rotation about Y axis on qubit q
- RZ q theta – Rotation about Z axis on qubit q

Angles are in radians.

Accepted formats:

pi  
+pi/2  
-pi/8  
3*pi/4  
-3*pi/4  
0.5*pi  
1.57079632679

Examples:

RX 0 pi/2  
RY 1 -pi/8  
RZ 2 3*pi/4

### Controlled and Entangling Gates

- CX c t – Controlled-X / CNOT; flip target t if control c is 1
- CNOT c t – Alias for CX
- CZ c t – Controlled-Z; phase flip when c and t are both 1
- SWAP q1 q2 – Swap two qubits
- CCX c1 c2 t – Toffoli gate; flip target t if both controls are 1
- TOFFOLI c1 c2 t – Alias for CCX

Examples:

CX 0 1  
CZ 0 1  
SWAP 0 2  
CCX 0 1 2

### Controlled Rotation Gates

- CRX c t theta – Controlled RX rotation
- CRY c t theta – Controlled RY rotation
- CRZ c t theta – Controlled RZ rotation

Examples:

CRX 0 1 pi/2  
CRY 1 2 -pi/4  
CRZ 0 2 pi

### Measurement and Sampling

- MEASURE q – Measure one qubit and partially collapse the state
- MEASUREALL – Measure the full computational basis and collapse the state
- SAMPLE n – Repeatedly measure the current state n times, restoring the prepared state after each trial

Examples:

MEASURE 0  
MEASUREALL  
SAMPLE 1000

### Expectation Values

EXPECT computes real Pauli observable expectation values.

Supported forms include:

EXPECT Z 0  
EXPECT X 1  
EXPECT Y 2  
EXPECT ZZ 0 1  
EXPECT XX 0 1  
EXPECT XXX 0 1 2  
EXPECT Z0 Z1

Examples:

EXPECT Z 0  
EXPECT ZZ 0 1  
EXPECT XX 0 1

For a Bell state, the individual Z expectations are zero, but the two-qubit correlations are non-zero:

EXPECT Z 0  
EXPECT Z 1  
EXPECT ZZ 0 1  
EXPECT XX 0 1

### Randomness

- QRAND – Generate n random bits using all qubits
- QRAND k – Generate k random bits from the lowest k qubits

QRAND resets the register, applies Hadamard gates to all qubits, measures the full register, and returns random bits.

Examples:

QRAND  
QRAND 4

### Script Execution

- RUN path – Run commands from a script file immediately

Script files support blank lines and comments using #.

Example:

RUN examples/bell.qc

Example script file:

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

### Circuit Loading and Drawing

Gate-only circuit files can be loaded into a QuantumCircuit model.

- LOAD path – Load gate operations from a .qc file into a QuantumCircuit
- CIRCUIT – Print the currently loaded circuit as an operation list
- DRAW – Draw the currently loaded circuit as a Unicode circuit diagram
- RUNCIRCUIT – Execute the currently loaded circuit
- CLEARCIRCUIT – Clear the currently loaded circuit

Example:

LOAD circuits/bell.qc  
CIRCUIT  
DRAW  
RUNCIRCUIT  
PRINT

Gate-only circuit files should contain unitary gate commands only. RESET is allowed and ignored when loading because RUNCIRCUIT resets before execution.

Example gate-only circuit file:

# Bell circuit only

H 0  
CX 0 1

### Utility

- HELP – Show available commands
- QUIT – Exit the interpreter
- EXIT – Exit the interpreter

## Example Usage

Create a single-qubit superposition:

RESET  
H 0  
PRINT  
PROBS

Sample statistics:

RESET  
H 0  
SAMPLE 10000

Biased rotation:

RESET  
RY 0 pi/3  
PROBS  
SAMPLE 2000

Create a Bell state:

RESET  
H 0  
CX 0 1  
PRINT  
EXPECT ZZ 0 1  
EXPECT XX 0 1

Expected Bell-state probabilities:

|00> : 0.5  
|11> : 0.5

Create a 3-qubit GHZ state:

RESET  
H 0  
CX 0 1  
CX 1 2  
PRINT  
EXPECT ZZ 0 1  
EXPECT ZZ 1 2  
EXPECT XXX 0 1 2

Expected GHZ-state probabilities:

|000> : 0.5  
|111> : 0.5

Test SWAP:

RESET  
X 0  
PRINT  
SWAP 0 1  
PRINT

Test Toffoli / CCX:

RESET  
X 0  
X 1  
CCX 0 1 2  
PRINT

## Script Examples

### examples/bell.qc

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

Run with:

RUN examples/bell.qc

Or from the command line:

dotnet run -- --qubits 2 --run examples/bell.qc

### examples/ghz3.qc

# GHZ state: (|000> + |111>) / sqrt(2)

RESET  
H 0  
CX 0 1  
CX 1 2

PRINT

EXPECT Z 0  
EXPECT Z 1  
EXPECT Z 2

EXPECT ZZ 0 1  
EXPECT ZZ 1 2  
EXPECT ZZ 0 2

EXPECT XXX 0 1 2

SAMPLE 1000

Run with:

RUN examples/ghz3.qc

Or from the command line:

dotnet run -- --qubits 3 --run examples/ghz3.qc

## Circuit Examples

### circuits/bell.qc

# Bell circuit: (|00> + |11>) / sqrt(2)

H 0  
CX 0 1

Run in the REPL:

LOAD circuits/bell.qc  
DRAW  
RUNCIRCUIT  
PRINT  
EXPECT ZZ 0 1  
EXPECT XX 0 1

Run from the command line:

dotnet run -- --qubits 2 --circuit circuits/bell.qc --draw --expect "ZZ 0 1" --expect "XX 0 1"

Expected drawing:

q0: ─H──●─  
        │  
q1: ────X─

### circuits/ghz3.qc

# GHZ3 circuit: (|000> + |111>) / sqrt(2)

H 0  
CX 0 1  
CX 1 2

Run in the REPL:

LOAD circuits/ghz3.qc  
DRAW  
RUNCIRCUIT  
PRINT  
EXPECT ZZ 0 1  
EXPECT ZZ 1 2  
EXPECT XXX 0 1 2

Run from the command line:

dotnet run -- --qubits 3 --circuit circuits/ghz3.qc --draw --print

Expected drawing:

q0: ─H──●────  
        │  
q1: ────X──●─  
           │  
q2: ───────X─

### circuits/toffoli.qc

# Toffoli circuit

X 0  
X 1  
CCX 0 1 2

Run from the command line:

dotnet run -- --qubits 3 --circuit circuits/toffoli.qc --draw --print

Expected result:

|111> : 1.0

## Testing

The project includes an NUnit test project.

Run all tests with:

dotnet test

The tests cover:

- Single-qubit gate behaviour
- Bell-state preparation
- GHZ-state preparation
- SWAP behaviour
- CCX / Toffoli behaviour
- Pauli expectation values
- Measurement collapse
- Snapshot and restore
- QuantumCircuit execution
- Circuit drawing

## Design Notes

- Uses System.Numerics.Complex for amplitudes
- Uses System.Security.Cryptography.RandomNumberGenerator for measurement randomness
- Uses a dense state-vector representation of size 2^n
- Qubit 0 is the least significant bit in the basis-state index
- Bit strings are printed most-significant bit to least-significant bit
- Measurement follows the Born rule
- Single-qubit measurement performs partial collapse and renormalizes the surviving branch
- Full measurement collapses the register to one computational-basis state
- Unitary gates do not renormalize after every operation
- Debug builds can assert that the state remains normalized
- Periodic normalization is used only as a numerical safety mechanism
- Expectation values are computed from Pauli observables, not just displayed probabilities
- RUN reuses the same command executor as the interactive REPL
- QuantumCircuit stores gate operations before execution
- RUNCIRCUIT executes the loaded QuantumCircuit against the state-vector simulator
- DRAW renders the loaded QuantumCircuit as a Unicode circuit diagram
- CLI mode allows scripts and circuits to be executed without entering the REPL

## Memory Model

The simulator uses a dense state vector with 2^n complex amplitudes.

Each amplitude is a System.Numerics.Complex value containing two double values:

Real      = 8 bytes  
Imaginary = 8 bytes  
Total     = 16 bytes per amplitude

Approximate state-vector memory usage:

20 qubits = 16 MiB  
21 qubits = 32 MiB  
22 qubits = 64 MiB  
23 qubits = 128 MiB  
24 qubits = 256 MiB  
25 qubits = 512 MiB  
26 qubits = 1 GiB  
27 qubits = 2 GiB  
28 qubits = 4 GiB

The default safety limit is 25 qubits.

This limit is intentional because dense state-vector simulators grow exponentially in memory and time.

## Current Architecture

The project is organized around the following core files:

Program.cs  
Quantum.cs  
QuantumRegister.cs  
PauliTerm.cs  
GateKind.cs  
GateOperation.cs  
QuantumCircuit.cs  
CircuitDrawer.cs

### Program.cs

Handles:

- Interactive REPL
- Command parsing
- Script execution using RUN
- Circuit loading using LOAD
- Circuit execution using RUNCIRCUIT
- Circuit drawing using DRAW
- Command-line argument parsing
- Command-line script and circuit execution
- Angle parsing
- Observable parsing
- User help text

### Quantum.cs

Provides the public gate API:

- Single-qubit gates
- Controlled gates
- Measurement wrappers
- Norm and memory diagnostics
- Sampling support
- Post-unitary gate handling

### QuantumRegister.cs

Owns the low-level state-vector mechanics:

- State storage
- Normalization
- Probabilities
- Measurement
- Partial collapse
- Controlled gate application
- SWAP and CZ implementation
- Pauli expectation evaluation
- Dense memory validation

### PauliTerm.cs

Represents one term in a Pauli observable:

public readonly record struct PauliTerm(char Pauli, int Qubit);

### GateKind.cs

Defines the supported gate operation types used by QuantumCircuit.

### GateOperation.cs

Represents a single circuit operation, including:

- Gate kind
- Target qubits
- Control qubits
- Optional rotation angle
- Application through the Quantum API
- Command-style formatting

### QuantumCircuit.cs

Represents a reusable gate-only quantum circuit.

It supports:

- Adding gate operations
- Validating qubit indices
- Executing the circuit
- Printing the operation list
- Drawing the circuit

### CircuitDrawer.cs

Renders a QuantumCircuit as a Unicode circuit diagram.

It supports:

- Single-qubit gates
- Controlled gates
- SWAP
- CCX / Toffoli
- Controlled rotations
- Connector rows between qubits

## Limitations

- Dense state-vector simulation only
- Exponential memory growth with qubit count
- No density matrix representation
- No noise or decoherence models
- No mixed states
- No Bloch sphere visualization
- No OpenQASM import/export yet
- No gate optimization or circuit transpilation
- Circuit loading currently supports unitary gate operations only
- Circuit drawing is text/Unicode based, not graphical
- No plain ASCII fallback drawing mode yet

## Possible Extensions

- OpenQASM import
- Circuit export
- Plain ASCII drawing fallback
- Circuit diagram export to text or SVG
- Bloch sphere visualization for single-qubit states
- Density matrix simulator
- Noise channels
- Decoherence models
- Sparse-state simulation
- Tensor-network simulation
- Additional gates: CY, CH, CS, CT, phase gates, arbitrary unitary gates
- Controlled arbitrary single-qubit gates exposed at command level
- Named state-preparation commands such as BELL and GHZ
- More command-line execution options
- More formal circuit validation
- Circuit optimization passes

## Educational Purpose

This project is a learning tool for linear algebra, quantum mechanics, and quantum computing fundamentals.

The emphasis is on conceptual correctness, readable implementation, and a clear path from simple single-qubit behaviour to entanglement, circuit representation, command-line execution, testing, and multi-qubit circuit simulation.

## License

MIT