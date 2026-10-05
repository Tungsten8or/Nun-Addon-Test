using MiraAPI.Roles;
using UnityEngine;

namespace NunAddon.Roles;

public sealed class NunRole : ImpostorRole, ICustomRole
{
    public string RoleName => "Nun";
    public string RoleDescription => "Seal the dead away from the living.";
    public string RoleLongDescription =>
        "Use Conjure to seal other ghosts. One living player is chosen to counter you; " +
        "Mediums already walking as spirits can also intervene.";

    public Color RoleColor => new(0.72f, 0.08f, 0.08f, 1f);
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public override bool IsDead => true;

    public CustomRoleConfiguration Configuration => new(this)
    {
        MaxRoleCount = 0,
        DefaultRoleCount = 0,
        DefaultChance = 0,
        CanModifyChance = false,
        HideSettings = false,
        ShowInFreeplay = true,
        FreeplayFolder = "Impostor",
        Icon = NunAssets.RoleIcon,
        OptionsScreenshot = NunAssets.RoleIcon,
        CanGetKilled = false,
        UseVanillaKillButton = false,
        CanUseVent = true,
        CanUseSabotage = true,
        TasksCountForProgress = false,
    };
}
