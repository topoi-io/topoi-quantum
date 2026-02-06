# Single-Qubit Quantum Gate Interpreter (C# / .NET 10)

A minimal single-qubit quantum computer simulator written in C# (.NET 10).

It simulates unitary quantum gates, probabilistic measurement, sampling, and expectation values using a state-vector model.

This project is intentionally small and educational, focusing on clarity and correctness rather than performance.

## Features

- Single-qubit state vector simulation
- Standard quantum gates: X, Y, Z, H, S, T
- Continuous rotation gates: RX, RY, RZ
- Cryptographically strong randomness for measurement
- Measurement with state collapse
- Repeated sampling using SAMPLE
- Expectation values using EXPECT

## Mathematical Model

The qubit state is represented as:

|psi> = alpha|0> + beta|1>

with normalization:

|alpha|^2 + |beta|^2 = 1

Quantum gates are 2x2 unitary matrices applied via:

|psi'> = U |psi>

Measurement follows the Born rule:

P(0) = |alpha|^2  
P(1) = |beta|^2

## Requirements

- .NET 10 (Preview or later)
- Windows, macOS, or Linux

## Build and Run

dotnet build  
dotnet run

## Commands

### State Control

- RESET – Reset qubit to |0>
- PRINT – Print current state and probabilities
- EXPECT – Show theoretical probabilities without measurement

### Quantum Gates

- X – Pauli-X (bit flip)
- Y – Pauli-Y
- Z – Pauli-Z (phase flip)
- H – Hadamard (creates superposition)
- S – Phase gate
- T – Pi over 8 gate

### Rotation Gates

- RX theta – Rotation about X axis
- RY theta – Rotation about Y axis
- RZ theta – Rotation about Z axis

Angles are in radians.

Accepted formats:

pi  
pi/2  
3*pi/4  
-0.25*pi  
1.57079632679

### Measurement and Sampling

- MEASURE – Measure once and collapse the state
- SAMPLE n – Measure n times without collapsing the prepared state

### Utility

- HELP – Show available commands
- QUIT or EXIT – Exit the interpreter

## Example Usage

Create a superposition:

RESET  
H  
EXPECT

Sample statistics:

RESET  
H  
SAMPLE 10000

Biased rotation:

RESET  
RY pi/3  
EXPECT  
SAMPLE 2000

## Design Notes

- Uses System.Numerics.Complex for amplitudes
- Uses System.Security.Cryptography.RandomNumberGenerator for unbiased randomness
- Explicit renormalization avoids floating-point drift
- Clean separation between unitary evolution, expectation, and measurement

## Limitations

- Single qubit only
- No noise or decoherence
- No entanglement or tensor products

## Possible Extensions

- Two-qubit support (CNOT, Bell states)
- Density matrix representation
- Noise models
- Script or batch execution
- Bloch sphere visualization

## Educational Purpose

This project is a learning tool for linear algebra, quantum mechanics, and quantum computing fundamentals.

The emphasis is on conceptual correctness.

## License

MIT
