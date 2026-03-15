using Celeste.Mod.WarlockHelper.Components;
using System;
using Celeste.Mod.Helpers;
using Mono.Cecil.Cil;
using MonoMod.Cil;

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
        IL.Celeste.Player.CallDashEvents += Player_callDashEventsMod;
        DashDirOverrider.Load();
    }

    public override void Unload()
    {
        IL.Celeste.Player.CallDashEvents -= Player_callDashEventsMod;
        DashDirOverrider.Unload();
    }

    private static void playerBonusDashEvents(Player player)
    {
        player.GetSafe<DashDirOverrider>().OnDash();
        
    }

    private static bool playerGetSkipDashListeners(Player player)
    {
        return player.GetSafe<DataPlayer>().forcedDash;
    }
    private static Func<Instruction, bool>[] dashListenerUpdateBeginTarget =
    [
        new Func<Instruction, bool>(instr => instr.MatchLdarg(0)),
        new Func<Instruction, bool>(instr => instr.MatchCall(out _)),
        new Func<Instruction, bool>(instr => instr.MatchCallvirt(out _)),
        new Func<Instruction, bool>(instr => instr.MatchCallvirt(out _)),
        new Func<Instruction, bool>(instr => instr.MatchCallvirt(out _)),
        new Func<Instruction, bool>(instr => instr.MatchStloc(1)),
    ];
    private static Func<Instruction, bool>[] dashListenerUpdateEndTarget =
    [
        new Func<Instruction, bool>(instr => instr.MatchLdloca(1)),
        new Func<Instruction, bool>(instr => instr.MatchCall(out _)),
        new Func<Instruction, bool>(instr => instr.MatchBrtrue(out _)),
    ];
    private static void Player_callDashEventsMod (ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        Debug.Log($"Updating CIL code for {cursor.Method.FullName}", LogLevel.Debug, "Player_callDashEventsMod");
        cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdarg(0));
            cursor.EmitLdloc1();
            cursor.EmitDelegate(playerBonusDashEvents);
        cursor.TryGotoNextBestFit(MoveType.Before,dashListenerUpdateBeginTarget);
            cursor.EmitLdloc1(); //todo reset forcedDash for normal use
            cursor.EmitDelegate(playerGetSkipDashListeners);
            ILLabel AfterDashListeners = cursor.DefineLabel();
            cursor.EmitBrtrue(AfterDashListeners);
        cursor.TryGotoNextBestFit(MoveType.Before, dashListenerUpdateEndTarget);
            cursor.MarkLabel(AfterDashListeners);
    }
}