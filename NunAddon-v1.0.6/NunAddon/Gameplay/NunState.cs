using System.Collections.Generic;

namespace NunAddon.Gameplay;

public enum NunPhase : byte { Idle, Windup, Counter, Sealed }

public static class NunState
{
    public static byte NunId = byte.MaxValue;
    public static byte CounterId = byte.MaxValue;
    public static NunPhase Phase;
    public static float PhaseEndsAt;
    public static float CooldownEndsAt;
    public static readonly HashSet<byte> SealedGhosts = new();
    public static readonly HashSet<byte> MediumCounters = new();

    public static bool Active => NunId != byte.MaxValue;
    public static bool IsCounter(byte id) => id == CounterId || MediumCounters.Contains(id);
    public static bool IsSealed(byte id) => SealedGhosts.Contains(id);

    public static void Reset()
    {
        NunId = CounterId = byte.MaxValue;
        Phase = NunPhase.Idle;
        PhaseEndsAt = CooldownEndsAt = 0;
        SealedGhosts.Clear();
        MediumCounters.Clear();
    }

    public static void Parried(float now)
    {
        Phase = NunPhase.Idle;
        PhaseEndsAt = 0;
        SealedGhosts.Clear();
        MediumCounters.Clear();
        CounterId = byte.MaxValue;
        CooldownEndsAt = now + NunConfig.ConjureCooldown.Value * 2f;
    }

    public static void EndNormally(float now)
    {
        Phase = NunPhase.Idle;
        PhaseEndsAt = 0;
        SealedGhosts.Clear();
        MediumCounters.Clear();
        CounterId = byte.MaxValue;
    }
}
