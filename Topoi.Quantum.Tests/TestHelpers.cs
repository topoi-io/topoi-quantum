using NUnit.Framework;
using System.Numerics;

namespace Topoi.Quantum.Tests;

internal static class TestHelpers
{
    public const double Tolerance = 1e-10;

    public static void AssertProbability(int basisIndex, double expected, double tolerance = Tolerance)
    {
        double[] probabilities = Quantum.Register.Probabilities();
        Assert.That(probabilities[basisIndex], Is.EqualTo(expected).Within(tolerance));
    }

    public static void AssertAmplitude(int basisIndex, Complex expected, double tolerance = Tolerance)
    {
        Complex actual = Quantum.Register.State[basisIndex];

        Assert.That(actual.Real, Is.EqualTo(expected.Real).Within(tolerance));
        Assert.That(actual.Imaginary, Is.EqualTo(expected.Imaginary).Within(tolerance));
    }

    public static void AssertExpectation(double expected, params PauliTerm[] terms)
    {
        Complex actual = Quantum.ExpectPauliString(terms);

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