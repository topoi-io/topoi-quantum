using NUnit.Framework;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class ExpectationValueTests
{
    QuantumSimulator _simulator;

    [SetUp]
    public void SetUp()
    {
        _simulator = new QuantumSimulator(1);
    }

    [Test]
    public void ZeroState_HasZExpectationPlusOne()
    {
        TestHelpers.AssertExpectation(_simulator, 1.0, new PauliTerm('Z', 0));
    }

    [Test]
    public void OneState_HasZExpectationMinusOne()
    {
        _simulator.X(0);

        TestHelpers.AssertExpectation(_simulator, -1.0, new PauliTerm('Z', 0));
    }

    [Test]
    public void PlusState_HasXExpectationPlusOne()
    {
        _simulator.H(0);

        TestHelpers.AssertExpectation(_simulator, 1.0, new PauliTerm('X', 0));
    }

    [Test]
    public void PlusState_HasZExpectationZero()
    {
        _simulator.H(0);

        TestHelpers.AssertExpectation(_simulator, 0.0, new PauliTerm('Z', 0));
    }

    [Test]
    public void YEigenstate_HasYExpectationPlusOne()
    {
        _simulator.H(0);
        _simulator.S(0);

        TestHelpers.AssertExpectation(_simulator, 1.0, new PauliTerm('Y', 0));
    }
}