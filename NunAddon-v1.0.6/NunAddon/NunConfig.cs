using BepInEx.Configuration;

namespace NunAddon;

public enum ParryMode { Duel = 0, RockPaperScissors = 1 }

public static class NunConfig
{
    public static ConfigEntry<float> ConjureCooldown = null!;
    public static ConfigEntry<float> ConjureWindup = null!;
    public static ConfigEntry<float> SealDuration = null!;
    public static ConfigEntry<ParryMode> CounterMode = null!;
    public static ConfigEntry<float> DuelTime = null!;
    public static ConfigEntry<float> CounterRange = null!;

    public static void Bind(ConfigFile c)
    {
        ConjureCooldown = c.Bind("Nun", "Conjure Cooldown", 30f, "Seconds between Conjures.");
        ConjureWindup = c.Bind("Nun", "Conjure Windup", 4f, "Visible windup before sealing resolves.");
        SealDuration = c.Bind("Nun", "Seal Duration", 8f, "How long ghosts remain sealed.");
        CounterMode = c.Bind("Nun", "Parry Mode", ParryMode.Duel, "Duel or RockPaperScissors.");
        DuelTime = c.Bind("Nun", "Duel Time", 6f, "Time for the chosen living counter to find/click the Nun.");
        CounterRange = c.Bind("Nun", "Click Range", 1.25f, "Maximum click distance for the counter.");
    }
}
