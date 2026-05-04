using System.Numerics;

namespace QuantumComputer;

public static class Quantum
{
    private const long NormalizeEveryUnitaryGates = 4096;
    private static long _unitaryGateCount;

    public static QuantumRegister Register { get; private set; } = new QuantumRegister(1);

    public static void Init(int n)
    {
        Register = new QuantumRegister(n);
        _unitaryGateCount = 0;
    }

    public static void Reset()
    {
        Register.Reset();
        _unitaryGateCount = 0;
    }

    public static Complex ExpectPauliString(IReadOnlyList<PauliTerm> terms)
        => Register.ExpectPauliString(terms);

    public static void Apply1(int t, Complex m00, Complex m01, Complex m10, Complex m11)
    {
        if ((uint)t >= (uint)Register.QubitCount)
            throw new ArgumentOutOfRangeException(nameof(t), $"Qubit index out of range. Must be 0..{Register.QubitCount - 1}");

        int stride = 1 << t;
        int step = stride << 1;
        var v = Register.MutableState;

        for (int block = 0; block < v.Length; block += step)
        {
            for (int i = 0; i < stride; i++)
            {
                int i0 = block + i;
                int i1 = i0 + stride;

                Complex a = v[i0];
                Complex b = v[i1];

                v[i0] = m00 * a + m01 * b;
                v[i1] = m10 * a + m11 * b;
            }
        }

        AfterUnitaryGate();
    }

    public static void X(int t) => Apply1(t, 0, 1, 1, 0);

    public static void Y(int t) => Apply1(t, 0, -Complex.ImaginaryOne, Complex.ImaginaryOne, 0);

    public static void Z(int t) => Apply1(t, 1, 0, 0, -1);

    public static void H(int t)
    {
        double invSqrt2 = 1.0 / Math.Sqrt(2.0);
        Apply1(t, invSqrt2, invSqrt2, invSqrt2, -invSqrt2);
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

        Apply1(t, c, minusIS, minusIS, c);
    }

    public static void RY(int t, double theta)
    {
        double c = Math.Cos(theta / 2.0);
        double s = Math.Sin(theta / 2.0);

        Apply1(t, c, -s, s, c);
    }

    public static void RZ(int t, double theta)
    {
        Complex p0 = Complex.Exp(-Complex.ImaginaryOne * (theta / 2.0));
        Complex p1 = Complex.Exp(Complex.ImaginaryOne * (theta / 2.0));

        Apply1(t, p0, 0, 0, p1);
    }

    public static void CX(int control, int target)
    {
        Controlled1(control, target, 0, 1, 1, 0);
    }

    public static void CNOT(int control, int target)
    {
        CX(control, target);
    }

    public static void CZ(int control, int target)
    {
        Register.ApplyCZ(control, target);
        AfterUnitaryGate();
    }

    public static void SWAP(int q1, int q2)
    {
        Register.ApplySWAP(q1, q2);
        AfterUnitaryGate();
    }

    public static void CCX(int control1, int control2, int target)
    {
        Controlled1(new[] { control1, control2 }, target, 0, 1, 1, 0);
    }

    public static void TOFFOLI(int control1, int control2, int target)
    {
        CCX(control1, control2, target);
    }

    public static void CRX(int control, int target, double theta)
    {
        double c = Math.Cos(theta / 2.0);
        double s = Math.Sin(theta / 2.0);
        Complex minusIS = -Complex.ImaginaryOne * s;

        Controlled1(control, target, c, minusIS, minusIS, c);
    }

    public static void CRY(int control, int target, double theta)
    {
        double c = Math.Cos(theta / 2.0);
        double s = Math.Sin(theta / 2.0);

        Controlled1(control, target, c, -s, s, c);
    }

    public static void CRZ(int control, int target, double theta)
    {
        Complex p0 = Complex.Exp(-Complex.ImaginaryOne * (theta / 2.0));
        Complex p1 = Complex.Exp(Complex.ImaginaryOne * (theta / 2.0));

        Controlled1(control, target, p0, 0, 0, p1);
    }

    public static void Controlled1(
        int control,
        int target,
        Complex m00,
        Complex m01,
        Complex m10,
        Complex m11)
    {
        Register.ApplyControlled1(
            new[] { control },
            target,
            m00,
            m01,
            m10,
            m11);

        AfterUnitaryGate();
    }

    public static void Controlled1(
        IReadOnlyList<int> controls,
        int target,
        Complex m00,
        Complex m01,
        Complex m10,
        Complex m11)
    {
        Register.ApplyControlled1(
            controls,
            target,
            m00,
            m01,
            m10,
            m11);

        AfterUnitaryGate();
    }

    public static int MeasureAll()
    {
        int result = Register.MeasureAll();
        _unitaryGateCount = 0;
        return result;
    }

    public static int Measure(int q)
    {
        int result = Register.MeasureQubit(q);
        _unitaryGateCount = 0;
        return result;
    }

    public static void PrintProbabilitiesTop(int top = 16)
    {
        var p = Register.Probabilities();
        var idx = Enumerable.Range(0, p.Length)
                            .Where(i => p[i] > 1e-12)
                            .OrderByDescending(i => p[i])
                            .Take(top)
                            .ToArray();

        Console.WriteLine("Basis-state probabilities:");
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
    => (Complex[])Register.MutableState.Clone();

    public static void RestoreState(Complex[] snapshot)
    {
        if (snapshot.Length != Register.MutableState.Length)
            throw new ArgumentException("Snapshot size does not match register.");

        Array.Copy(snapshot, Register.MutableState, snapshot.Length);

#if DEBUG
        Register.AssertNormalized();
#endif

        _unitaryGateCount = 0;
    }

    public static void PrintMemoryEstimate()
    {
        int n = Register.QubitCount;
        long stateBytes = EstimateStateVectorBytes(n);

        Console.WriteLine($"Qubits: {n}");
        Console.WriteLine($"Amplitudes: 2^{n} = {Register.State.Length:N0}");
        Console.WriteLine($"State vector only: {FormatBytes(stateBytes)}");
        Console.WriteLine($"Snapshot copy: another {FormatBytes(stateBytes)}");
        Console.WriteLine($"Probability array: approximately {FormatBytes(8L * Register.State.Length)}");
    }

    public static void Normalize()
    {
        Register.Normalize();
        _unitaryGateCount = 0;
    }

    public static void PrintNorm()
    {
        double normSquared = Register.NormSquared();
        Console.WriteLine($"Norm² = {normSquared:R}");
        Console.WriteLine($"Norm  = {Math.Sqrt(normSquared):R}");
    }

    private static long EstimateStateVectorBytes(int qubitCount)
    {
        return 16L * (1L << qubitCount);
    }

    private static string FormatBytes(long bytes)
    {
        const double KiB = 1024.0;
        const double MiB = KiB * 1024.0;
        const double GiB = MiB * 1024.0;

        if (bytes >= GiB)
            return $"{bytes / GiB:F2} GiB";

        if (bytes >= MiB)
            return $"{bytes / MiB:F2} MiB";

        if (bytes >= KiB)
            return $"{bytes / KiB:F2} KiB";

        return $"{bytes} bytes";
    }

    private static string Fmt(Complex z)
    {
        double a = z.Real;
        double b = z.Imaginary;
        return $"{a:+0.######;-0.######;+0.######} {b:+0.######;-0.######;+0.######}i";
    }

    private static void AfterUnitaryGate()
    {
        _unitaryGateCount++;

#if DEBUG
        Register.AssertNormalized();
#endif

        if (_unitaryGateCount % NormalizeEveryUnitaryGates == 0)
            Register.Normalize();
    }
}