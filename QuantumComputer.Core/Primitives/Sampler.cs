namespace QuantumComputer.Core.Primitives;

public sealed class Sampler
{
    private readonly IRandomSource? _randomSource;

    public Sampler(IRandomSource? randomSource = null)
    {
        _randomSource = randomSource;
    }

    public SamplerResult Run(QuantumCircuit circuit, int shots)
    {
        if (circuit is null)
            throw new ArgumentNullException(nameof(circuit));

        if (shots <= 0)
            throw new ArgumentOutOfRangeException(nameof(shots), "Shots must be positive.");

        var counts = CreateEmptyCounts(circuit.QubitCount);
        var simulator = new QuantumSimulator(circuit.QubitCount, _randomSource);

        for (int shot = 0; shot < shots; shot++)
        {
            circuit.Run(simulator, resetFirst: true);

            int measuredBasisIndex = simulator.MeasureAll();
            string bitString = ToBitString(measuredBasisIndex, circuit.QubitCount);

            counts[bitString]++;
        }

        IReadOnlyDictionary<string, double> probabilities =
            counts.ToDictionary(
                pair => pair.Key,
                pair => pair.Value / (double)shots);

        return new SamplerResult(
            circuit.QubitCount,
            shots,
            counts,
            probabilities);
    }

    private static Dictionary<string, int> CreateEmptyCounts(int qubitCount)
    {
        int basisStateCount = 1 << qubitCount;
        var counts = new Dictionary<string, int>(basisStateCount);

        for (int basis = 0; basis < basisStateCount; basis++)
            counts[ToBitString(basis, qubitCount)] = 0;

        return counts;
    }

    private static string ToBitString(int basisIndex, int qubitCount)
    {
        char[] bits = new char[qubitCount];

        for (int displayIndex = 0; displayIndex < qubitCount; displayIndex++)
        {
            int qubit = qubitCount - displayIndex - 1;
            bits[displayIndex] = ((basisIndex & (1 << qubit)) != 0) ? '1' : '0';
        }

        return new string(bits);
    }
}