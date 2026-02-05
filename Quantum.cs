using System.Numerics;
using System.Security.Cryptography;

namespace QuantumComputer;

static class Quantum
{
    // State |psi> = alpha|0> + beta|1>
    public static Complex Alpha { get; private set; } = Complex.One; // start in |0>
    public static Complex Beta { get; private set; } = Complex.Zero;

    //private static readonly Random Rng = new();

    public static void Reset()
    {
        Alpha = Complex.One;
        Beta = Complex.Zero;
    }

    public static void Apply(Complex m00, Complex m01, Complex m10, Complex m11)
    {
        // |psi'> = M |psi>
        var a = m00 * Alpha + m01 * Beta;
        var b = m10 * Alpha + m11 * Beta;

        Alpha = a;
        Beta = b;

        // Small numerical drift can occur; renormalize gently.
        Normalize();
    }

    public static void Normalize()
    {
        double norm = Math.Sqrt(Alpha.Magnitude * Alpha.Magnitude + Beta.Magnitude * Beta.Magnitude);
        if (norm == 0) throw new InvalidOperationException("State norm is zero (should not happen for unitary gates).");
        Alpha /= norm;
        Beta /= norm;
    }

    public static (double p0, double p1) Probabilities()
    {
        double p0 = Alpha.Magnitude * Alpha.Magnitude;
        double p1 = Beta.Magnitude * Beta.Magnitude;
        // Numerical rounding: force sum to 1-ish
        double s = p0 + p1;
        if (s != 0) { p0 /= s; p1 /= s; }
        return (p0, p1);
    }

    public static int Measure()
    {
        var (p0, p1) = Probabilities();
        double r = NextUnitDouble();
        int outcome = (r < p0) ? 0 : 1;

        // Collapse state
        if (outcome == 0)
        {
            Alpha = Complex.One;
            Beta = Complex.Zero;
        }
        else
        {
            Alpha = Complex.Zero;
            Beta = Complex.One;
        }

        return outcome;
    }

    // Common 1-qubit gates (matrices are in computational basis |0>,|1>)
    public static void X() => Apply(0, 1, 1, 0);

    public static void Y() => Apply(0, -Complex.ImaginaryOne,
                                    Complex.ImaginaryOne, 0);

    public static void Z() => Apply(1, 0, 0, -1);

    public static void H()
    {
        double invSqrt2 = 1.0 / Math.Sqrt(2.0);
        Apply(invSqrt2, invSqrt2,
              invSqrt2, -invSqrt2);
    }

    public static void S() => Apply(1, 0, 0, Complex.ImaginaryOne); // diag(1, i)

    public static void T()
    {
        // diag(1, e^{i*pi/4})
        Complex phase = Complex.Exp(Complex.ImaginaryOne * (Math.PI / 4.0));
        Apply(1, 0, 0, phase);
    }

    // Rotations: Rn(theta) = exp(-i theta sigma_n / 2)
    public static void RX(double theta)
    {
        // Rx = cos(t/2) I - i sin(t/2) X
        double c = Math.Cos(theta / 2.0);
        double s = Math.Sin(theta / 2.0);
        Complex minusIS = -Complex.ImaginaryOne * s;

        // [[c, -i s], [-i s, c]]
        Apply(c, minusIS,
              minusIS, c);
    }

    public static void RY(double theta)
    {
        // Ry = cos(t/2) I - i sin(t/2) Y
        // Equivalent matrix: [[c, -s], [s, c]]
        double c = Math.Cos(theta / 2.0);
        double s = Math.Sin(theta / 2.0);

        Apply(c, -s,
              s, c);
    }

    public static void RZ(double theta)
    {
        // Rz = exp(-i theta Z/2) = diag(e^{-i t/2}, e^{+i t/2})
        Complex p0 = Complex.Exp(-Complex.ImaginaryOne * (theta / 2.0));
        Complex p1 = Complex.Exp(Complex.ImaginaryOne * (theta / 2.0));
        Apply(p0, 0, 0, p1);
    }

    public static void PrintState()
    {
        var (p0, p1) = Probabilities();
        Console.WriteLine($"|ψ⟩ = α|0⟩ + β|1⟩");
        Console.WriteLine($"α = {Fmt(Alpha)}");
        Console.WriteLine($"β = {Fmt(Beta)}");
        Console.WriteLine($"P(0) = {p0:F6}, P(1) = {p1:F6}");
    }

    public static void SetState(Complex alpha, Complex beta)
    {
        Alpha = alpha;
        Beta = beta;
        Normalize();
    }

    public static void Expect()
    {
        var (p0, p1) = Probabilities();
        Console.WriteLine("Expectation (no collapse):");
        Console.WriteLine($"⟨0⟩ probability = {p0:F6}");
        Console.WriteLine($"⟨1⟩ probability = {p1:F6}");
    }

    private static string Fmt(Complex z)
    {
        // Pretty-print a+bi with a bit of rounding
        double a = z.Real;
        double b = z.Imaginary;
        return $"{a:+0.######;-0.######;+0.######} {b:+0.######;-0.######;+0.######}i";
    }

    private static double NextUnitDouble()
    {
        // Uniform in [0,1)
        // RandomNumberGenerator.GetInt32 is crypto-strong and avoids seeding issues.
        int x = RandomNumberGenerator.GetInt32(int.MaxValue); // 0..int.MaxValue-1
        return x / (double)int.MaxValue;
    }
}
