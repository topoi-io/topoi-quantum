# N-Qubit Quantum Gate Interpreter (C# / .NET 10)

A minimal N-qubit quantum computer simulator written in C# (.NET 10).

It simulates unitary quantum gates, entanglement, probabilistic measurement, sampling, Pauli expectation values, and script execution using a dense state-vector model.

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
- Comment support in script files using #

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

- RUN path – Run commands from a script file

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

### Program.cs

Handles:

- Interactive REPL
- Command parsing
- Script execution using RUN
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

## Limitations

- Dense state-vector simulation only
- Exponential memory growth with qubit count
- No density matrix representation
- No noise or decoherence models
- No mixed states
- No Bloch sphere visualization
- No circuit object model yet
- No formal test project yet
- No OpenQASM import/export yet
- No gate optimization or circuit transpilation

## Possible Extensions

- Formal QuantumCircuit class
- xUnit or NUnit test project
- More example scripts
- OpenQASM import
- Circuit export
- Circuit diagram output
- Bloch sphere visualization for single-qubit states
- Density matrix simulator
- Noise channels
- Decoherence models
- Sparse-state simulation
- Tensor-network simulation
- Additional gates: CY, CH, CS, CT, phase gates, arbitrary unitary gates
- Controlled arbitrary single-qubit gates exposed at command level
- Named state-preparation commands such as BELL and GHZ
- Command-line script execution without entering the REPL

## Educational Purpose

This project is a learning tool for linear algebra, quantum mechanics, and quantum computing fundamentals.

The emphasis is on conceptual correctness, readable implementation, and a clear path from simple single-qubit behaviour to entanglement and multi-qubit circuit simulation.

## License

MIT