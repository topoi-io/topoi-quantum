using NUnit.Framework;
using System.Numerics;

namespace Topoi.Quantum.Tests;

internal static class TestHelpers
{
    public const double Tolerance = 1e-10;

    public static void AssertProbability(
        QuantumSimulator simulator,
        int basisIndex,
        double expected,
        double tolerance = Tolerance)
    {
        ArgumentNullException.ThrowIfNull(simulator);

        double[] probabilities = simulator.Register.Probabilities();

        Assert.That(probabilities[basisIndex], Is.EqualTo(expected).Within(tolerance));
    }

    public static void AssertAmplitude(
        QuantumSimulator simulator,
        int basisIndex,
        Complex expected,
        double tolerance = Tolerance)
    {
        ArgumentNullException.ThrowIfNull(simulator);

        Complex actual = simulator.Register.State[basisIndex];

        Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(tolerance));
        Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(tolerance));
    }

    public static void AssertExpectation(
        QuantumSimulator simulator,
        double expected,
        params PauliTerm[] terms)
    {
        ArgumentNullException.ThrowIfNull(simulator);

        Complex actual = simulator.ExpectPauliString(terms);

        Assert.That(actual.Real, Is.EqualTo(expected).Within(Tolerance));
        Assert.That(actual.Imaginary, Is.EqualTo(0.0).Within(Tolerance));
    }

    public static int Basis(params int[] oneQubits)
    {
        int basis = 0;

        foreach (int q in oneQubits)
            basis |= 1 << q;

        return basis;
    }
}