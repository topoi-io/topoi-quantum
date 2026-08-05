using NUnit.Framework;

using Topoi.Quantum.Primitives;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class SamplerTests
{
    [Test]
    public void Run_BellCircuit_ReturnsOnly00And11()
    {
        var circuit = new QuantumCircuit(2);

        circuit.Add(
            new GateOperation(
                GateKind.H,
                new[] { 0 }));

        circuit.Add(
            new GateOperation(
                GateKind.CX,
                new[] { 0, 1 }));

        var sampler =
            new Sampler(
                new SeededRandomSource(123));

        SamplerResult result =
            sampler.Run(
                circuit,
                shots: 1000);

        Assert.That(
            result.QubitCount,
            Is.EqualTo(2));

        Assert.That(
            result.Shots,
            Is.EqualTo(1000));

        Assert.That(
            result.CountFor("01"),
            Is.Zero);

        Assert.That(
            result.CountFor("10"),
            Is.Zero);

        Assert.That(
            result.CountFor("00") +
            result.CountFor("11"),
            Is.EqualTo(1000));

        /*
         * Sparse result dictionaries must not contain
         * impossible zero-count outcomes.
         */
        Assert.That(
            result.Counts.ContainsKey("01"),
            Is.False);

        Assert.That(
            result.Counts.ContainsKey("10"),
            Is.False);

        Assert.That(
            result.Probabilities.ContainsKey("01"),
            Is.False);

        Assert.That(
            result.Probabilities.ContainsKey("10"),
            Is.False);

        Assert.That(
            result.Counts.Count,
            Is.LessThanOrEqualTo(2));

        Assert.That(
            result.Probabilities.Count,
            Is.EqualTo(result.Counts.Count));
    }

    [Test]
    public void Run_OneQubitHadamard_ReturnsExpectedBitStrings()
    {
        var circuit = new QuantumCircuit(1);

        circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));

        var sampler = new Sampler(new SeededRandomSource(456));

        SamplerResult result = sampler.Run(circuit, shots: 1000);

        Assert.That(result.Counts.Keys, Does.Contain("0"));
        Assert.That(result.Counts.Keys, Does.Contain("1"));
        Assert.That(result.CountFor("0") + result.CountFor("1"), Is.EqualTo(1000));
    }

    [Test]
    public void Run_WithZeroShots_Throws()
    {
        var circuit = new QuantumCircuit(1);
        var sampler = new Sampler();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            sampler.Run(circuit, shots: 0));
    }

    [Test]
    public void Run_ManyQubitsWithFewShots_StoresAtMostOneEntryPerShot()
    {
        const int qubitCount = 16;
        const int shots = 3;

        var circuit =
            new QuantumCircuit(qubitCount);

        /*
         * Put every qubit into superposition so the sampler
         * has many possible outcomes.
         */
        for (int qubit = 0;
             qubit < qubitCount;
             qubit++)
        {
            circuit.Add(
                new GateOperation(
                    GateKind.H,
                    new[] { qubit }));
        }

        var sampler =
            new Sampler(
                new SeededRandomSource(123));

        SamplerResult result =
            sampler.Run(
                circuit,
                shots);

        /*
         * Only three measurements were taken, so there cannot
         * be more than three stored outcomes.
         */
        Assert.That(
            result.Counts.Count,
            Is.LessThanOrEqualTo(shots));

        Assert.That(
            result.Probabilities.Count,
            Is.EqualTo(result.Counts.Count));

        /*
         * The stored counts must account for every shot.
         */
        Assert.That(
            result.Counts.Values.Sum(),
            Is.EqualTo(shots));

        /*
         * Sparse dictionaries must never store zero-count entries.
         */
        Assert.That(
            result.Counts.Values.All(
                count => count > 0),
            Is.True);

        Assert.That(
            result.Probabilities.Values.All(
                probability => probability > 0.0),
            Is.True);

        /*
         * The observed probabilities must sum to one.
         */
        Assert.That(
            result.Probabilities.Values.Sum(),
            Is.EqualTo(1.0).Within(1e-12));
    }

    [Test]
    public void Run_ZeroState_StoresOnlyTheZeroOutcome()
    {
        const int qubitCount = 16;
        const int shots = 100;

        var circuit =
            new QuantumCircuit(qubitCount);

        var sampler =
            new Sampler(
                new SeededRandomSource(456));

        SamplerResult result =
            sampler.Run(
                circuit,
                shots);

        string zeroState =
            new('0', qubitCount);

        string unobservedState =
            new string('0', qubitCount - 1) + "1";

        Assert.That(
            result.Counts.Count,
            Is.EqualTo(1));

        Assert.That(
            result.Probabilities.Count,
            Is.EqualTo(1));

        Assert.That(
            result.CountFor(zeroState),
            Is.EqualTo(shots));

        Assert.That(
            result.ProbabilityFor(zeroState),
            Is.EqualTo(1.0));

        /*
         * Lookup methods remain backwards-compatible:
         * absent outcomes return zero.
         */
        Assert.That(
            result.CountFor(unobservedState),
            Is.Zero);

        Assert.That(
            result.ProbabilityFor(unobservedState),
            Is.Zero);

        /*
         * But zero-count outcomes are not physically stored.
         */
        Assert.That(
            result.Counts.ContainsKey(unobservedState),
            Is.False);

        Assert.That(
            result.Probabilities.ContainsKey(unobservedState),
            Is.False);
    }
}