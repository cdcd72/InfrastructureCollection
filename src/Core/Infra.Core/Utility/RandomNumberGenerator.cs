using System;

namespace Infra.Core.Utility;

public static class RandomNumberGenerator
{
    private static readonly byte[] Rb = new byte[4];

    public static int Next(int min, int max)
    {
        if (min > max)
            throw new ArgumentOutOfRangeException(nameof(min));

        return Next(max - min) + min;
    }

    public static int Next(int max)
    {
        if (max < 0)
            throw new ArgumentOutOfRangeException(nameof(max));

        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();

        rng.GetBytes(Rb);

        var value = BitConverter.ToInt32(Rb, 0);

        value %= max + 1;

        if (value < 0) value = -value;

        return value;
    }
}
