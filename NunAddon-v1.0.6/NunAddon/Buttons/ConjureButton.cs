using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Utilities.Assets;
using NunAddon.Gameplay;
using NunAddon.Roles;
using UnityEngine;

namespace NunAddon.Buttons;

public sealed class ConjureButton : CustomActionButton
{
    public override string Name
    {
        get
        {
            if (NunState.Phase == NunPhase.Windup) return "CONJURING";
            if (NunState.Phase == NunPhase.Sealed) return "SEALED";
            var left = NunState.CooldownEndsAt - Time.time;
            return left > 0f ? $"COOLDOWN {Mathf.CeilToInt(left)}" : "Conjure";
        }
    }
    public override float Cooldown => NunConfig.ConjureCooldown.Value;
    public override float EffectDuration => 0f;
    public override int MaxUses => 0;
    public override LoadableAsset<Sprite> Sprite => NunAssets.RoleIcon;
    public override BaseKeybind? Keybind => MiraGlobalKeybinds.PrimaryAbility;
    public override ButtonLocation Location => ButtonLocation.BottomRight;

    public override bool Enabled(RoleBehaviour? role) => role is NunRole;

    public override bool CanUse()
    {
        var local = PlayerControl.LocalPlayer;
        return base.CanUse() &&
               local != null &&
               local.Data != null &&
               local.Data.Role is NunRole &&
               NunState.Phase == NunPhase.Idle &&
               Time.time >= NunState.CooldownEndsAt;
    }

    protected override void OnClick()
    {
        var local = PlayerControl.LocalPlayer;
        if (local == null) return;

        NunState.NunId = local.PlayerId;
        NunController.BeginConjure(local);

        Gameplay.SabotageCooldownBridge.StartFromConjure();
    }
}
