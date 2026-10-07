# Contributing to Topoi Quantum

Thank you for your interest in contributing to **Topoi Quantum**.

Topoi Quantum is an open-source .NET quantum-computing toolkit for building circuits, running local state-vector simulations, working with OpenQASM 3, sampling measurements, estimating observables, and experimenting with quantum algorithms.

Contributions from developers, students, researchers, educators, and quantum-computing enthusiasts are welcome.

## Ways to contribute

You can contribute by:

- reporting reproducible bugs
- suggesting improvements
- contributing code
- improving documentation
- adding examples
- improving tests
- helping with OpenQASM support
- contributing quantum gates or simulator improvements
- answering questions in GitHub Discussions
- sharing projects and experiments built with Topoi Quantum

## Questions and community discussions

If you have a question about using Topoi Quantum, want to discuss an idea, or want to share something you have built, please use **GitHub Discussions**.

Discussions are the best place for:

- usage questions
- quantum-computing questions
- feature ideas that still need discussion
- design discussions
- project showcases
- general community conversation

For reproducible software bugs, please use **GitHub Issues** instead.

## Reporting bugs

Before opening an issue, please check whether the problem has already been reported.

When reporting a bug, include as much relevant information as possible, including:

- Topoi Quantum package or tool version
- .NET SDK version
- operating system
- package or component affected
- steps required to reproduce the problem
- expected behaviour
- actual behaviour
- a minimal code sample or OpenQASM example where possible
- relevant exception messages or console output

Please keep bug reports focused on a single reproducible problem.

## Suggesting features

Early-stage ideas and proposals should normally begin in the **Ideas** category in GitHub Discussions.

This gives the community an opportunity to discuss the problem, possible approaches, API design, compatibility, and scope before implementation work begins.

Once an idea is sufficiently defined, it may be tracked as a GitHub Issue.

## Development requirements

To build Topoi Quantum from source you will need:

- .NET 10 SDK
- Git
- Windows, macOS, or Linux

Clone the repository:

```bash
git clone https://github.com/topoi-io/topoi-quantum.git
cd topoi-quantum
```

Restore dependencies:

```bash
dotnet restore Topoi.Quantum.slnx
```

Build the solution:

```bash
dotnet build Topoi.Quantum.slnx --configuration Release --no-restore
```

Run the test suite:

```bash
dotnet test Topoi.Quantum.slnx --configuration Release --no-build
```

Please ensure the solution builds successfully and all tests pass before submitting a pull request.

## Project structure

The repository contains several packages and supporting projects, including:

- `Topoi.Quantum` — core simulator, circuits, gates, sampling, estimation, and randomness
- `Topoi.Quantum.OpenQasm` — OpenQASM parsing, execution, conversion, and export
- `Topoi.Quantum.Drawing` — text and Unicode circuit drawing
- `Topoi.Quantum.Parsing` — native circuit, angle, gate, and observable parsing
- `Topoi.Quantum.Cli` — command-line and interactive-shell functionality
- `Topoi.Quantum.Tool` — installable .NET tool exposing the `tq` command
- `Topoi.Quantum.Tests` — automated regression tests
- `Topoi.Quantum.Benchmarks` — BenchmarkDotNet benchmarks

Please keep changes within the appropriate project and avoid introducing unnecessary dependencies between packages.

## Coding guidelines

Contributions should prioritise:

- correctness
- clear and maintainable code
- predictable public APIs
- appropriate validation
- useful exception messages
- testability
- compatibility with the existing package structure
- separation between the core SDK and CLI or presentation concerns

Avoid unrelated refactoring in the same pull request as a bug fix or feature.

Keep changes focused so they can be reviewed and tested independently.

## Tests

Bug fixes should include a regression test whenever practical.

New functionality should include tests covering:

- expected behaviour
- relevant validation
- edge cases where appropriate
- previously unsupported behaviour being introduced

Changes affecting OpenQASM should include representative OpenQASM test cases.

Changes to gates or simulator behaviour should verify the resulting quantum state, probabilities, measurements, or observable values as appropriate.

All existing tests must continue to pass.

## Adding or changing quantum functionality

Changes to core quantum behaviour should be made carefully.

When adding or modifying gates, simulation behaviour, measurement, sampling, observables, or OpenQASM execution:

- verify the mathematical behaviour
- preserve qubit ordering conventions used by the existing toolkit
- include automated tests
- document user-visible behaviour
- consider whether the change affects package APIs or compatibility

Breaking API changes should be discussed before implementation.

## Documentation

Documentation contributions are welcome.

Please update documentation when a change affects:

- public APIs
- CLI options
- OpenQASM support
- package installation
- expected behaviour
- limitations
- examples

The Topoi Quantum Developer Hub is available at:

https://topoi-io.github.io/quantum/

## Pull requests

Before opening a pull request:

1. Build the solution successfully.
2. Run the relevant tests.
3. Add or update tests where appropriate.
4. Update documentation when required.
5. Keep the change focused on one clear purpose.

A pull request should explain:

- what changed
- why the change is needed
- how the change was tested
- whether it changes any public API or existing behaviour

Small, focused pull requests are generally easier to review than large changes containing multiple unrelated modifications.

## Security issues

Please do **not** report security vulnerabilities through public GitHub Issues or Discussions.

Follow the private reporting instructions in [`SECURITY.md`](SECURITY.md).

## Code of Conduct

Participation in the Topoi Quantum community is expected to follow the project's Code of Conduct.

## Licence

By contributing to Topoi Quantum, you agree that your contributions will be licensed under the project's [MIT License](LICENSE).

Thank you for helping improve Topoi Quantum.
