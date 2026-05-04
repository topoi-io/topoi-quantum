using NUnit.Framework;

namespace QuantumComputer.Tests;

[TestFixture]
public sealed class ExpectationValueTests
{
    [SetUp]
    public void SetUp()
    {
        Quantum.Init(1);
        Quantum.Reset();
    }

    [Test]
    public void ZeroState_HasZExpectationPlusOne()
    {
        TestHelpers.AssertExpectation(1.0, new PauliTerm('Z', 0));
    }

    [Test]
    public void OneState_HasZExpectationMinusOne()
    {
        Quantum.X(0);

        TestHelpers.AssertExpectation(-1.0, new PauliTerm('Z', 0));
    }

    [Test]
    public void PlusState_HasXExpectationPlusOne()
    {
        Quantum.H(0);

        TestHelpers.AssertExpectation(1.0, new PauliTerm('X', 0));
    }

    [Test]
    public void PlusState_HasZExpectationZero()
    {
        Quantum.H(0);

        TestHelpers.AssertExpectation(0.0, new PauliTerm('Z', 0));
    }

    [Test]
    public void YEigenstate_HasYExpectationPlusOne()
    {
        Quantum.H(0);
        Quantum.S(0);

        TestHelpers.AssertExpectation(1.0, new PauliTerm('Y', 0));
    }
}