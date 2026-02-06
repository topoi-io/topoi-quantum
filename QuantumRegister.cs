using System.Numerics;
using System.Security.Cryptography;

namespace QuantumComputer;

public sealed class QuantumRegister
{
    public int QubitCount { get; }
    public Complex[] State { get; } // length = 2^n

    public QuantumRegister(int qubitCount)
    {
        if (qubitCount <= 0) throw new ArgumentOutOfRangeException(nameof(qubitCount));
        if (qubitCount > 28) throw new ArgumentOutOfRangeException(nameof(qubitCount),
            "Too many qubits for a dense state-vector simulator in this toy interpreter.");

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
        {
            double mag = State[i].Magnitude;
            sum += mag * mag;
        }

        if (sum <= 0.0) throw new InvalidOperationException("State norm is zero.");

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
            double mag = State[i].Magnitude;
            double pi = mag * mag;
            p[i] = pi;
            sum += pi;
        }

        if (sum > 0)
        {
            for (int i = 0; i < p.Length; i++)
                p[i] /= sum;
        }

        return p;
    }

    // Measure full computational-basis state; collapses to |picked>
    public int MeasureAll()
    {
        var p = Probabilities();
        double r = NextUnitDouble();

        double acc = 0.0;
        int picked = 0;

        for (int i = 0; i < p.Length; i++)
        {
            acc += p[i];
            if (r < acc) { picked = i; break; }
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

        // 1) Compute probabilities P(0) and P(1) for this qubit
        double p0 = 0.0;
        double p1 = 0.0;

        for (int i = 0; i < State.Length; i++)
        {
            double mag = State[i].Magnitude;
            double p = mag * mag;
            if ((i & mask) == 0) p0 += p;
            else p1 += p;
        }

        double s = p0 + p1;
        if (s <= 0.0)
            throw new InvalidOperationException("State has zero norm.");

        // Normalize probabilities (guards against tiny numerical drift)
        p0 /= s;
        p1 /= s;

        // 2) Sample outcome
        double r = NextUnitDouble();
        int outcome = (r < p0) ? 0 : 1;

        // 3) Collapse: zero out amplitudes inconsistent with outcome
        // 4) Renormalize surviving amplitudes by dividing by sqrt(P(outcome))
        double keepProb = (outcome == 0) ? p0 : p1;

        // If keepProb is extremely tiny, collapse would blow up numerically.
        // In a unitary evolution this shouldn't happen unless probabilities are ~0.
        if (keepProb <= 0.0)
        {
            // Deterministic collapse to the other outcome (numerical edge case)
            outcome = 1 - outcome;
            keepProb = (outcome == 0) ? p0 : p1;
            if (keepProb <= 0.0)
                throw new InvalidOperationException("Both measurement probabilities are zero (invalid state).");
        }

        double invNorm = 1.0 / Math.Sqrt(keepProb);

        for (int i = 0; i < State.Length; i++)
        {
            bool bitIs1 = (i & mask) != 0;
            if ((outcome == 1) != bitIs1)
            {
                State[i] = Complex.Zero;
            }
            else
            {
                State[i] *= invNorm;
            }
        }

        return outcome;
    }

    public string BitString(int basisIndex)
        => Convert.ToString(basisIndex, 2).PadLeft(QubitCount, '0');

    private static double NextUnitDouble()
    {
        int x = RandomNumberGenerator.GetInt32(int.MaxValue);
        return x / (double)int.MaxValue;
    }
}