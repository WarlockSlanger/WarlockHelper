using Celeste.Mod.WarlockHelper.Components;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Reflection;
using MonoMod.Logs;

namespace Celeste.Mod.WarlockHelper;

public class WarlockHelperModule : EverestModule {
    public static WarlockHelperModule Instance { get; private set; }

    public override Type SettingsType => typeof(WarlockHelperModuleSettings);
    public static WarlockHelperModuleSettings Settings => (WarlockHelperModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(WarlockHelperModuleSession);
    public static WarlockHelperModuleSession Session => (WarlockHelperModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(WarlockHelperModuleSaveData);
    public static WarlockHelperModuleSaveData SaveData => (WarlockHelperModuleSaveData) Instance._SaveData;
    
    internal const string HelperName = "WarlockHelper";

    public WarlockHelperModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(HelperName, LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(HelperName, LogLevel.Info);
#endif
    }

    public override void Load()
    {
        DashDirOverrider.Load();
    }

    public override void Unload()
    {
        DashDirOverrider.Unload();
    }
}