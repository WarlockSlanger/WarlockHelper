using System;
using Celeste.Mod.WarlockHelper.Triggers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.WarlockHelper.Components;

[Tracked]

public class DashModifier : Component
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
    public bool silent;
    public bool noCooldown;
    public bool changeInterrupt;
    public bool super;

    public DashModifier() : base(active: true, visible: false)
    {
        Reset();
    }
    public override void Added(Entity entity)
    {
        base.Added(entity);
        player = (Player)entity;
    }
    public void Reset()
    {
        silent = false;
        noCooldown = false;
        changeInterrupt = false;
        super = false;
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
        player.SetDefaultDashDir(DashDirTrigger.DataToFunc( WarlockHelperModule.ModSession.DefaultDashDirection) );
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