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
}
