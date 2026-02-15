using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Core.Platforms;
using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.WarlockHelper.Components;

[Tracked]

public class DashDirOverrider : Component
{
    public Func<Vector2?> defaultDashDir;
    public Vector2? altDashDir;

    public Vector2? DashDir
    {
        get
        {
            if (altDashDir != null)
            {
                return altDashDir;
            }
            if (defaultDashDir != null)
            {
                return defaultDashDir();
            }

            return null;
        }
    }
    private DashListener dashlistener;
    public void SetCurrent(Vector2? dashDir)
    {
        altDashDir = dashDir;
    }
    public void SetDefault(Func<Vector2?> dashDirFunc)
    {
        defaultDashDir = dashDirFunc;
    }

    public DashDirOverrider() : base(active: true, visible: false) {
        dashlistener = new DashListener(OnDash);
    }
    public override void Added(Entity entity)
    {
        base.Added(entity);
        entity.Add(dashlistener);
    }
    public override void Removed(Entity entity)
    {
        base.Removed(entity);
        entity.Remove(dashlistener);
    }
    public void OnDash(Vector2 dir)
    {
        SetCurrent(null);
    }

    internal static Vector2 playerGetDashDir(Vector2 fallback, Player player)
    {
        DashDirOverrider ddr = player.Get<DashDirOverrider>();
        Debug.Log($"using DashDirOverrider {Debug.ComponentIds[ddr]} with dl {Debug.ComponentIds[ddr.dashlistener]} to set dir to {ddr.DashDir}");
        return ddr.DashDir ?? fallback;
    }

    public static void Load() {
        dashCoroutineHook = new ILHook(typeof(Player).GetMethod("DashCoroutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), Player_redirDash);
        redDashCoroutineHook = new ILHook(typeof(Player).GetMethod("RedDashCoroutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), Player_redirDash);
        Everest.Events.Player.OnSpawn += player_OnSpawn;
    }

    public static void Unload()
    {
        dashCoroutineHook.Dispose();
        dashCoroutineHook = null;
        redDashCoroutineHook.Dispose();
        redDashCoroutineHook = null;
        Everest.Events.Player.OnSpawn -= player_OnSpawn;
    }
    public static ILHook dashCoroutineHook,redDashCoroutineHook;

    public static void player_OnSpawn(Player player)
    {
        player.SetDefaultDashDir(null);
    }
    private static void Player_redirDash (ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        Debug.Log($"Overwriting lastAim in CIL code for {cursor.Method.FullName}", LogLevel.Debug, "Player_redirDash");
        while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld<Player>("lastAim")))
        {
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.EmitDelegate(DashDirOverrider.playerGetDashDir);
        }
    }
}