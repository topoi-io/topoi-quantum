using System.Numerics;

namespace QuantumComputer;

static class Quantum
{
    public static QuantumRegister Register { get; private set; } = new QuantumRegister(1);

    public static void Init(int n) => Register = new QuantumRegister(n);

    public static void Reset() => Register.Reset();

    // Apply a 2x2 matrix gate to target qubit t (0 = LSB)
    public static void Apply1(int t, Complex m00, Complex m01, Complex m10, Complex m11)
    {
        if ((uint)t >= (uint)Register.QubitCount)
            throw new ArgumentOutOfRangeException(nameof(t), $"Qubit index out of range. Must be 0..{Register.QubitCount - 1}");

        int stride = 1 << t;
        int step = stride << 1;
        var v = Register.State;

        for (int block = 0; block < v.Length; block += step)
        {
            for (int i = 0; i < stride; i++)
            {
                int i0 = block + i;       // bit t = 0
                int i1 = i0 + stride;     // bit t = 1

                Complex a = v[i0];
                Complex b = v[i1];

                v[i0] = m00 * a + m01 * b;
                v[i1] = m10 * a + m11 * b;
            }
        }

        // Gentle renorm for drift (optional but keeps behavior similar to your 1-qubit version)
        Register.Normalize();
    }

    // Common 1-qubit gates (require a target index)
    public static void X(int t) => Apply1(t, 0, 1, 1, 0);

    public static void Y(int t) => Apply1(t, 0, -Complex.ImaginaryOne,
                                           Complex.ImaginaryOne, 0);

    public static void Z(int t) => Apply1(t, 1, 0, 0, -1);

    public static void H(int t)
    {
        double invSqrt2 = 1.0 / Math.Sqrt(2.0);
        Apply1(t, invSqrt2, invSqrt2,
                  invSqrt2, -invSqrt2);
    }

    public static void S(int t) => Apply1(t, 1, 0, 0, Complex.ImaginaryOne);

    public static void T(int t)
    {
        Complex phase = Complex.Exp(Complex.ImaginaryOne * (Math.PI / 4.0));
        Apply1(t, 1, 0, 0, phase);
    }

    public static void RX(int t, double theta)
    {
        double c = Math.Cos(theta / 2.0);
        double s = Math.Sin(theta / 2.0);
        Complex minusIS = -Complex.ImaginaryOne * s;

        Apply1(t, c, minusIS,
                  minusIS, c);
    }

    public static void RY(int t, double theta)
    {
        double c = Math.Cos(theta / 2.0);
        double s = Math.Sin(theta / 2.0);

        Apply1(t, c, -s,
                  s, c);
    }

    public static void RZ(int t, double theta)
    {
        Complex p0 = Complex.Exp(-Complex.ImaginaryOne * (theta / 2.0));
        Complex p1 = Complex.Exp(Complex.ImaginaryOne * (theta / 2.0));
        Apply1(t, p0, 0, 0, p1);
    }

    public static int MeasureAll() => Register.MeasureAll();

    public static int Measure(int q) => Register.MeasureQubit(q);

    public static void ExpectTop(int top = 16)
    {
        var p = Register.Probabilities();
        var idx = Enumerable.Range(0, p.Length)
                            .Where(i => p[i] > 1e-12)
                            .OrderByDescending(i => p[i])
                            .Take(top)
                            .ToArray();

        Console.WriteLine("Expectation (no collapse):");
        foreach (var i in idx)
            Console.WriteLine($"|{Register.BitString(i)}⟩  P = {p[i]:F6}");
    }

    public static void PrintStateTop(int top = 16)
    {
        var p = Register.Probabilities();
        var idx = Enumerable.Range(0, p.Length)
                            .Where(i => p[i] > 1e-12)
                            .OrderByDescending(i => p[i])
                            .Take(top)
                            .ToArray();

        Console.WriteLine($"n = {Register.QubitCount} qubits, |ψ⟩ has {Register.State.Length} amplitudes");
        Console.WriteLine("Top basis states:");
        foreach (var i in idx)
        {
            var amp = Register.State[i];
            Console.WriteLine($"|{Register.BitString(i)}⟩  amp={Fmt(amp)}  P={p[i]:F6}");
        }
    }

    public static Complex[] SnapshotState()
        => (Complex[])Register.State.Clone();

    public static void RestoreState(Complex[] snapshot)
    {
        if (snapshot.Length != Register.State.Length)
            throw new ArgumentException("Snapshot size does not match register.");
        Array.Copy(snapshot, Register.State, snapshot.Length);
        Register.Normalize();
    }

    private static string Fmt(Complex z)
    {
        double a = z.Real;
        double b = z.Imaginary;
        return $"{a:+0.######;-0.######;+0.######} {b:+0.######;-0.######;+0.######}i";
    }
}