using System;
using System.Linq;
using System.Reflection;

namespace NunAddon.Gameplay;

public static class SabotageCooldownBridge
{
    public static void StartFromConjure()
    {
        try
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies.Where(x =>
                         x.GetName().Name is "TownOfUsMira" or "MiraAPI"))
            {
                foreach (var t in SafeTypes(a))
                {
                    foreach (var m in t.GetMethods(BindingFlags.Public |
                                                   BindingFlags.NonPublic |
                                                   BindingFlags.Static))
                    {
                        if (m.GetParameters().Length != 0) continue;
                        var n = m.Name;
                        if (!(n.Contains("ResetSabotage", StringComparison.OrdinalIgnoreCase) ||
                              n.Contains("StartSabotageCooldown", StringComparison.OrdinalIgnoreCase) ||
                              n.Contains("ResetSabo", StringComparison.OrdinalIgnoreCase))) continue;
                        try { m.Invoke(null, null); return; } catch { }
                    }
                }
            }
        }
        catch { }
    }

    private static Type[] SafeTypes(Assembly a)
    {
        try { return a.GetTypes(); }
        catch (ReflectionTypeLoadException e) { return e.Types.Where(x => x != null).Cast<Type>().ToArray(); }
        catch { return Array.Empty<Type>(); }
    }
}
