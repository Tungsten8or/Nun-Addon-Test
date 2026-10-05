using HarmonyLib;
using NunAddon.Gameplay;
using NunAddon.Roles;
using UnityEngine;

namespace NunAddon.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class NunHudUpdatePatch
{
    public static void Postfix()
    {
        var local = PlayerControl.LocalPlayer;
        if (local != null && local.Data?.Role is NunRole)
            NunState.NunId = local.PlayerId;

        NunController.Tick();

        if (local == null) return;

        if (NunState.IsCounter(local.PlayerId) && Input.GetMouseButtonDown(0))
            NunController.TryClickNun(local);

        if (RpsCounter.Open && RpsCounter.Counter == local.PlayerId)
        {
            if (Input.GetKeyDown(KeyCode.R)) RpsCounter.Cycle();
            if (Input.GetKeyDown(KeyCode.F)) RpsCounter.Submit();
        }
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
public static class NunSealMovementPatch
{
    public static void Postfix(PlayerPhysics __instance)
    {
        var p = __instance.myPlayer;
        if (p != null &&
            (NunState.IsSealed(p.PlayerId) ||
             (p.PlayerId == NunState.NunId && NunState.Phase == NunPhase.Windup)))
            __instance.body.velocity = Vector2.zero;
    }
}
