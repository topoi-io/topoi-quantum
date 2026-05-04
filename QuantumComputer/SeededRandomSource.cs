namespace QuantumComputer;

public sealed class SeededRandomSource : IRandomSource
{
    private readonly Random _random;

    public SeededRandomSource(int seed)
    {
        _random = new Random(seed);
    }

    public double NextDouble()
    {
        return _random.NextDouble();
    }
}