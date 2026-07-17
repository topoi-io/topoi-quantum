# Topoi.Quantum Benchmarks

These benchmarks were taken before the v0.1.0 MVP release.

## Summary

Topoi.Quantum is a dense state-vector simulator intended for local learning,
experimentation, OpenQASM workflows, sampling, estimation, and small-to-medium
quantum circuit prototyping.

It is not positioned as a GPU, distributed, tensor-network, or cloud-hardware simulator.

## Platforms

| Platform | CPU | Architecture | .NET | Report |
|---|---|---:|---|---|
| Windows | Snapdragon X / etc. | ARM64 | .NET 10 | docs/benchmarks/BENCHMARKS-Windows-ARM64.md |
| macOS | Apple Silicon / Intel | ARM64/x64 | .NET 10 | docs/benchmarks/BENCHMARKS-macOS-ARM64.md |

## Practical MVP guidance

- Comfortable range: up to 20–22 qubits
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