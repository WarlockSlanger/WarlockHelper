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
    ILHook dashCoroutineHook,redDashCoroutineHook;

    public override void Load() {
        dashCoroutineHook = new ILHook(typeof(Player).GetMethod("DashCoroutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), Player_redirDash);
        redDashCoroutineHook = new ILHook(typeof(Player).GetMethod("RedDashCoroutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), Player_redirDash);
        Everest.Events.Player.OnSpawn += DashDirOverrider.player_OnSpawn;
    }

    public override void Unload()
    {
        dashCoroutineHook.Dispose();
        dashCoroutineHook = null;
        redDashCoroutineHook.Dispose();
        redDashCoroutineHook = null;
        Everest.Events.Player.OnSpawn -= DashDirOverrider.player_OnSpawn;
    }
    private static void Player_redirDash (ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        Utils.Log($"Overwriting lastAim in CIL code for {cursor.Method.FullName}", LogLevel.Debug, "Player_redirDash");
        while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld<Player>("lastAim")))
        {
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.EmitDelegate(DashDirOverrider.playerGetDashDir);
        }
    }
}