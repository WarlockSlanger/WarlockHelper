using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Core.Platforms;
using System;
using System.Reflection;

namespace Celeste.Mod.WarlockHelper.Components;

[Tracked]

public class DashDirOverrider : Component
{
    public Vector2? DashDir;
    private int? _count;
    private float? _duration;
    public int? Count
    {
        get => _count;
        private set
        {
            _count = value;
            if (_count <= 0) { Reset(); }
        }
    }
    public float? Duration
    {
        get =>_duration; 
        private set
        {
            _duration = value;
            if (_duration <= 0f) { Reset(); }
        }
    }
    public DashListener dashlistener;
    public void Set(Vector2? dashdir, int? count = 1, float? duration = null)
    {
        DashDir = dashdir;
        Count = count;
        Duration = duration;
    }
    public void Reset()
    {
        Set(null, null, null);
    }
    public DashDirOverrider() : base(active: true, visible: false) {
        dashlistener = new DashListener(OnDash);
        Reset();
    }
    public override void Added(Entity entity)
    {
        base.Added(entity);
        dashlistener.Added(entity);
    }
    public override void Removed(Entity entity)
    {
        base.Removed(entity);
        dashlistener.Removed(entity);
    }
    public override void Update() {
        base.Update();
        Duration -= Engine.DeltaTime;
    }
    public void OnDash(Vector2 dir)
    {
        Count--;
        Utils.Log($"dashed count to {Count}... because of dashlistener {Debug.ComponentIds[dashlistener]} for ddor {Debug.ComponentIds[this]}",LogLevel.Debug,nameof(DashDirOverrider));
    }
    
    internal static void Player_OnSpawn(Player player)
    {
        player.Add(new DashDirOverrider());
    }
    
}