using System;
using Celeste.Mod.Helpers;
using Celeste.Mod.WarlockHelper.Components;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;
using Celeste.Mod.WarlockHelper.Entities;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.WarlockHelper;

public class WarlockHelperModule : EverestModule
{
    internal const string HelperName = "WarlockHelper";

    public static WarlockHelperModule Instance { get; private set; }

    public override Type SettingsType => typeof(WarlockHelperModuleSettings);
    public static WarlockHelperModuleSettings ModSettings => (WarlockHelperModuleSettings)Instance._Settings;

    public override Type SessionType => typeof(WarlockHelperModuleSession);
    public static WarlockHelperModuleSession ModSession => (WarlockHelperModuleSession)Instance._Session;

    public override Type SaveDataType => typeof(WarlockHelperModuleSaveData);
    public static WarlockHelperModuleSaveData ModSaveData => (WarlockHelperModuleSaveData)Instance._SaveData;


    public WarlockHelperModule()
    {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(HelperName, LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(HelperName, LogLevel.Info);
#endif
    }

    private EverestModuleMetadata gravityHelper = new()
    {
        Name = "GravityHelper",
        Version = new Version(1, 2, 28)
    };

    internal bool gravityHelperLoaded;

    public override void Load()
    {
        gravityHelperLoaded = Everest.Loader.DependencyLoaded(gravityHelper);
        IL.Celeste.Player.CallDashEvents += Player_callDashEventsMod;
        IL.Celeste.Player.DashBegin += Player_dashBeginHook;
        IL.Celeste.Player.RedDashBegin += Player_redDashBeginHook;
        IL.Celeste.Player.DashUpdate += Player_dashUpdateMod;
        IL.Celeste.Player.RedDashUpdate += Player_redDashUpdateMod;
        dashCoroutineHook =
            new ILHook(
                typeof(Player).GetMethod(nameof(Player.DashCoroutine), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .GetStateMachineTarget()!, Player_dashCoroutineMod);
        redDashCoroutineHook =
            new ILHook(
                typeof(Player).GetMethod(nameof(Player.RedDashCoroutine), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .GetStateMachineTarget()!, Player_redDashCoroutineMod);
        DashModifier.Load();
        CustomHoldable.Load();
        CustomTheoCrystal.Load();
    }

    public override void Unload()
    {
        IL.Celeste.Player.CallDashEvents -= Player_callDashEventsMod;
        IL.Celeste.Player.DashBegin -= Player_dashBeginHook;
        IL.Celeste.Player.RedDashBegin -= Player_redDashBeginHook;
        IL.Celeste.Player.DashUpdate -= Player_dashUpdateMod;
        IL.Celeste.Player.RedDashUpdate -= Player_redDashUpdateMod;
        dashCoroutineHook.Dispose();
        dashCoroutineHook = null;
        redDashCoroutineHook.Dispose();
        redDashCoroutineHook = null;
        DashModifier.Unload();
        CustomHoldable.Unload();
        CustomTheoCrystal.Unload();
    }

    private static ILHook dashCoroutineHook, redDashCoroutineHook;

    //Reusable hooks
    private static void overrideDashDir(ILCursor cursor, bool coroutine = false)
    {
        while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld<Player>(nameof(Player.lastAim))))
        {
            cursor.emitThis(coroutine);
            cursor.EmitDelegate(playerGetDashDir);
        }
    }

    private static void overrideSuperDash(ILCursor cursor, bool coroutine = false)
    {
        while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld<Assists>(nameof(Assists.SuperDashing))))
        {
            cursor.emitThis(coroutine);
            cursor.EmitDelegate(playerGetSuperDash);
        }
    }

    //Emittable Delegates
    private static void playerSetDashProperties(Player player)
    {
        var dmod = player.GetSafe<DashModifier>();
        var data = player.GetSafe<DataPlayer>();
        data.dashInterrupt = (player.StateMachine.State == 5) ^ dmod.changeInterrupt;
        data.dashListenersSkipped = dmod.silent;
        data.dashNoCD = dmod.noCooldown;
        data.dashSuper = dmod.super;
        dmod.Reset();
    }

    public static Vector2 playerGetDashDir(Vector2 orig, Player player)
    {
        return player.GetSafe<DashModifier>().DashDir ?? orig;
    }

    private static bool playerGetSuperDash(bool orig, Player player)
    {
        return orig || player.GetSafe<DataPlayer>().dashSuper;
    }

    private static bool playerGetDashNoCooldown(Player player)
    {
        return player.GetSafe<DataPlayer>().dashNoCD;
    }

    private static bool playerGetDashInterrupt(Player player)
    {
        return player.GetSafe<DataPlayer>().dashInterrupt;
    }


    private static void Player_dashCoroutineMod(ILContext il)
    {
        ILCursor cursor = new(il);
        overrideDashDir(cursor, true);

        cursor.Index = 0;
        overrideSuperDash(cursor, true);
    }

    private static void Player_redDashCoroutineMod(ILContext il)
    {
        ILCursor cursor = new(il);
        overrideDashDir(cursor, true);
    }


    private static readonly Func<Instruction, bool>[] dashCooldownTarget =
    [
        instr => instr.MatchLdarg(0),
        instr => instr.MatchLdcR4(0.2f),
        instr => instr.MatchStfld<Player>(nameof(Player.dashCooldownTimer)),
        instr => instr.MatchLdarg(0),
        instr => instr.MatchLdcR4(0.1f),
        instr => instr.MatchStfld<Player>(nameof(Player.dashRefillCooldownTimer)),
    ];

    private static void Player_dashBeginHook(ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.GotoNext(MoveType.After, instr => instr.MatchStfld<Player>(nameof(Player.calledDashEvents)));

        cursor.EmitLdarg0();
        cursor.EmitDelegate(playerSetDashProperties);
        cursor.GotoNextBestFit(MoveType.Before, 64, dashCooldownTarget);

        cursor.EmitLdarg0();
        cursor.EmitDelegate(playerGetDashNoCooldown);
        ILLabel AfterCooldowns = cursor.DefineLabel();
        cursor.EmitBrtrue(AfterCooldowns);
        cursor.GotoNextBestFit(MoveType.After, 64, dashCooldownTarget);

        cursor.MarkLabel(AfterCooldowns);
        cursor.Index = 0;
        overrideSuperDash(cursor);
    }

    private static void Player_redDashBeginHook(ILContext il)
    {
        ILCursor cursor = new(il);
        cursor.GotoNext(MoveType.After, instr => instr.MatchStfld<Player>(nameof(Player.calledDashEvents)));

        cursor.EmitLdarg0();
        cursor.EmitDelegate(playerSetDashProperties);
        cursor.GotoNextBestFit(MoveType.Before, 64, dashCooldownTarget);

        cursor.EmitLdarg0();
        cursor.EmitDelegate(playerGetDashNoCooldown);
        ILLabel AfterCooldowns = cursor.DefineLabel();
        cursor.EmitBrtrue(AfterCooldowns);
        cursor.GotoNextBestFit(MoveType.After, 64, dashCooldownTarget);

        cursor.MarkLabel(AfterCooldowns);
    }


    private static void Player_dashUpdateMod(ILContext il)
    {
        ILCursor cursor = new(il);

        cursor.GotoNext(MoveType.Before, instr => instr.MatchCallvirt<Player>("get_CanDash"));
        cursor.Index--;

        ILLabel CheckCanDash = cursor.DefineLabel();
        cursor.MarkLabel(CheckCanDash);

        cursor.GotoPrev(MoveType.Before, instr => instr.MatchLdfld<Assists>(nameof(Assists.SuperDashing)));

        cursor.Index -= 2;
        cursor.MoveAfterLabels();
        cursor.EmitLdarg0();
        cursor.EmitDelegate(playerGetDashInterrupt);
        cursor.EmitBrtrue(CheckCanDash);

        cursor.Index = 0;
        overrideSuperDash(cursor);
    }

    private static void Player_redDashUpdateMod(ILContext il)
    {
        ILCursor cursor = new(il);

        cursor.GotoNext(MoveType.After, instr => instr.MatchCallvirt<Player>("get_CanDash"));

        ILLabel NoDash = (ILLabel)cursor.Next!.Operand;
        cursor.Index++;

        cursor.EmitLdarg0();
        cursor.EmitDelegate(playerGetDashInterrupt);

        cursor.EmitBrfalse(NoDash);
    }

    //CallDashEvents

    private static void playerPreDashEvents(Player player)
    {
        var ddo = player.GetSafe<DashModifier>();
        ddo.SetCurrent(null);
    }

    private static void playerPostDashEvents(Player player)
    {
    }

    private static bool playerGetSkipDashListeners(Player player)
    {
        return player.GetSafe<DataPlayer>().dashListenersSkipped;
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
        instr => instr.MatchLdfld<Player>(nameof(Player.calledDashEvents)),
        instr => instr.MatchBrtrue(out _),
    ];

    private static void Player_callDashEventsMod(ILContext il)
    {
        ILCursor cursor = new(il);
        /*
        cursor.GotoNextBestFit(MoveType.After, earlyReturnDashEvents);
        cursor.Index--;
        ILLabel ToReturn = cursor.DefineLabel();
        cursor.EmitBrtrue(ToReturn);
        cursor.EmitLdcI4(0);
        */
        cursor.GotoNext(MoveType.After, instr => instr.MatchStfld<Player>("calledDashEvents"));

        cursor.EmitLdarg0();
        cursor.EmitDelegate(playerPreDashEvents);

        cursor.GotoNextBestFit(MoveType.AfterLabel, dashListenerUpdateBeginTarget);
        cursor.EmitLdarg0();
        cursor.EmitDelegate(playerGetSkipDashListeners);
        ILLabel PostAllDashEvents = cursor.DefineLabel();
        cursor.EmitBrtrue(PostAllDashEvents);

        cursor.GotoNext(MoveType.AfterLabel, instr => instr.MatchRet());

        cursor.MarkLabel(PostAllDashEvents);
        /*
        cursor.EmitLdarg0();
        cursor.EmitDelegate(playerPostDashEvents);

        cursor.GotoNext(MoveType.Before, instr => instr.MatchRet());
        cursor.MarkLabel(ToReturn);
        */
    }
}