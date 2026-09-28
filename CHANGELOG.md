# Changelog

All notable changes to Topoi Quantum are documented in this file.

The format is based on Keep a Changelog, and the project follows Semantic Versioning.

## [Unreleased]

## [0.1.0] - 2026-09-29

Initial public MVP release of Topoi Quantum.

### Packages

* `Topoi.Quantum` NuGet package.
* `Topoi.Quantum.OpenQasm` NuGet package.
* `Topoi.Quantum.Parsing` NuGet package.
* `Topoi.Quantum.Drawing` NuGet package.
* `Topoi.Quantum.Cli` NuGet package.
* `Topoi.Quantum.Tool` .NET global tool using the `tq` command.
* Symbol packages using the `.snupkg` format.
* Repository and source information in package metadata.

### Core SDK

* Dense pure-state vector simulation.
* Fluent quantum-circuit construction.
* Cryptographically secure randomness by default.
* Seeded randomness for deterministic tests and experiments.
* Full-register measurement.
* Individual-qubit measurement and partial state collapse.
* Circuit sampling with sparse observed-outcome storage.
* Pauli expectation-value estimation.
* State snapshots and restoration.
* State normalization and diagnostic support.
* A default dense-state safety limit of 25 qubits.

### Gates

* Identity gate.
* Pauli X, Y and Z gates.
* Hadamard gate.
* S, S-dagger, T and T-dagger gates.
* Square-root X and inverse square-root X gates.
* RX, RY and RZ rotation gates.
* CX, CY, CZ and CH controlled gates.
* Controlled phase gate.
* Controlled rotation gates.
* SWAP gate.
* CCX/Toffoli gate.

### OpenQASM

* Practical OpenQASM 3.x parsing support.
* Standard gate mapping.
* Parameterized gates and angle expressions.
* Custom gate declarations.
* Nested and parameterized custom-gate expansion.
* Recursive custom-gate detection.
* Measurement operations and classical-bit storage.
* Reset operations.
* Barrier operations.
* Gate-only and executable OpenQASM models.
* OpenQASM circuit export.
* Supported `ctrl @` and `inv @` gate modifiers.

### Command-Line Tool

* Interactive quantum-simulator shell.
* Native `.qc` script execution.
* OpenQASM file execution.
* Circuit printing and Unicode circuit drawing.
* State and probability output.
* Pauli expectation calculations.
* Repeated sampling.
* Quantum random-bit generation.
* Installable `tq` command through the .NET tool system.

### Testing and Performance

* NUnit regression test suite covering gates, circuits, measurements, sampling, estimators, drawing and OpenQASM.
* Package-consumer tests for `Topoi.Quantum`.
* Package-consumer tests for `Topoi.Quantum.OpenQasm`.
* Isolated installation testing for `Topoi.Quantum.Tool`.
* BenchmarkDotNet performance project.
* Benchmark results for Windows ARM64 and macOS ARM64.
* GitHub Actions CI for restore, Release build and automated tests.

### Known Limitations

* OpenQASM support is a practical subset rather than full OpenQASM 3 compliance.
* The simulator uses dense state vectors and therefore scales exponentially with qubit count.
* Noise and decoherence models are not currently implemented.
* Density-matrix simulation is not currently implemented.
* Quantum hardware and cloud-provider backends are not currently implemented.
* GPU, distributed and tensor-network simulation are not currently implemented.
* SVG and PNG circuit rendering are not currently implemented.
* OpenQASM `pow` and `negctrl` modifiers are not currently executable.

[Unreleased]: https://github.com/topoi-io/topoi-quantum/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/topoi-io/topoi-quantum/releases/tag/v0.1.0