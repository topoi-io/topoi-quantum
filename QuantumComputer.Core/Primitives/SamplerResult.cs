namespace QuantumComputer.Core.Primitives;

public sealed record SamplerResult(
    int QubitCount,
    int Shots,
    IReadOnlyDictionary<string, int> Counts,
    IReadOnlyDictionary<string, double> Probabilities)
{
    public int CountFor(string bitString)
    {
        if (bitString is null)
            throw new ArgumentNullException(nameof(bitString));

        return Counts.TryGetValue(bitString, out int count)
            ? count
            : 0;
    }

    public double ProbabilityFor(string bitString)
    {
        if (bitString is null)
            throw new ArgumentNullException(nameof(bitString));

        return Probabilities.TryGetValue(bitString, out double probability)
            ? probability
            : 0.0;
    }
}