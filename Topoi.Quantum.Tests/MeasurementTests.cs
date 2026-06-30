using NUnit.Framework;

namespace Topoi.Quantum.Tests;

[TestFixture]
public sealed class MeasurementTests
{
    [Test]
    public void MeasureAll_CollapsesZeroStateToZero()
    {
        Quantum.Init(2);
        Quantum.Reset();

        int outcome = Quantum.MeasureAll();

        Assert.That(outcome, Is.EqualTo(0));
        TestHelpers.AssertProbability(0, 1.0);
    }

    [Test]
    public void MeasureAll_CollapsesKnownBasisState()
    {
        Quantum.Init(3);
        Quantum.Reset();

        Quantum.X(0);
        Quantum.X(2);

        int expected = TestHelpers.Basis(0, 2);
        int outcome = Quantum.MeasureAll();

        Assert.That(outcome, Is.EqualTo(expected));
        TestHelpers.AssertProbability(expected, 1.0);
    }

    [Test]
    public void MeasureQubit_OnKnownZero_ReturnsZero()
    {
        Quantum.Init(2);
        Quantum.Reset();

        int bit = Quantum.Measure(0);

        Assert.That(bit, Is.EqualTo(0));
        TestHelpers.AssertProbability(0, 1.0);
    }

    [Test]
    public void MeasureQubit_OnKnownOne_ReturnsOne()
    {
        Quantum.Init(2);
        Quantum.Reset();

        Quantum.X(1);

        int bit = Quantum.Measure(1);

        Assert.That(bit, Is.EqualTo(1));
        TestHelpers.AssertProbability(TestHelpers.Basis(1), 1.0);
    }

    [Test]
    public void SnapshotAndRestore_PreservesPreparedState()
    {
        Quantum.Init(2);
        Quantum.Reset();

        Quantum.H(0);
        Quantum.CX(0, 1);

        var snapshot = Quantum.SnapshotState();

        Quantum.MeasureAll();
        Quantum.RestoreState(snapshot);

        TestHelpers.AssertProbability(TestHelpers.Basis(), 0.5);
        TestHelpers.AssertProbability(TestHelpers.Basis(0, 1), 0.5);
    }
}