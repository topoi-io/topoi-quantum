using NUnit.Framework;

namespace QuantumComputer.Tests;

[TestFixture]
public sealed class QuantumSimulatorTests
{
    [Test]
    public void Simulator_Instance_CanRunIndependentlyOfStaticQuantumFacade()
    {
        Quantum.Init(1);
        Quantum.Reset();

        var simulator = new QuantumSimulator(2);

        simulator.H(0);
        simulator.CX(0, 1);

        double[] probabilities = simulator.Register.Probabilities();

        Assert.That(probabilities[TestHelpers.Basis()], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));
        Assert.That(probabilities[TestHelpers.Basis(0, 1)], Is.EqualTo(0.5).Within(TestHelpers.Tolerance));

        TestHelpers.AssertProbability(0, 1.0); // static Quantum still has its own 1-qubit zero state
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
