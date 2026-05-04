using System.Numerics;

namespace QuantumComputer;

public static class Quantum
{
    private static QuantumSimulator _simulator = new(1);

    internal static QuantumSimulator DefaultSimulator => _simulator;

    public static QuantumRegister Register => _simulator.Register;

    public static void Init(int n)
    {
        _simulator = new QuantumSimulator(n);
    }

    public static void Reset() => _simulator.Reset();

    public static Complex ExpectPauliString(IReadOnlyList<PauliTerm> terms)
        => _simulator.ExpectPauliString(terms);

    public static void X(int t) => _simulator.X(t);
    public static void Y(int t) => _simulator.Y(t);
    public static void Z(int t) => _simulator.Z(t);
    public static void H(int t) => _simulator.H(t);
    public static void S(int t) => _simulator.S(t);
    public static void T(int t) => _simulator.T(t);

    public static void RX(int t, double theta) => _simulator.RX(t, theta);
    public static void RY(int t, double theta) => _simulator.RY(t, theta);
    public static void RZ(int t, double theta) => _simulator.RZ(t, theta);

    public static void CX(int control, int target) => _simulator.CX(control, target);
    public static void CNOT(int control, int target) => _simulator.CNOT(control, target);
    public static void CZ(int control, int target) => _simulator.CZ(control, target);
    public static void SWAP(int q1, int q2) => _simulator.SWAP(q1, q2);
    public static void CCX(int control1, int control2, int target) => _simulator.CCX(control1, control2, target);
    public static void TOFFOLI(int control1, int control2, int target) => _simulator.TOFFOLI(control1, control2, target);

    public static void CRX(int control, int target, double theta) => _simulator.CRX(control, target, theta);
    public static void CRY(int control, int target, double theta) => _simulator.CRY(control, target, theta);
    public static void CRZ(int control, int target, double theta) => _simulator.CRZ(control, target, theta);

    public static int MeasureAll() => _simulator.MeasureAll();
    public static int Measure(int q) => _simulator.Measure(q);

    public static Complex[] SnapshotState() => _simulator.SnapshotState();
    public static void RestoreState(Complex[] snapshot) => _simulator.RestoreState(snapshot);

    public static void Normalize() => _simulator.Normalize();

    // Keep your existing PrintStateTop, PrintProbabilitiesTop, PrintMemoryEstimate,
    // PrintNorm, FormatBytes, Fmt methods either here or move them into a separate
    // QuantumConsolePrinter later.
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

    public static void PrintNorm()
    {
        double normSquared = Register.NormSquared();
        Console.WriteLine($"Norm² = {normSquared:R}");
        Console.WriteLine($"Norm  = {Math.Sqrt(normSquared):R}");
    }

    private static string Fmt(Complex z)
    {
        double a = z.Real;
        double b = z.Imaginary;
        return $"{a:+0.######;-0.######;+0.######} {b:+0.######;-0.######;+0.######}i";
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
}