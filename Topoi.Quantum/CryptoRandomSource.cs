using System.Security.Cryptography;

namespace Topoi.Quantum;

public sealed class CryptoRandomSource : IRandomSource
{
    public static CryptoRandomSource Shared { get; } = new();

    private CryptoRandomSource()
    {
    }

    public double NextDouble()
    {
        int x = RandomNumberGenerator.GetInt32(int.MaxValue);
        return x / (double)int.MaxValue;
    }
}