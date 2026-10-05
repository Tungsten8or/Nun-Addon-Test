using HarmonyLib;
using NunAddon.Gameplay;

namespace NunAddon.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
public static class NunVisibilityPatch
{
    public static void Postfix(PlayerControl __instance)
    {
        if (!NunState.Active || __instance.PlayerId != NunState.NunId) return;
        var local = PlayerControl.LocalPlayer;
        if (local == null) return;

        var canCounter = NunState.IsCounter(local.PlayerId);
        _ = canCounter;
    }
}
