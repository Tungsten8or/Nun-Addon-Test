using System.Linq;
using NunAddon.Roles;
using UnityEngine;

namespace NunAddon.Gameplay;

public static class NunController
{
    public static void BeginConjure(PlayerControl nun)
    {
        if (nun.Data?.Role is not NunRole) return;
        var now = Time.time;
        if (NunState.Phase != NunPhase.Idle || now < NunState.CooldownEndsAt) return;

        CleanupCastPresentation();
        NunState.SealedGhosts.Clear();
        NunState.MediumCounters.Clear();
        NunState.CounterId = byte.MaxValue;

        NunState.NunId = nun.PlayerId;
        NunState.CooldownEndsAt = now + NunConfig.ConjureCooldown.Value;

        NunState.CounterId = CounterSelection.Choose(
            TouBridge.LivingCounterCandidates(nun.PlayerId),
            (int)(now * 1000) ^ nun.PlayerId);

        foreach (var p in TouBridge.Players())
            if (p.PlayerId != nun.PlayerId && !TouBridge.IsDead(p) && TouBridge.IsGhostedMedium(p))
                NunState.MediumCounters.Add(p.PlayerId);

        NunState.Phase = NunPhase.Windup;
        NunState.PhaseEndsAt = now + NunConfig.ConjureWindup.Value;
        NunPlugin.InstanceLog($"Conjure START counter={NunState.CounterId} windup={NunConfig.ConjureWindup.Value:0.0}s cooldownEnds={NunState.CooldownEndsAt:0.0}");

        TouBridge.Freeze(nun, true);

        SealVisual.Show(nun);

        ApplyCounterPresentation(true);
    }

    public static void Tick()
    {
        if (!NunState.Active) return;
        var now = Time.time;

        foreach (var p in TouBridge.Players())
        {
            if (NunState.IsSealed(p.PlayerId))
                SealVisual.Tick(p);
            else if (p.PlayerId == NunState.NunId && NunState.Phase == NunPhase.Windup)
                SealVisual.Tick(p);
        }

        if (NunState.Phase == NunPhase.Windup && now >= NunState.PhaseEndsAt)
            ResolveConjure();
        else if (NunState.Phase == NunPhase.Sealed && now >= NunState.PhaseEndsAt)
        {
            ReleaseAll();
            NunState.EndNormally(now);
            NunPlugin.InstanceLog($"Conjure END cooldownRemaining={Mathf.Max(0f, NunState.CooldownEndsAt - now):0.0}s");
        }
    }

    private static void ResolveConjure()
    {
        var now = Time.time;

        SetNunFrozen(false);
        SealVisual.Hide(NunState.NunId);
        ApplyCounterPresentation(false);

        NunState.SealedGhosts.Clear();
        foreach (var p in TouBridge.DeadGhostTargets(NunState.NunId))
        {
            NunState.SealedGhosts.Add(p.PlayerId);
            TouBridge.Freeze(p, true);
            SealVisual.Show(p);
        }

        NunState.Phase = NunPhase.Sealed;
        NunState.PhaseEndsAt = now + NunConfig.SealDuration.Value;
        NunPlugin.InstanceLog($"Conjure RESOLVE sealed={NunState.SealedGhosts.Count} sealDuration={NunConfig.SealDuration.Value:0.0}s");
    }

    public static void TryClickNun(PlayerControl clicker)
    {
        if (NunState.Phase != NunPhase.Windup) return;
        if (!NunState.IsCounter(clicker.PlayerId)) return;

        var nun = TouBridge.Players().FirstOrDefault(p => p.PlayerId == NunState.NunId);
        if (nun == null) return;
        if (Vector2.Distance(clicker.GetTruePosition(), nun.GetTruePosition()) >
            NunConfig.CounterRange.Value) return;

        if (NunConfig.CounterMode.Value == ParryMode.Duel)
            Parry();
        else if (!RpsCounter.Open)
            RpsCounter.Begin(clicker.PlayerId, NunState.NunId);
    }

    public static void Parry()
    {
        CleanupCastPresentation();
        ReleaseAll();
        RpsCounter.Reset();
        NunState.Parried(Time.time);
        NunPlugin.InstanceLog($"Conjure PARRIED cooldown={NunConfig.ConjureCooldown.Value * 2f:0.0}s");
    }

    public static void ReleaseAll()
    {
        SetNunFrozen(false);
        foreach (var p in TouBridge.Players())
        {
            if (NunState.IsSealed(p.PlayerId))
                TouBridge.Freeze(p, false);
            SealVisual.Hide(p.PlayerId);
        }
        NunState.SealedGhosts.Clear();
    }

    private static void CleanupCastPresentation()
    {
        SetNunFrozen(false);
        if (NunState.NunId != byte.MaxValue)
            SealVisual.Hide(NunState.NunId);
        ApplyCounterPresentation(false);
        RpsCounter.Reset();
    }

    private static void SetNunFrozen(bool frozen)
    {
        var nun = TouBridge.Players().FirstOrDefault(p => p.PlayerId == NunState.NunId);
        if (nun != null) TouBridge.Freeze(nun, frozen);
    }

    private static void ApplyCounterPresentation(bool on)
    {
        foreach (var p in TouBridge.Players())
        {
            if (!NunState.IsCounter(p.PlayerId)) continue;

            if (p.PlayerId == NunState.CounterId && !TouBridge.IsGhostedMedium(p))
                TouBridge.SetCounterGhostLook(p, on);
            else
                TouBridge.TintRed(p, on);
        }
    }
}
