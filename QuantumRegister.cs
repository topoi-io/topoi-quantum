using System.Numerics;
using System.Security.Cryptography;

namespace QuantumComputer;

public sealed class QuantumRegister
{
    private const int DefaultMaxQubits = 25;

    public int QubitCount { get; }
    public Complex[] State { get; } // length = 2^n

    public QuantumRegister(int qubitCount)
    {
        if (qubitCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(qubitCount));

        ValidateDenseStateVectorSize(qubitCount);

        QubitCount = qubitCount;
        State = new Complex[1 << qubitCount];
        Reset();
    }

    public void Reset()
    {
        Array.Clear(State);
        State[0] = Complex.One; // |00..0>
    }

    public void Normalize()
    {
        double sum = 0.0;

        for (int i = 0; i < State.Length; i++)
            sum += NormSquared(State[i]);

        if (sum <= 0.0)
            throw new InvalidOperationException("State norm is zero.");

        double inv = 1.0 / Math.Sqrt(sum);

        for (int i = 0; i < State.Length; i++)
            State[i] *= inv;
    }

    public double[] Probabilities()
    {
        var p = new double[State.Length];
        double sum = 0.0;

        for (int i = 0; i < State.Length; i++)
        {
            double pi = NormSquared(State[i]);
            p[i] = pi;
            sum += pi;
        }

        if (sum > 0.0)
        {
            double inv = 1.0 / sum;

            for (int i = 0; i < p.Length; i++)
                p[i] *= inv;
        }

        return p;
    }

    // Measure full computational-basis state; collapses to |picked>
    public int MeasureAll()
    {
        double total = 0.0;

        for (int i = 0; i < State.Length; i++)
            total += NormSquared(State[i]);

        if (total <= 0.0)
            throw new InvalidOperationException("State has zero norm.");

        double r = NextUnitDouble() * total;
        double acc = 0.0;

        // Important:
        // If rounding prevents r < acc from triggering,
        // the final valid bucket absorbs the residue.
        int picked = State.Length - 1;

        for (int i = 0; i < State.Length; i++)
        {
            acc += NormSquared(State[i]);

            if (r < acc)
            {
                picked = i;
                break;
            }
        }

        Array.Clear(State);
        State[picked] = Complex.One;

        return picked;
    }

    public int MeasureQubit(int q)
    {
        if ((uint)q >= (uint)QubitCount)
            throw new ArgumentOutOfRangeException(nameof(q), $"Qubit index must be 0..{QubitCount - 1}");

        int mask = 1 << q;

        double p0 = 0.0;
        double p1 = 0.0;

        for (int i = 0; i < State.Length; i++)
        {
            double p = NormSquared(State[i]);

            if ((i & mask) == 0)
                p0 += p;
            else
                p1 += p;
        }

        double total = p0 + p1;

        if (total <= 0.0)
            throw new InvalidOperationException("State has zero norm.");

        p0 /= total;
        p1 /= total;

        double r = NextUnitDouble();
        int outcome = r < p0 ? 0 : 1;

        double keepProb = outcome == 0 ? p0 : p1;

        if (keepProb <= 0.0)
        {
            outcome = 1 - outcome;
            keepProb = outcome == 0 ? p0 : p1;

            if (keepProb <= 0.0)
                throw new InvalidOperationException("Both measurement probabilities are zero.");
        }

        double invNorm = 1.0 / Math.Sqrt(keepProb);

        for (int i = 0; i < State.Length; i++)
        {
            bool bitIs1 = (i & mask) != 0;

            if ((outcome == 1) != bitIs1)
                State[i] = Complex.Zero;
            else
                State[i] *= invNorm;
        }

        return outcome;
    }

    public string BitString(int basisIndex)
        => Convert.ToString(basisIndex, 2).PadLeft(QubitCount, '0');

    public Complex ExpectPauliString(IReadOnlyList<Quantum.PauliTerm> terms)
    {
        if (terms.Count == 0)
            throw new ArgumentException("Observable must contain at least one Pauli term.", nameof(terms));

        ValidatePauliTerms(terms);

        Complex numerator = Complex.Zero;
        double denominator = 0.0;

        for (int i = 0; i < State.Length; i++)
        {
            Complex amplitude = State[i];

            denominator += NormSquared(amplitude);

            int mappedIndex = i;
            Complex phase = Complex.One;

            foreach (var term in terms)
            {
                int mask = 1 << term.Qubit;
                bool bitIs1 = (mappedIndex & mask) != 0;

                switch (term.Pauli)
                {
                    case 'I':
                        break;

                    case 'X':
                        mappedIndex ^= mask;
                        break;

                    case 'Y':
                        // Y|0⟩ = i|1⟩
                        // Y|1⟩ = -i|0⟩
                        phase *= bitIs1 ? -Complex.ImaginaryOne : Complex.ImaginaryOne;
                        mappedIndex ^= mask;
                        break;

                    case 'Z':
                        // Z|0⟩ = |0⟩
                        // Z|1⟩ = -|1⟩
                        if (bitIs1)
                            phase = -phase;
                        break;

                    default:
                        throw new ArgumentException($"Unsupported Pauli operator '{term.Pauli}'.");
                }
            }

            numerator += Complex.Conjugate(amplitude) * phase * State[mappedIndex];
        }

        if (denominator <= 0.0)
            throw new InvalidOperationException("State has zero norm.");

        return numerator / denominator;
    }

    private double NextUnitDouble()
    {
        int x = RandomNumberGenerator.GetInt32(int.MaxValue);
        return x / (double)int.MaxValue;
    }

    private double NormSquared(Complex z)
    {
        return z.Real * z.Real + z.Imaginary * z.Imaginary;
    }

    private void ValidateDenseStateVectorSize(int qubitCount)
    {
        if (qubitCount > DefaultMaxQubits)
        {
            long bytes = EstimateStateVectorBytes(qubitCount);

            throw new ArgumentOutOfRangeException(
                nameof(qubitCount),
                $"This dense state-vector simulator stores 2^n complex amplitudes. " +
                $"{qubitCount} qubits requires approximately {FormatBytes(bytes)} just for the amplitude array. " +
                $"The default safety limit is {DefaultMaxQubits} qubits. " +
                $"Use fewer qubits, or later add an explicit advanced override if you really want larger simulations.");
        }
    }

    private long EstimateStateVectorBytes(int qubitCount)
    {
        // Complex = two doubles = 16 bytes
        return 16L * (1L << qubitCount);
    }

    private string FormatBytes(long bytes)
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

    private void ValidatePauliTerms(IReadOnlyList<Quantum.PauliTerm> terms)
    {
        var usedQubits = new HashSet<int>();

        foreach (var term in terms)
        {
            if (term.Pauli is not ('I' or 'X' or 'Y' or 'Z'))
                throw new ArgumentException($"Unsupported Pauli operator '{term.Pauli}'. Use I, X, Y, or Z.");

            if ((uint)term.Qubit >= (uint)QubitCount)
                throw new ArgumentOutOfRangeException(
                    nameof(term.Qubit),
                    $"Qubit index {term.Qubit} is out of range. Must be 0..{QubitCount - 1}");

            if (!usedQubits.Add(term.Qubit))
                throw new ArgumentException(
                    $"Observable contains more than one Pauli operator on qubit {term.Qubit}. " +
                    $"Use one operator per qubit, e.g. Z0 X1, not Z0 X0.");
        }
    }
}