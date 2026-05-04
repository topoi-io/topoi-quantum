using System.Numerics;

namespace QuantumComputer;

public static class QuantumConsolePrinter
{
    public static void PrintProbabilitiesTop(QuantumSimulator simulator, int top = 16)
    {
        PrintProbabilitiesTop(simulator.Register, top);
    }

    public static void PrintProbabilitiesTop(QuantumRegister register, int top = 16)
    {
        var p = register.Probabilities();

        var idx = Enumerable.Range(0, p.Length)
                            .Where(i => p[i] > 1e-12)
                            .OrderByDescending(i => p[i])
                            .Take(top)
                            .ToArray();

        Console.WriteLine("Basis-state probabilities:");

        foreach (var i in idx)
            Console.WriteLine($"|{register.BitString(i)}⟩  P = {p[i]:F6}");
    }

    public static void PrintStateTop(QuantumSimulator simulator, int top = 16)
    {
        PrintStateTop(simulator.Register, top);
    }

    public static void PrintStateTop(QuantumRegister register, int top = 16)
    {
        var p = register.Probabilities();

        var idx = Enumerable.Range(0, p.Length)
                            .Where(i => p[i] > 1e-12)
                            .OrderByDescending(i => p[i])
                            .Take(top)
                            .ToArray();

        Console.WriteLine($"n = {register.QubitCount} qubits, |ψ⟩ has {register.State.Length} amplitudes");
        Console.WriteLine("Top basis states:");

        foreach (var i in idx)
        {
            Complex amp = register.State[i];
            Console.WriteLine($"|{register.BitString(i)}⟩  amp={Fmt(amp)}  P={p[i]:F6}");
        }
    }

    public static void PrintNorm(QuantumRegister register)
    {
        double normSquared = register.NormSquared();

        Console.WriteLine($"Norm² = {normSquared:R}");
        Console.WriteLine($"Norm  = {Math.Sqrt(normSquared):R}");
    }

    public static void PrintMemoryEstimate(QuantumRegister register)
    {
        int n = register.QubitCount;
        long stateBytes = EstimateStateVectorBytes(n);

        Console.WriteLine($"Qubits: {n}");
        Console.WriteLine($"Amplitudes: 2^{n} = {register.State.Length:N0}");
        Console.WriteLine($"State vector only: {FormatBytes(stateBytes)}");
        Console.WriteLine($"Snapshot copy: another {FormatBytes(stateBytes)}");
        Console.WriteLine($"Probability array: approximately {FormatBytes(8L * register.State.Length)}");
    }

    public static void PrintExpectation(QuantumSimulator simulator, PauliTerm[] terms)
    {
        Complex value = simulator.ExpectPauliString(terms);

        Console.WriteLine($"⟨{ObservableParser.Format(terms)}⟩ = {value.Real:+0.############;-0.############;0}");

        if (Math.Abs(value.Imaginary) > 1e-10)
            Console.WriteLine($"  note: small imaginary residue = {value.Imaginary:+0.###e+0;-0.###e+0;0}");
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
}