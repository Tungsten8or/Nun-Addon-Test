using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace NunAddon.Gameplay;

public static class TouBridge
{
    private static Assembly? Tou => AppDomain.CurrentDomain.GetAssemblies()
        .FirstOrDefault(a => a.GetName().Name == "TownOfUsMira");

    public static bool IsMedium(PlayerControl p)
        => HasRoleName(p, "MediumRole") || HasRoleName(p, "Medium");

    public static bool IsHaunter(PlayerControl p)
        => HasRoleName(p, "HaunterRole") || HasRoleName(p, "Haunter");

    public static bool IsSpectre(PlayerControl p)
        => HasRoleName(p, "SpectreRole") || HasRoleName(p, "Spectre");

    public static bool IsInfluencer(PlayerControl p)
        => HasRoleName(p, "InfluencerRole") || HasRoleName(p, "Influencer");

    public static bool IsGhostedMedium(PlayerControl p)
    {
        if (!IsMedium(p)) return false;
        return ReadBool(p, "IsGhost") || ReadBool(p, "IsGhosted") ||
               ReadBool(p, "Mediating") || ReadBool(p, "IsMediating");
    }

    public static bool IsDead(PlayerControl p)
        => p.Data != null && p.Data.IsDead;

    public static IEnumerable<PlayerControl> Players()
    {
        if (PlayerControl.AllPlayerControls == null) yield break;
        foreach (var p in PlayerControl.AllPlayerControls)
            if (p != null && p.Data != null && !p.Data.Disconnected) yield return p;
    }

    public static List<byte> LivingCounterCandidates(byte nunId)
        => Players().Where(p => p.PlayerId != nunId && !IsDead(p))
                    .Select(p => p.PlayerId).ToList();

    public static IEnumerable<PlayerControl> DeadGhostTargets(byte nunId)
        => Players().Where(p => p.PlayerId != nunId && IsDead(p));

    public static void Freeze(PlayerControl p, bool freeze)
    {
        if (p.MyPhysics != null && freeze)
            p.MyPhysics.body.velocity = Vector2.zero;
        p.moveable = !freeze;
    }

    public static void TintRed(PlayerControl p, bool red)
    {
        try
        {
            var color = red ? new Color(1f, .22f, .22f, 1f) : Color.white;
            foreach (var r in p.GetComponentsInChildren<SpriteRenderer>(true))
                if (r != null) r.color = color;
        }
        catch { }
    }

    public static void SetCounterGhostLook(PlayerControl p, bool on)
    {
        try
        {
            foreach (var r in p.GetComponentsInChildren<SpriteRenderer>(true))
            {
                if (r == null) continue;
                var c = r.color;
                if (on)
                    r.color = new Color(1f, .18f, .18f, .48f);
                else
                    r.color = new Color(1f, 1f, 1f, 1f);
            }
        }
        catch { }
    }

    public static bool HasRoleName(PlayerControl p, string roleName)
    {
        foreach (var c in p.GetComponents<Component>())
        {
            if (c == null) continue;
            var n = c.GetType().Name;
            if (n.Contains(roleName, StringComparison.OrdinalIgnoreCase)) return true;
            foreach (var prop in c.GetType().GetProperties(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))
            {
                if (!prop.CanRead || prop.GetIndexParameters().Length != 0) continue;
                object? v = null;
                try { v = prop.GetValue(c); } catch { }
                if (v != null && v.GetType().Name.Contains(roleName, StringComparison.OrdinalIgnoreCase)) return true;
            }
        }
        return false;
    }

    private static bool ReadBool(object o, string name)
    {
        try
        {
            var t = o.GetType();
            var p = t.GetProperty(name, BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
            if (p?.PropertyType == typeof(bool)) return (bool)p.GetValue(o)!;
            var f = t.GetField(name, BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
            if (f?.FieldType == typeof(bool)) return (bool)f.GetValue(o)!;
        }
        catch { }
        return false;
    }
}
