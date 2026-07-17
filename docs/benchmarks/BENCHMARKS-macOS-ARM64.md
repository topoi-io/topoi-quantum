# Topoi.Quantum Benchmarks

## Scope

Topoi.Quantum is a local dense state-vector simulator intended for learning,
experimentation, OpenQASM workflows, and small-to-medium circuit prototyping.
It is not positioned as a GPU, distributed, tensor-network, or cloud-hardware backend.

## Environment

OS:
CPU:
Architecture:
RAM:
.NET SDK:
.NET Runtime:
BenchmarkDotNet:
Commit:

## Summary

- Recommended comfortable range: up to 22 qubits
- Practical high-end local test: 24 qubits
- Default safety limit: 25 qubits
- Hot gate operations allocate 0 B after setup

## State-vector scaling

| Method | Qubits | Mean | Error | StdDev | Allocated |
|---|---:|---:|---:|---:|---:|

## Gate benchmarks

| Method | Qubits | Mean | Allocated |
|---|---:|---:|---:|

## Circuit benchmarks

| Circuit | Qubits | Mean | Allocated |
|---|---:|---:|---:|

## OpenQASM benchmarks

| Method | Qubits | Mean | Allocated |
|---|---:|---:|---:|

## Measurement and sampling

| Method | Qubits/Shots | Mean | Allocated |
|---|---:|---:|---:|

## Notes

These benchmarks are intended to detect regressions and communicate practical
limits, not to claim parity with production quantum simulators.