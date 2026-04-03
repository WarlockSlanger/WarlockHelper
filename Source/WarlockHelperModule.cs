using System;
using Celeste.Mod.Helpers;
using Celeste.Mod.WarlockHelper.Components;
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

    private static void playerPreAllDashEvents(Player player)
    {
        player.GetSafe<DashDirOverrider>().OnDash();
    }

    private static void playerPostAllDashEvents(Player player)
    {
        player.GetSafe<DataPlayer>().forcedDash = false;
    }

    private static bool playerGetSkipDashListeners(Player player)
    {
        bool ans= player.GetSafe<DataPlayer>().forcedDash;
        return ans;
    }

    private static readonly Func<Instruction, bool>[] dashListenerUpdateBeginTarget =
        [
            instr => instr.MatchLdarg(0),
            instr => instr.MatchCall(out _),
            instr => instr.MatchCallvirt(out _),
            instr => instr.MatchCallvirt(out _),
            instr => instr.MatchCallvirt(out _),
            instr => instr.MatchStloc(1),
        ];

    private static readonly Func<Instruction, bool>[] earlyReturnDashEvents =
        [
            instr => instr.MatchLdarg(0),
            instr => instr.MatchLdfld<Player>("calledDashEvents"),
            instr => instr.MatchBrtrue(out _),
        ];

    private static void Player_callDashEventsMod (ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        Debug.Log($"Updating CIL code for {cursor.Method.FullName}", LogLevel.Debug, "Player_callDashEventsMod");

        if (!cursor.TryGotoNextBestFit(MoveType.After, earlyReturnDashEvents)) { return; } //forces repeat dash events to avoid PostAllDashEvents and directly return
        cursor.Index -= 1;
        ILLabel ToReturn = cursor.DefineLabel();
        cursor.EmitBrtrue(ToReturn);
        cursor.EmitLdcI4(0);

        if (!cursor.TryGotoNext(MoveType.After, instr => instr.MatchStfld<Player>("calledDashEvents"))) { return; } //first dash events
        cursor.EmitLdarg0();
        cursor.EmitDelegate(playerPreAllDashEvents);
            
        if (!cursor.TryGotoNextBestFit(MoveType.AfterLabel, dashListenerUpdateBeginTarget)) {return; } //attempts to skip dashlisteners
        cursor.EmitLdarg0();
        cursor.EmitDelegate(playerGetSkipDashListeners);
        ILLabel PostAllDashEvents = cursor.DefineLabel();
        cursor.EmitBrtrue(PostAllDashEvents);
        
        if (!cursor.TryGotoNext(MoveType.AfterLabel, instr => instr.MatchRet())) { return; } //last dash events, before return 
        cursor.MarkLabel(PostAllDashEvents);
        cursor.EmitLdarg0();
        cursor.EmitDelegate(playerPostAllDashEvents);
        
        cursor.TryGotoNext(MoveType.Before, instr => instr.MatchRet()); //direct return
        cursor.MarkLabel(ToReturn);
        
    }
}