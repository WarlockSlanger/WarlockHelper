using Microsoft.Xna.Framework;
using Monocle;
using System;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Celeste.Mod.WarlockHelper.Components;

[Tracked]

public class DashDirOverrider() : Component(active: true, visible: false)
{
    private Player player;
    private Func<Player,Vector2> defaultDashDir { get; set; }

    private Vector2? altDashDir { get; set; }

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
                return defaultDashDir(player);
            }

            return null;
        }
    }

    public override void Added(Entity entity)
    {
        base.Added(entity);
        player = (Player)entity;
    }

    public void SetCurrent(Vector2? dashDir)
    {
        altDashDir = dashDir;
    }
    public void SetDefault(Func<Player,Vector2> dashDirFunc)
    {
        defaultDashDir = dashDirFunc;
    }

    internal static void SetSessionDashDir(Player player)
    {
        player.SetDefaultDashDir(WarlockHelperModule.ModSession.DefaultDashDirection);
    }
    internal static void Load()
    {
        Everest.Events.Player.OnSpawn += SetSessionDashDir;
    }
    internal static void Unload()
    {
        Everest.Events.Player.OnSpawn -= SetSessionDashDir;
    }

    
}