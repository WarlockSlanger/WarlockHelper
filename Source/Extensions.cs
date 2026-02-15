using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Core.Platforms;
using System;
using Celeste.Mod.WarlockHelper.Components;

namespace Celeste.Mod.WarlockHelper;

public static class Extensions
{
    public static void ForceDash(this Player player, Vector2? dashdir = null, bool red = false)
    {
        SetNextDashDir(player,dashdir);
        player.StateMachine.ForceState(red ? Player.StRedDash : Player.StDash);
    }

    public static DashDirOverrider GetDashDirOverrider(this Player player)
    {
        DashDirOverrider ddr = player.Get<DashDirOverrider>();
        if (ddr == null)
        {
            ddr = new DashDirOverrider();
            player.Add(ddr);
        }
        return ddr;
    }

    public static void SetNextDashDir(this Player player, Vector2? dashdir = null)
    {
        DashDirOverrider ddr = player.GetDashDirOverrider();
        ddr.SetCurrent(dashdir);
        Debug.Log(
            $"Forcing next Player dash direction to {dashdir}",LogLevel.Verbose,"SetNextDashDir");
    }

    public static void SetDefaultDashDir(this Player player, Func<Vector2?> dashdirfunc)
    {
        DashDirOverrider ddr = player.GetDashDirOverrider();
        ddr.SetDefault(dashdirfunc);
        Debug.Log(
            $"Setting default Player dash direction to {dashdirfunc}",LogLevel.Verbose,"SetDefaultDashDir");
    }
}