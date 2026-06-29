using NUnit.Framework;
using QuantumComputer.Core;
using QuantumComputer.Core.Primitives;

namespace QuantumComputer.Tests;

[TestFixture]
public sealed class SamplerTests
{
    [Test]
    public void Run_BellCircuit_ReturnsOnly00And11()
    {
        var circuit = new QuantumCircuit(2);

        circuit.Add(new GateOperation(GateKind.H, new[] { 0 }));
        circuit.Add(new GateOperation(GateKind.CX, new[] { 0, 1 }));

        var sampler = new Sampler(new SeededRandomSource(123));

        SamplerResult result = sampler.Run(circuit, shots: 1000);

        Assert.That(result.QubitCount, Is.EqualTo(2));
        Assert.That(result.Shots, Is.EqualTo(1000));

        Assert.That(result.CountFor("01"), Is.EqualTo(0));
        Assert.That(result.CountFor("10"), Is.EqualTo(0));

        Assert.That(result.CountFor("00") + result.CountFor("11"), Is.EqualTo(1000));
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
}