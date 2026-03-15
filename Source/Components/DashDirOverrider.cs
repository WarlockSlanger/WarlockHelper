using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.WarlockHelper.Components;

[Tracked]

public class DashDirOverrider : Component
{
    public Func<Player,Vector2> defaultDashDir { get; private set; }

    public Vector2? altDashDir { get; private set; }

    public Vector2 DashDir
    {
        get
        {
            if (altDashDir != null)
            {
                return (Vector2)altDashDir;
            }
            if (defaultDashDir != null)
            {
                return defaultDashDir((Player)Entity);
            }
            return Input.GetAimVector(((Player)Entity).Facing);
        }
    }
    public void SetCurrent(Vector2? dashDir)
    {
        altDashDir = dashDir;
    }
    public void SetDefault(Func<Player,Vector2> dashDirFunc)
    {
        defaultDashDir = dashDirFunc;
    }

    public DashDirOverrider() : base(active: true, visible: false) {
        defaultDashDir = null;
        altDashDir = null;
    }
    internal void OnDash()
    {
        SetCurrent(null);
    }

    private static Vector2 playerGetDashDir(Vector2 fallback, Player player)
    {
        DashDirOverrider ddr = player.Get<DashDirOverrider>();
        return ddr?.DashDir ?? fallback;
    }

    internal static void Load() {
        dashCoroutineHook = new ILHook(typeof(Player).GetMethod("DashCoroutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), Player_redirDash);
        redDashCoroutineHook = new ILHook(typeof(Player).GetMethod("RedDashCoroutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), Player_redirDash);
    }

    internal static void Unload()
    {
        dashCoroutineHook.Dispose();
        dashCoroutineHook = null;
        redDashCoroutineHook.Dispose();
        redDashCoroutineHook = null;
    }
    internal static ILHook dashCoroutineHook,redDashCoroutineHook;

    private static void Player_redirDash (ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        Debug.Log($"Overwriting lastAim in CIL code for {cursor.Method.FullName}", LogLevel.Debug, "Player_redirDash");
        while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld<Player>("lastAim")))
        {
            cursor.EmitLdloc1();
            cursor.EmitDelegate(playerGetDashDir);
        }
    }
}