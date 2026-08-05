namespace Topoi.Quantum.Primitives;

public sealed class Sampler
{
    private readonly IRandomSource? _randomSource;

    public Sampler(IRandomSource? randomSource = null)
    {
        _randomSource = randomSource;
    }

    public SamplerResult Run(QuantumCircuit circuit, int shots)
    {
        ArgumentNullException.ThrowIfNull(circuit);

        if (shots <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(shots),
                shots,
                "Shots must be positive.");
        }

        /*
         * Store counts using the integer basis-state index while sampling.
         *
         * This avoids:
         *
         * 1. Allocating entries for all 2^n possible states.
         * 2. Creating a new bit-string for every individual shot.
         *
         * The dictionary can contain no more entries than the number
         * of shots, regardless of the number of qubits.
         */
        var basisCounts = new Dictionary<int, int>();

        var simulator = new QuantumSimulator(
            circuit.QubitCount,
            _randomSource);

        for (int shot = 0; shot < shots; shot++)
        {
            circuit.Run(
                simulator,
                resetFirst: true);

            int measuredBasisIndex = simulator.MeasureAll();

            basisCounts[measuredBasisIndex] =
                basisCounts.TryGetValue(
                    measuredBasisIndex,
                    out int currentCount)
                    ? currentCount + 1
                    : 1;
        }

        /*
         * Convert each distinct measured outcome to a bit-string once.
         *
         * Counts and probabilities contain observed outcomes only.
         */
        var counts = new Dictionary<string, int>(
            basisCounts.Count,
            StringComparer.Ordinal);

        var probabilities = new Dictionary<string, double>(
            basisCounts.Count,
            StringComparer.Ordinal);

        foreach (KeyValuePair<int, int> outcome in basisCounts)
        {
            string bitString = ToBitString(
                outcome.Key,
                circuit.QubitCount);

            counts.Add(
                bitString,
                outcome.Value);

            probabilities.Add(
                bitString,
                outcome.Value / (double)shots);
        }

        return new SamplerResult(
            circuit.QubitCount,
            shots,
            counts,
            probabilities);
    }

    private static string ToBitString(
        int basisIndex,
        int qubitCount)
    {
        char[] bits = new char[qubitCount];

        for (int displayIndex = 0;
             displayIndex < qubitCount;
             displayIndex++)
        {
            int qubit =
                qubitCount - displayIndex - 1;

            bits[displayIndex] =
                (basisIndex & (1 << qubit)) != 0
                    ? '1'
                    : '0';
        }

        return new string(bits);
    }
}