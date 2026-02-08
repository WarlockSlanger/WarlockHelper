using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Core.Platforms;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.WarlockHelper.Components;

[Tracked]

public class DashDirOverrider : Component
{
    private Func<Vector2?> defaultDashDir;
    private Vector2? altDashDir;

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
    public DashListener dashlistener;
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
        Utils.Log($"using DashDirOverrider {Debug.ComponentIds[ddr]} with dl {Debug.ComponentIds[ddr.dashlistener]} to set dir to {ddr.DashDir}");
        return ddr.DashDir ?? fallback;
    }

    public static void player_OnSpawn(Player player)
    {
        player.SetDefaultDashDir(null);
    }
}