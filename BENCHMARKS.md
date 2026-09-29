# Topoi.Quantum Benchmarks

These benchmarks were recorded during development of the `v0.1.0` MVP release.

## Summary

Topoi.Quantum is a dense state-vector simulator intended for local learning,
experimentation, OpenQASM workflows, sampling, estimation, and small-to-medium
quantum circuit prototyping.

It is not positioned as a GPU, distributed, tensor-network, or cloud-hardware simulator.

## Platforms

| Platform | CPU | Architecture | RAM | .NET SDK | Report |
|---|---|---:|---:|---:|---|
| Windows 11 | Snapdragon X 10-core X1P64100, 3.40 GHz | ARM64 | 16 GB | 10.0.301 | [Windows ARM64 benchmarks](docs/benchmarks/BENCHMARKS-Windows-ARM64.md) |
| macOS Sequoia 15.7.7 | Apple M2 Max, 12-core | ARM64 | 64 GB | 10.0.103 | [macOS ARM64 benchmarks](docs/benchmarks/BENCHMARKS-macOS-ARM64.md) |

## Practical MVP guidance

- Comfortable local range: approximately 20–22 qubits
- High-end local test range: 24 qubits
- Default dense-state safety limit: 25 qubits
- Hot gate operations should allocate 0 B after setup

## Release benchmark status

| Area | Status |
|---|---|
| Core gates | Passed |
| Controlled gates | Passed |
| Bell/GHZ/random circuits | Passed |
| OpenQASM parse/execute | Passed |
| Measurement/sampling/estimator | Passed |
| Windows report | Complete |
| macOS report | Complete |
