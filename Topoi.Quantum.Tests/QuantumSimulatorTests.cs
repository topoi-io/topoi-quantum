using NUnit.Framework;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class QuantumSimulatorTests
{
    [Test]
    public void Simulator_Instances_CanRunIndependently()
    {
        var simulator1 = new QuantumSimulator(1);
        var simulator2 = new QuantumSimulator(2);

        simulator2.H(0);
        simulator2.CX(0, 1);

        double[] probabilities = simulator2.Register.Probabilities();

        Assert.That(probabilities[TestHelpers.Basis()], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
        Assert.That(probabilities[TestHelpers.Basis(0, 1)], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));

        TestHelpers.AssertProbability(simulator1, 0, 1.0);
        TestHelpers.AssertProbability(simulator1, 1, 0.0);
    }

    [Test]
    public void SeededSimulator_ProducesRepeatableMeasurements()
    {
        var sim1 = new QuantumSimulator(1, new SeededRandomSource(123));
        var sim2 = new QuantumSimulator(1, new SeededRandomSource(123));

        sim1.H(0);
        sim2.H(0);

        int m1 = sim1.MeasureAll();
        int m2 = sim2.MeasureAll();

        Assert.That(m1, Is.EqualTo(m2));
    }
}
