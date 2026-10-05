using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MiraAPI.PluginLoading;
using Reactor;
using Reactor.Networking.Attributes;

namespace NunAddon;

[BepInPlugin(Id, "Nun Addon", "1.0.0")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency("mira.api")]
[BepInDependency("auavengers.tou.mira")]
[ReactorModFlags(Reactor.Networking.ModFlags.RequireOnAllClients)]
public sealed class NunPlugin : BasePlugin, IMiraPlugin
{
    public const string Id = "serena.toum.nun";
    private static NunPlugin? _instance;
    internal static void InstanceLog(string message) => _instance?.Log.LogInfo(message);

    public Harmony Harmony { get; } = new(Id);
    public string OptionsTitleText => "Nun";
    public string CustomOptionMenuNameTwo => "Nun Addon";
    public BepInEx.Configuration.ConfigFile GetConfigFile() => Config;

    public override void Load()
    {
        _instance = this;
        NunConfig.Bind(Config);
        Gameplay.NunState.Reset();
        Harmony.PatchAll();
        Log.LogInfo("Nun Addon 1.0 loaded. Mira should register NunRole and ConjureButton from this assembly.");
    }
}
