using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.WarlockHelper.Entities;

[CustomEntity("WarlockHelper/SampleSolid")]
[Tracked]
public class SampleSolid : Solid {
    public char tileType;
    public DashCollisionResults dashCollisionResult;
    public float width;
    public float height;
    public bool activated;
    public TileGrid tileGrid;
    public SampleSolid(EntityData data, Vector2 offset)
        : base(data.Position + offset, data.Width, data.Height, safe:true)
    {
        width = data.Width;
        height = data.Height;
        Depth = Depths.Solids;
        dashCollisionResult = data.Enum("dashCollisionResult", DashCollisionResults.Rebound);
        tileType = data.Char("tiletype", '3');
        Add(new LightOcclude());
        SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
        OnDashCollide = OnDashed;
        activated = false;
    }
    public override void Added(Scene scene) { 
        base.Added(scene);
        tileGrid = GFX.FGAutotiler.GenerateBox(tileType, (int)width / 8, (int)height / 8).TileGrid;
        Add(tileGrid);
        Add(new TileInterceptor(tileGrid, highPriority: true));
        activated = false;
    }
    public override void OnShake(Vector2 amount)
    {
        base.OnShake(amount);
        tileGrid.Position += amount;
    }
    public void ToggleActive()
    {
        if (!activated)
        {
            StartShaking();
        }
        else
        {
            StopShaking();
        }
        activated = !activated;
    }
    private DashCollisionResults OnDashed(Player player, Vector2 direction)
    {
        ToggleActive();
        return dashCollisionResult;
    }

}