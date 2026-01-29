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


    public WarlockHelperModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(Utils.wsh, LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(Utils.wsh, LogLevel.Info);
#endif
    }
    ILHook dashCoroutineHook,redDashCoroutineHook;

    public override void Load() {
        dashCoroutineHook = new ILHook(typeof(Player).GetMethod("DashCoroutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), Player_redirDash);
        redDashCoroutineHook = new ILHook(typeof(Player).GetMethod("RedDashCoroutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), Player_redirDash);
        Everest.Events.Player.OnSpawn += Player_OnSpawn;
    }

    public override void Unload()
    {
        dashCoroutineHook.Dispose();
        dashCoroutineHook = null;
        redDashCoroutineHook.Dispose();
        redDashCoroutineHook = null;
        Everest.Events.Player.OnSpawn -= Player_OnSpawn;
    }
    private void Player_OnSpawn(Player player)
    {
        player.Add(new DashDirOverrider());
    }
    private static void Player_redirDash (ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld<Player>("lastAim")))
        {
            Logger.Log(LogLevel.Debug,$"{Utils.wsh}/Player_redirDash", $"Overwriting lastAim at {cursor.Index} in CIL code for {cursor.Method.FullName}");
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.EmitDelegate(playerOverrideDashDir);
        }
    }
    private static Vector2 playerOverrideDashDir(Vector2 fallback, Player player)
    {
        Vector2? overridedashdir = player.Get<DashDirOverrider>().DashDir;
        return overridedashdir ?? fallback;
    }
}