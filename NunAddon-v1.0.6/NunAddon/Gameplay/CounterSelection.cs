using System;
using System.Collections.Generic;

namespace NunAddon.Gameplay;

public static class CounterSelection
{
    public static byte Choose(IReadOnlyList<byte> livingEligible, int seed)
    {
        if (livingEligible.Count == 0) return byte.MaxValue;
        var rng = new Random(seed);
        return livingEligible[rng.Next(livingEligible.Count)];
    }
}
